using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.FPController.Core {
    /// <summary>
    /// What moves a <see cref="FirstPersonController"/>'s body and collides it with the world. The controller
    /// uses a component implementing this on its GameObject, or else wraps its <see cref="CharacterController"/>
    /// in a <see cref="CharacterControllerMotor"/>.
    /// </summary>
    public interface ICharacterMotor {
        /// <summary>Off, <see cref="Move"/> is not called and the body passes through nothing.</summary>
        bool Enabled { get; set; }

        /// <summary>Whether the last <see cref="Move"/> ended on the ground.</summary>
        bool IsGrounded { get; }

        /// <summary>How fast the last <see cref="Move"/> actually went, after collisions.</summary>
        Vector3 Velocity { get; }

        /// <summary>Degrees of ground slope the body still stands on.</summary>
        float SlopeLimit { get; }

        CollisionFlags Move(Vector3 motion);

        /// <summary>Resizes the collision shape along the vertical: its height and its centre above the transform.</summary>
        void SetShape(float height, float centerY);

        /// <summary>
        /// Whether the shape, resized to this where it stands, would overlap nothing above its current top.
        /// </summary>
        bool HasHeadroom(float height, float centerY);

        void Teleport(Vector3 position);
    }
}
