using UnityEditor;
using UnityEngine;
using KadaXuanwu.Utils.Runtime.Components;

namespace KadaXuanwu.Utils.Editor.Components {
    [CustomEditor(typeof(FollowTransform))]
    public class FollowTransformEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            SerializedProperty targetProp = serializedObject.FindProperty("targetTransform");

            if (targetProp != null && targetProp.objectReferenceValue == null) {
                EditorGUILayout.HelpBox("No target transform assigned. This GameObject will not move.", MessageType.Warning);
            }

            if (target is FollowTransform component && targetProp?.objectReferenceValue != null) {
                Transform targetTransform = targetProp.objectReferenceValue as Transform;
                if (targetTransform == component.transform) {
                    EditorGUILayout.HelpBox("Target transform is set to itself! This will cause issues.", MessageType.Error);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This GameObject will follow the target transform's position and rotation in LateUpdate.", MessageType.Info);
        }
    }
}
