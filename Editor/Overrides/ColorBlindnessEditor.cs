using System.Reflection;
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

            var rendererData = GetRendererData(urpAsset);
            if (rendererData == null)
            {
                EditorGUILayout.HelpBox("Render data isn't found", MessageType.Error);
                return;
            }

            if (rendererData.useNativeRenderPass == false)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("'Native RenderPass' should be enabled", MessageType.Error);
                if (GUILayout.Button("Fix", GUILayout.Width(30)))
                {
                    rendererData.useNativeRenderPass = true;
                    EditorUtility.SetDirty(urpAsset);
                }
                EditorGUILayout.EndHorizontal();
                return;
            }
        }

        private static PropertyInfo _pi;
        private static ScriptableRendererData GetRendererData(UniversalRenderPipelineAsset asset)
        {
            if (_pi == null)
                _pi = asset.GetType().GetProperty("scriptableRendererData", BindingFlags.Instance | BindingFlags.NonPublic);
            var obj = _pi.GetValue(asset, null);
            return (ScriptableRendererData)obj;
        }
    }
}
