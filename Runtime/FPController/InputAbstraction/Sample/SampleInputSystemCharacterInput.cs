using UnityEngine;
using UnityEngine.InputSystem;

namespace KadaXuanwu.Utils.Runtime.FPController.InputAbstraction.Sample {
    /// <summary>
    /// Default implementation using Unity's Input System via InputManager.
    /// </summary>
    public class SampleInputSystemCharacterInput : ICharacterInput {
        private InputAction _move;
        private InputAction _look;
        private InputAction _jump;
        private InputAction _sprint;
        private InputAction _crouch;

        public Vector2 MoveInput => _move?.ReadValue<Vector2>() ?? Vector2.zero;
        public Vector2 LookInput => _look?.ReadValue<Vector2>() ?? Vector2.zero;
        public bool JumpPressed => _jump?.WasPressedThisFrame() ?? false;
        public bool JumpHeld => _jump?.IsPressed() ?? false;
        public bool SprintPressed => _sprint?.IsPressed() ?? false;
        public bool CrouchPressed => _crouch?.IsPressed() ?? false;

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