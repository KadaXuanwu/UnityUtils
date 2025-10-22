using UnityEditor;
using UnityEngine;
using KadaXuanwu.Utils.Runtime.Components;

namespace KadaXuanwu.Utils.Editor.Components {
    [CustomEditor(typeof(DestroyOnCollision))]
    public class DestroyOnCollisionEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (target is DestroyOnCollision component) {
                if (!component.TryGetComponent<Collider>(out _) && !component.TryGetComponent<Collider2D>(out _)) {
                    EditorGUILayout.HelpBox("No Collider or Collider2D found! This component requires a collider to detect collisions.", MessageType.Warning);
                }

                if (!component.TryGetComponent<Rigidbody>(out _) && !component.TryGetComponent<Rigidbody2D>(out _)) {
                    EditorGUILayout.HelpBox("No Rigidbody found. OnCollisionEnter requires at least one object to have a Rigidbody.", MessageType.Info);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("GameObject will be destroyed after the specified delay when colliding with the specified layers.", MessageType.Info);
        }
    }
}
