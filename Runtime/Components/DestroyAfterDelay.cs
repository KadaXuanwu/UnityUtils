using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.Components {
    public class DestroyAfterDelay : MonoBehaviour {
        [SerializeField] private float delay;

        private void OnEnable() {
            if (gameObject != null) {
                Destroy(gameObject, Mathf.Abs(delay));
            }
        }
    }
}
