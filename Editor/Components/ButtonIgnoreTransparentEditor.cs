using UnityEditor;
using KadaXuanwu.Utils.Runtime.Components;

namespace KadaXuanwu.Utils.Editor.Components {
    [CustomEditor(typeof(ButtonIgnoreTransparent))]
    public class ButtonIgnoreTransparentEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            EditorGUILayout.HelpBox("This component sets alphaHitTestMinimumThreshold to 0.1 on Start.\nTransparent parts of the image won't register clicks.", MessageType.Info);

            if (target is ButtonIgnoreTransparent component) {
                if (!component.TryGetComponent<UnityEngine.UI.Image>(out _)) {
                    EditorGUILayout.HelpBox("Missing Image component! This script requires an Image component.", MessageType.Error);
                }
            }
        }
    }
}
