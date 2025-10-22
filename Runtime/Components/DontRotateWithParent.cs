using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.Components {
    public class DontRotateWithParent : MonoBehaviour {
        private void LateUpdate() {
            transform.rotation = Quaternion.Euler(0f, 0f, transform.parent.transform.rotation.z * -1f);
        }
    }
}
