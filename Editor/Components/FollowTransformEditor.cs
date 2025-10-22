using UnityEditor;
using UnityEngine;
using KadaXuanwu.Utils.Runtime.Components;

namespace KadaXuanwu.Utils.Editor.Components {
    [CustomEditor(typeof(FollowTransformCode))]
    public class FollowTransformCodeEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            EditorGUILayout.HelpBox("This component is controlled through code.\nUse SetTargetTransform() to assign a target at runtime.", MessageType.Info);

            if (Application.isPlaying) {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Runtime Info", EditorStyles.boldLabel);

                FollowTransformCode component = target as FollowTransformCode;
                System.Reflection.FieldInfo targetField = typeof(FollowTransformCode).GetField("_targetTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (targetField != null) {
                    Transform targetTransform = targetField.GetValue(component) as Transform;
                    EditorGUILayout.LabelField("Current Target:", targetTransform != null ? targetTransform.name : "None");
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Methods:\n• SetTargetTransform(Transform)\n• SetTargetTransform(Transform, Vector3 offset, Quaternion offset)\n• ResetTargetTransform()", MessageType.None);
        }
    }
}
