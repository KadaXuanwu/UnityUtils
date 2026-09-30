using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.FPController.Core {
    /// <summary>The default <see cref="ICharacterMotor"/>: Unity's <see cref="CharacterController"/>, a capsule.</summary>
    public sealed class CharacterControllerMotor : ICharacterMotor {
        private const float HeadroomTolerance = 0.01f;
        private static readonly Collider[] HeadroomHits = new Collider[16];

        private readonly CharacterController _controller;

        public CharacterControllerMotor(CharacterController controller) {
            _controller = controller;
        }

        public bool Enabled { get => _controller.enabled; set => _controller.enabled = value; }
        public bool IsGrounded => _controller.isGrounded;
        public Vector3 Velocity => _controller.velocity;
        public float SlopeLimit => _controller.slopeLimit;

        public CollisionFlags Move(Vector3 motion) => _controller.Move(motion);

        public void SetShape(float height, float centerY) {
            _controller.height = height;
            _controller.center = new Vector3(0f, centerY, 0f);
        }

        /// <summary>
        /// Checks the full width, so a ceiling edge off to one side blocks too, not only one straight over the
        /// centre. The controller's own collider and triggers do not count.
        /// </summary>
        public bool HasHeadroom(float height, float centerY) {
            float radius = _controller.radius;
            Vector3 position = _controller.transform.position;
            float currentTop = _controller.center.y + _controller.height / 2f;
            float top = centerY + height / 2f;
            Vector3 bottom = position + Vector3.up * (currentTop - radius);
            Vector3 upper = position + Vector3.up * (Mathf.Max(top, currentTop) - radius);

            // A hair narrower, so a wall the capsule already rests against does not count.
            int count = Physics.OverlapCapsuleNonAlloc(bottom, upper, radius - HeadroomTolerance, HeadroomHits,
                Physics.AllLayers, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++) {
                if (HeadroomHits[i] != _controller) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>The controller caches its position, so it is off while the transform moves.</summary>
        public void Teleport(Vector3 position) {
            bool wasEnabled = _controller.enabled;
            _controller.enabled = false;
            _controller.transform.position = position;
            _controller.enabled = wasEnabled;
        }
    }
}
