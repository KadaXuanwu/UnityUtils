using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

namespace KadaXuanwu.Utils.Runtime.FPController.InputAbstraction {
    /// <summary>
    /// Default implementation using Unity's Input System via InputManager.
    /// </summary>
    [MovedFrom(true, "KadaXuanwu.Utils.Runtime.FPController.InputAbstraction.Samples", null, "SampleInputSystemCharacterInput")]
    public class InputSystemCharacterInput : ICharacterInput {
        private InputAction _move;
        private InputAction _look;
        private InputAction _jump;
        private InputAction _sprint;
        private InputAction _crouch;

        /// <summary>
        /// Look speed of a stick at full tilt, in mouse pixels per second. A stick reports a rate (-1..1)
        /// where a mouse reports a per-frame delta in pixels, so LookInput scales a stick by this and the
        /// frame time; the controller's mouse sensitivity then means the same on both.
        /// </summary>
        public float GamepadLookSpeed { get; set; } = 4500f;

        public Vector2 MoveInput => _move?.ReadValue<Vector2>() ?? Vector2.zero;
        public bool JumpPressed => _jump?.WasPressedThisFrame() ?? false;
        public bool JumpHeld => _jump?.IsPressed() ?? false;
        public bool SprintPressed => _sprint?.IsPressed() ?? false;
        public bool CrouchPressed => _crouch?.IsPressed() ?? false;

        public Vector2 LookInput {
            get {
                if (_look == null) {
                    return Vector2.zero;
                }
                Vector2 look = _look.ReadValue<Vector2>();
                return _look.activeControl?.device is Gamepad or Joystick ? look * (GamepadLookSpeed * Time.deltaTime) : look;
            }
        }

        public void Initialize(InputActionMap actionMap) {
            if (actionMap == null) {
                Debug.LogError("ActionMap is null!");
                return;
            }

            _move = actionMap.FindAction("Move");
            _look = actionMap.FindAction("Look");
            _jump = actionMap.FindAction("Jump");
            _sprint = actionMap.FindAction("Sprint");
            _crouch = actionMap.FindAction("Crouch");
        }
    }
}
