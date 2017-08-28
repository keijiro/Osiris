using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEditor;

namespace Klak
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BrownianMotionPlayableAsset))]
    class BrownianMotionPlayableInspector : Editor
    {
        SerializedProperty _target;
        SerializedProperty _enablePositionNoise;
        SerializedProperty _enableRotationNoise;
        SerializedProperty _positionFrequency;
        SerializedProperty _rotationFrequency;
        SerializedProperty _positionAmplitude;
        SerializedProperty _rotationAmplitude;
        SerializedProperty _positionFractalLevel;
        SerializedProperty _rotationFractalLevel;
        SerializedProperty _randomSeed;

        static class Styles
        {
            public static readonly GUIContent positionNoise = new GUIContent("Position Noise");
            public static readonly GUIContent rotationNoise = new GUIContent("Rotation Noise");
            public static readonly GUIContent frequency = new GUIContent("Frequency");
            public static readonly GUIContent amplitude = new GUIContent("Amplitude");
            public static readonly GUIContent fractal = new GUIContent("Fractal");
        }

        void OnEnable()
        {
            _target = serializedObject.FindProperty("_target");
            _enablePositionNoise = serializedObject.FindProperty("_enablePositionNoise");
            _enableRotationNoise = serializedObject.FindProperty("_enableRotationNoise");
            _positionFrequency = serializedObject.FindProperty("_positionFrequency");
            _rotationFrequency = serializedObject.FindProperty("_rotationFrequency");
            _positionAmplitude = serializedObject.FindProperty("_positionAmplitude");
            _rotationAmplitude = serializedObject.FindProperty("_rotationAmplitude");
            _positionFractalLevel = serializedObject.FindProperty("_positionFractalLevel");
            _rotationFractalLevel = serializedObject.FindProperty("_rotationFractalLevel");
            _randomSeed = serializedObject.FindProperty("_randomSeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_target);

            EditorGUILayout.PropertyField(_enablePositionNoise, Styles.positionNoise);

            if (_enablePositionNoise.hasMultipleDifferentValues || _enablePositionNoise.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_positionFrequency, Styles.frequency);
                EditorGUILayout.PropertyField(_positionAmplitude, Styles.amplitude);
                EditorGUILayout.PropertyField(_positionFractalLevel, Styles.fractal);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(_enableRotationNoise, Styles.rotationNoise);

            if (_enableRotationNoise.hasMultipleDifferentValues || _enableRotationNoise.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_rotationFrequency, Styles.frequency);
                EditorGUILayout.PropertyField(_rotationAmplitude, Styles.amplitude);
                EditorGUILayout.PropertyField(_rotationFractalLevel, Styles.fractal);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(_randomSeed);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
