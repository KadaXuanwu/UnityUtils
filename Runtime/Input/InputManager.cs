using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KadaXuanwu.Utils.Runtime.Input {
    /// <summary>
    /// Centralized input manager that works with any InputActionAsset.
    /// Assign your Input Actions asset in the inspector - no code generation dependency.
    /// 
    /// SETUP:
    /// 1. Create Input Actions asset with your bindings
    /// 2. Add InputManager to a persistent GameObject
    /// 3. Assign your InputActionAsset in the inspector
    /// 
    /// USAGE:
    ///     // Get action map
    ///     InputActionMap playerMap = InputManager.Instance.GetActionMap("Player");
    ///     
    ///     // Get specific action
    ///     InputAction moveAction = InputManager.Instance.GetAction("Player", "Move");
    ///     Vector2 move = moveAction.ReadValue&lt;Vector2&gt;();
    ///     
    ///     // Events
    ///     InputManager.Instance.GetAction("Player", "Jump").performed += OnJump;
    /// </summary>
    public class InputManager : MonoBehaviour {
        public static InputManager S { get; private set; }

        [SerializeField] private InputActionAsset _inputActions;

        public InputActionAsset Actions => _inputActions;
        public bool InputEnabled { get; private set; } = true;

        public event Action OnInputEnabled;
        public event Action OnInputDisabled;

        private InputActionMap _activeActionMap;

        private void Awake() {
            if (S != null && S != this) {
                Destroy(gameObject);
                return;
            }

            S = this;
            DontDestroyOnLoad(gameObject);

            if (_inputActions == null) {
                Debug.LogError("[InputManager] No InputActionAsset assigned!");
                return;
            }

            _activeActionMap = _inputActions.FindActionMap("Player");
        }

        private void OnEnable() {
            EnableAllInput();
        }

        private void OnDisable() {
            DisableAllInput();
        }

        private void OnDestroy() {
            if (S == this) {
                S = null;
            }
        }

        public InputActionMap GetActionMap(string name) {
            return _inputActions?.FindActionMap(name);
        }

        public InputAction GetAction(string mapName, string actionName) {
            return _inputActions?.FindActionMap(mapName)?.FindAction(actionName);
        }

        public InputAction GetAction(string actionName) {
            return _inputActions?.FindAction(actionName);
        }

        public void EnableAllInput() {
            _inputActions?.Enable();
            InputEnabled = true;
            OnInputEnabled?.Invoke();
        }

        public void DisableAllInput() {
            _inputActions?.Disable();
            InputEnabled = false;
            OnInputDisabled?.Invoke();
        }

        public void SwitchToActionMap(string actionMapName) {
            InputActionMap newMap = GetActionMap(actionMapName);
            SwitchToActionMap(newMap);
        }

        public void SwitchToActionMap(InputActionMap actionMap) {
            if (_activeActionMap == actionMap || actionMap == null) {
                return;
            }

            _activeActionMap?.Disable();
            _activeActionMap = actionMap;

            if (InputEnabled) {
                _activeActionMap.Enable();
            }
        }

        public void EnableActionMap(string name) {
            if (InputEnabled) {
                GetActionMap(name)?.Enable();
            }
        }

        public void DisableActionMap(string name) {
            GetActionMap(name)?.Disable();
        }

        public void DisableInputForDuration(float seconds) {
            StartCoroutine(DisableInputCoroutine(seconds));
        }

        public string GetKeyBindingForAction(string actionName) {
            InputAction action = GetAction(actionName);
            if (action == null)
                return string.Empty;

            return action.GetBindingDisplayString();
        }

        public string GetKeyBindingForAction(string actionName, string controlScheme) {
            InputAction action = GetAction(actionName);
            if (action == null)
                return string.Empty;

            return action.GetBindingDisplayString(group: controlScheme);
        }

        private System.Collections.IEnumerator DisableInputCoroutine(float seconds) {
            DisableAllInput();
            yield return new WaitForSecondsRealtime(seconds);
            EnableAllInput();
        }
    }
}