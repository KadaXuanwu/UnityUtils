using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.Components {
    public class FollowTransform : MonoBehaviour {
        [SerializeField] private Transform targetTransform;

        private void LateUpdate() {
            if (targetTransform == null) {
                return;
            }

            transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
        }
    }
}
