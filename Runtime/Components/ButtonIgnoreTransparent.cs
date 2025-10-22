using UnityEngine;
using UnityEngine.UI;

namespace KadaXuanwu.Utils.Runtime.Components {
    [RequireComponent(typeof(Image))]
    public class ButtonIgnoreTransparent : MonoBehaviour {
        private void Start() {
            GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
    }
}
