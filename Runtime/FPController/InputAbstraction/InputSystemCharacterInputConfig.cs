using KadaXuanwu.Utils.Runtime.Input;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace KadaXuanwu.Utils.Runtime.FPController.InputAbstraction {
    [CreateAssetMenu(fileName = "InputSystemConfig", menuName = "KadaXuanwu Utils/FPController/Input System Config")]
    [MovedFrom(true, "KadaXuanwu.Utils.Runtime.FPController.InputAbstraction.Samples", null, "SampleInputSystemCharacterInputConfig")]
    public class InputSystemCharacterInputConfig : ScriptableObject, ICharacterInputConfig {
        [SerializeField] private string _actionMapName = "Player";

        [Tooltip("Look speed of a gamepad or joystick stick at full tilt, in mouse pixels per second. 4500 is about 180 degrees per second at the controller's mouse sensitivity of 5.")]
        [Min(0f)]
        [SerializeField] private float _gamepadLookSpeed = 4500f;

        public ICharacterInput CreateInput() {
            if (InputManager.S == null) {
                Debug.LogError("InputManager.S is null! Ensure InputManager exists in scene.");
                return null;
            }

            InputSystemCharacterInput input = new InputSystemCharacterInput { GamepadLookSpeed = _gamepadLookSpeed };
            input.Initialize(InputManager.S.GetActionMap(_actionMapName));
            return input;
        }
    }
}
