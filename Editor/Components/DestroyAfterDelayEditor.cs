using UnityEditor;
using KadaXuanwu.Utils.Runtime.Components;

namespace KadaXuanwu.Utils.Editor.Components {
    [CustomEditor(typeof(DestroyAfterDelay))]
    public class DestroyAfterDelayEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            SerializedProperty delayProp = serializedObject.FindProperty("delay");

            if (delayProp != null && delayProp.floatValue < 0f) {
                EditorGUILayout.HelpBox("Negative delays will be converted to positive values using Mathf.Abs().", MessageType.Warning);
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("GameObject will be destroyed after the specified delay when enabled.", MessageType.Info);
        }
    }
}
