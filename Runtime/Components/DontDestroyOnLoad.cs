using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.Components {
    public class DontDestroyOnLoad : MonoBehaviour {
        private void Start() {
            DontDestroyOnLoad(this);
        }
    }
}
