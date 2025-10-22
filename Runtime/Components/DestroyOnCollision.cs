using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.Components {
    public class DestroyOnCollision : MonoBehaviour {
        [SerializeField] private LayerMask layers;
        [SerializeField] private int delay;

        private void OnCollisionEnter(Collision other) {
            if ((layers.value & (1 << other.gameObject.layer)) == 0) {
                return;
            }

            Destroy(gameObject, delay);
        }
    }
}
