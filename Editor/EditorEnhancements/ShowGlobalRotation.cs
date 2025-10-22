using UnityEngine;
using UnityEditor;

namespace KadaXuanwu.Utils.Editor.EditorEnhancements {
    /// <summary>
    /// Custom editor extension for Transform that displays global position, rotation, and scale as read-only fields.
    /// Does not interfere with the default Transform inspector functionality.
    /// </summary>
    [CustomEditor(typeof(Transform))]
    public class TransformExtension : UnityEditor.Editor {
        private static bool _showGlobalInfo = false;

        public override void OnInspectorGUI() {
            Transform transform = (Transform)target;

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            
            Vector3 position = EditorGUILayout.Vector3Field("Position", transform.localPosition);
            Vector3 rotation = EditorGUILayout.Vector3Field("Rotation", transform.localEulerAngles);
            Vector3 scale = EditorGUILayout.Vector3Field("Scale", transform.localScale);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(transform, "Transform Change");
                transform.localPosition = position;
                transform.localEulerAngles = rotation;
                transform.localScale = scale;
            }

            serializedObject.ApplyModifiedProperties();

            _showGlobalInfo = EditorGUILayout.Foldout(_showGlobalInfo, "Global Transform", true);
            
            if (_showGlobalInfo) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Vector3Field("Global Position", transform.position);
                EditorGUILayout.Vector3Field("Global Rotation", transform.rotation.eulerAngles);
                EditorGUILayout.Vector3Field("Global Scale", transform.lossyScale);
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}