using KadaXuanwu.Utils.Runtime.Input;
using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.FPController.InputAbstraction.Samples {
    [CreateAssetMenu(fileName = "SampleInputSystemConfig", menuName = "KadaXuanwu Utils/PFController/Sample Input Config")]
    public class SampleInputSystemCharacterInputConfig : ScriptableObject, ICharacterInputConfig {
        [SerializeField] private string _actionMapName = "Player";

        public ICharacterInput CreateInput() {
            if (InputManager.S == null) {
                Debug.LogError("InputManager.Instance is null! Ensure InputManager exists in scene.");
                return null;
            }

            SampleInputSystemCharacterInput input = new SampleInputSystemCharacterInput();
            input.Initialize(InputManager.S.GetActionMap(_actionMapName));
            return input;
        }
    }
}