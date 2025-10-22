using UnityEditor;
using KadaXuanwu.Utils.Runtime.Components;

namespace KadaXuanwu.Utils.Editor.Components {
    [CustomEditor(typeof(DontRotateWithParent))]
    public class DontRotateWithParentEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            if (target is DontRotateWithParent component) {
                if (component.transform.parent == null) {
                    EditorGUILayout.HelpBox("This GameObject has no parent. The component will have no effect.", MessageType.Warning);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This component cancels out the Z-axis rotation inherited from the parent.\nUseful for UI elements or sprites that should remain upright.", MessageType.Info);
        }
    }
}
