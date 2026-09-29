using KadaXuanwu.Utils.Runtime.FPController.Core;
using KadaXuanwu.Utils.Runtime.FPController.Modifiers.Base;
using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.FPController.Modifiers.Crouch {
    public class CrouchModifier : MovementModifierBase<CrouchConfig, CrouchEvents> {
        private const float HeadroomTolerance = 0.01f;
        private static readonly Collider[] HeadroomHits = new Collider[16];

        public CrouchModifier(CrouchConfig config) : base(config) { }

        public override void ProcessMovement(ref MovementContext context) {
            CrouchState state = context.State.GetOrCreate<CrouchState>();

            bool wantsToCrouch = Input.CrouchPressed;

            // Check if we can stand up
            if (state.WasCrouchingLastFrame && !wantsToCrouch) {
                if (!CanStandUp()) {
                    wantsToCrouch = true;
                    Events.InvokeCrouchBlocked();
                }
            }

            state.IsCrouching = wantsToCrouch;

            if (state.IsCrouching) {
                context.SpeedMultiplier *= Config.SpeedMultiplier;
            }

            UpdateControllerHeight(state.IsCrouching);
            UpdateCameraPosition(state.IsCrouching, context.DeltaTime);

            if (state.IsCrouching != state.WasCrouchingLastFrame) {
                Events.InvokeCrouchChanged(state.IsCrouching);
            }

            state.WasCrouchingLastFrame = state.IsCrouching;
        }

        private bool CanStandUp() {
            CharacterController cc = Controller.CharacterController;
            return HasHeadroom(Controller.transform.position, cc.radius, Config, cc);
        }

        /// <summary>
        /// Whether the standing capsule fits at <paramref name="position"/>: nothing but
        /// <paramref name="self"/> overlaps the part of it above the crouching capsule. Checks the full
        /// width, so a ceiling edge off to one side blocks too, not only one straight over the centre.
        /// </summary>
        public static bool HasHeadroom(Vector3 position, float radius, CrouchConfig config, Collider self) {
            float crouchingTop = config.CrouchingCenterY + config.CrouchingHeight / 2f;
            float standingTop = config.StandingCenterY + config.StandingHeight / 2f;
            Vector3 bottom = position + Vector3.up * (crouchingTop - radius);
            Vector3 top = position + Vector3.up * (Mathf.Max(standingTop, crouchingTop) - radius);

            // A hair narrower, so a wall the capsule already rests against does not count.
            int count = Physics.OverlapCapsuleNonAlloc(bottom, top, radius - HeadroomTolerance, HeadroomHits,
                Physics.AllLayers, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++) {
                if (HeadroomHits[i] != self) {
                    return false;
                }
            }
            return true;
        }

        private void UpdateControllerHeight(bool isCrouching) {
            CharacterController cc = Controller.CharacterController;
            cc.height = isCrouching ? Config.CrouchingHeight : Config.StandingHeight;
            cc.center = new Vector3(0f, isCrouching ? Config.CrouchingCenterY : Config.StandingCenterY, 0f);
        }

        private void UpdateCameraPosition(bool isCrouching, float deltaTime) {
            Transform cameraHolder = Controller.CameraHolder;
            if (cameraHolder == null) {
                return;
            }

            float targetY = isCrouching ? Config.CameraCrouchingY : Config.CameraStandingY;
            Vector3 pos = cameraHolder.localPosition;
            pos.y = Mathf.MoveTowards(pos.y, targetY, Config.TransitionSpeed * deltaTime);
            cameraHolder.localPosition = pos;
        }
    }
}
