using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [CustomEditor(typeof(ColorBlindness))]
    sealed class ColorBlindnessEditor : VolumeComponentEditor
    {
        SerializedDataParameter m_BlindnessMode;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<ColorBlindness>(serializedObject);
            m_BlindnessMode = Unpack(o.Find(x => x.blindnessMode));
        }

        public override void OnInspectorGUI()
        {
            PropertyField(m_BlindnessMode);

            if (GraphicsSettings.renderPipelineAsset is not UniversalRenderPipelineAsset urpAsset)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("No URP pipeline detected", MessageType.Error);
                if (GUILayout.Button("Go to settings", GUILayout.MinWidth(60)))
                {
                    SettingsService.OpenProjectSettings("Project/Graphics");
                }
                EditorGUILayout.EndHorizontal();
                return;
            }

            if (urpAsset.supportsCameraOpaqueTexture == false)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("'Opaque Texture' should be enabled", MessageType.Error);
                if (GUILayout.Button("Fix", GUILayout.Width(30)))
                {
                    urpAsset.supportsCameraOpaqueTexture = true;
                    EditorUtility.SetDirty(urpAsset);
                }
                EditorGUILayout.EndHorizontal();
                return;
            }
        }
    }
}
