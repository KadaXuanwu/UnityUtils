using UnityEditor;
using UnityEngine;
using KadaXuanwu.Utils.Runtime.FPController.Core;
using KadaXuanwu.Utils.Runtime.FPController.InputAbstraction;
using KadaXuanwu.Utils.Runtime.FPController.Core.Interfaces;

namespace KadaXuanwu.Utils.Editor.FPController {
    [CustomEditor(typeof(FirstPersonController))]
    public class FirstPersonControllerEditor : UnityEditor.Editor {
        private SerializedProperty _config;
        private SerializedProperty _inputConfig;
        private SerializedProperty _modifierConfigs;
        private SerializedProperty _playerVisuals;
        private SerializedProperty _cameraHolder;
        private SerializedProperty _groundCheckOrigin;

        private void OnEnable() {
            _config = serializedObject.FindProperty("config");
            _inputConfig = serializedObject.FindProperty("inputConfig");
            _modifierConfigs = serializedObject.FindProperty("modifierConfigs");
            _playerVisuals = serializedObject.FindProperty("playerVisuals");
            _cameraHolder = serializedObject.FindProperty("cameraHolder");
            _groundCheckOrigin = serializedObject.FindProperty("groundCheckOrigin");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            FirstPersonController controller = (FirstPersonController)target;

            DrawConfigSection();
            DrawInputSection();
            DrawModifiersSection();
            DrawReferencesSection();
            DrawRuntimeInfo(controller);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConfigSection() {
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_config);

            if (_config.objectReferenceValue == null) {
                EditorGUILayout.HelpBox("Controller Config is required!", MessageType.Error);
            }

            EditorGUILayout.Space();
        }

        private void DrawInputSection() {
            EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_inputConfig);

            if (_inputConfig.objectReferenceValue == null) {
                EditorGUILayout.HelpBox("Input Config is required for player control.", MessageType.Warning);
            }
            else if (!(_inputConfig.objectReferenceValue is ICharacterInputConfig)) {
                EditorGUILayout.HelpBox("Input Config must implement ICharacterInputConfig!", MessageType.Error);
            }

            EditorGUILayout.Space();
        }

        private void DrawModifiersSection() {
            EditorGUILayout.LabelField("Modifiers", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_modifierConfigs);

            bool hasInvalidModifier = false;
            bool hasNullModifier = false;

            for (int i = 0; i < _modifierConfigs.arraySize; i++) {
                SerializedProperty element = _modifierConfigs.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == null) {
                    hasNullModifier = true;
                }
                else if (!(element.objectReferenceValue is IModifierConfig)) {
                    hasInvalidModifier = true;
                }
            }

            if (hasNullModifier) {
                EditorGUILayout.HelpBox("Modifier list contains null entries.", MessageType.Warning);
            }

            if (hasInvalidModifier) {
                EditorGUILayout.HelpBox("All modifiers must implement IModifierConfig!", MessageType.Error);
            }

            if (_modifierConfigs.arraySize == 0) {
                EditorGUILayout.HelpBox("No modifiers assigned. Controller will only apply gravity.", MessageType.Info);
            }

            EditorGUILayout.Space();
        }

        private void DrawReferencesSection() {
            EditorGUILayout.LabelField("References", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_playerVisuals);
            EditorGUILayout.PropertyField(_cameraHolder);
            EditorGUILayout.PropertyField(_groundCheckOrigin);

            if (_cameraHolder.objectReferenceValue is Transform cameraHolder) {
                if (cameraHolder.GetComponentInChildren<Camera>() == null) {
                    EditorGUILayout.HelpBox("CameraHolder has no Camera component attached!", MessageType.Error);
                }
            }
            else {
                EditorGUILayout.HelpBox("CameraHolder is required for camera rotation.", MessageType.Warning);
            }

            if (_groundCheckOrigin.objectReferenceValue == null) {
                EditorGUILayout.HelpBox("GroundCheckOrigin not set. Will use transform.position for ground checks.", MessageType.Info);
            }

            EditorGUILayout.Space();
        }

        private void DrawRuntimeInfo(FirstPersonController controller) {
            if (!Application.isPlaying) {
                return;
            }

            EditorGUILayout.LabelField("Runtime Info", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Is Grounded", controller.IsGrounded);
            EditorGUILayout.Vector3Field("Velocity", controller.Velocity);
            EditorGUILayout.FloatField("Speed", controller.VelocityMagnitude);
            EditorGUILayout.Vector3Field("Rotation", controller.GetRotation());
            EditorGUILayout.FloatField("Time Since Grounded", controller.GetTimeSinceGrounded());
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Reset Velocity")) {
                controller.ResetVelocity();
            }

            Repaint();
        }
    }
}