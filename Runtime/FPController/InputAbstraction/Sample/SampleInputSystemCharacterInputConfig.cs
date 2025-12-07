using KadaXuanwu.Utils.Runtime.Input;
using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.FPController.InputAbstraction.Sample {
    [CreateAssetMenu(fileName = "SampleInputSystemConfig", menuName = "Character/Input/Sample Input System")]
    public class SampleInputSystemCharacterInputConfig : ScriptableObject, ICharacterInputConfig {
        [SerializeField] private string _actionMapName = "Player";

        public ICharacterInput CreateInput() {
            if (InputManager.Instance == null) {
                Debug.LogError("InputManager.Instance is null! Ensure InputManager exists in scene.");
                return null;
            }

            SampleInputSystemCharacterInput input = new SampleInputSystemCharacterInput();
            input.Initialize(InputManager.Instance.GetActionMap(_actionMapName));
            return input;
        }
    }
}