using KadaXuanwu.Utils.Runtime.FPController.Core;
using KadaXuanwu.Utils.Runtime.FPController.Modifiers.Base;
using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.FPController.Modifiers.Crouch {
    public class CrouchModifier : MovementModifierBase<CrouchConfig, CrouchEvents> {
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
            return Controller.Motor.HasHeadroom(Config.StandingHeight, Config.StandingCenterY);
        }

        private void UpdateControllerHeight(bool isCrouching) {
            Controller.Motor.SetShape(isCrouching ? Config.CrouchingHeight : Config.StandingHeight,
                isCrouching ? Config.CrouchingCenterY : Config.StandingCenterY);
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
