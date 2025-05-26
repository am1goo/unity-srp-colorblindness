using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[DisallowMultipleRendererFeature("Color Blindness")]
public class ColorBlindnessRendererFeature : ScriptableRendererFeature
{
    [SerializeField]
    [Reload("Shaders/ColorBlindness.shader")]
    private Shader m_Shader;
    private Material m_Material;

    private static readonly Dictionary<ColorBlindnessMode, ColorBlindnessChannels> m_Settings = new Dictionary<ColorBlindnessMode, ColorBlindnessChannels>
    {
        { ColorBlindnessMode.Protanopia,    new ColorBlindnessChannels(Color(56.667f, 43.333f, 0), Color(55.833f, 44.167f, 0), Color(0, 24.167f, 75.833f)) },
        { ColorBlindnessMode.Protanomaly,   new ColorBlindnessChannels(Color(81.667f, 18.333f, 0), Color(33.333f, 66.667f, 0), Color(0, 12.5f, 87.5f)) },
        { ColorBlindnessMode.Deuteranopia,  new ColorBlindnessChannels(Color(62.5f, 37.5f, 0), Color(70, 30, 0), Color(0, 30, 70)) },
        { ColorBlindnessMode.Deuteranomaly, new ColorBlindnessChannels(Color(80, 20, 0), Color(25.833f, 74.167f, 0), Color(0, 14.167f, 85.833f)) },
        { ColorBlindnessMode.Tritanopia,    new ColorBlindnessChannels(Color(95, 5, 0), Color(0, 43.333f, 56.667f), Color(0, 47.5f, 52.5f)) },
        { ColorBlindnessMode.Tritanomaly,   new ColorBlindnessChannels(Color(96.667f, 3.333f, 0), Color(0, 73.333f, 26.667f), Color(0, 18.333f, 81.667f)) },
        { ColorBlindnessMode.Achromatopsia, new ColorBlindnessChannels(Color(29.9f, 58.7f, 11.4f), Color(29.9f, 58.7f, 11.4f), Color(29.9f, 58.7f, 11.4f)) },
        { ColorBlindnessMode.Achromatomaly, new ColorBlindnessChannels(Color(61.8f, 32, 6.2f), Color(16.3f, 77.5f, 6.2f), Color(16.3f, 32.0f, 51.6f)) },
    };

    private const float _percentInv = 1f / 100f;
    private static Vector3 Color(float r, float g, float b)
    {
        return new Vector3(r * _percentInv, g * _percentInv, b * _percentInv);
    }

    private ColorBlindnessRenderPass m_ColorBlindnessPass;

    private void Awake()
    {
#if UNITY_EDITOR
        ResourceReloader.ReloadAllNullIn(this, ColorBlindnessPackage.packagePath);
#endif
    }

    public override void Create()
    {
        if (m_Shader == null)
        {
            Debug.LogError($"Create: {nameof(m_Shader)} is not defined");
        }
        else
        {
            m_Material = new Material(m_Shader);
        }

        m_ColorBlindnessPass = new ColorBlindnessRenderPass(m_Material, m_Settings);
        m_ColorBlindnessPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_ColorBlindnessPass == null)
            return;

        ref var cameraData = ref renderingData.cameraData;
        if (cameraData.camera.cameraType != CameraType.Game)
            return;

        if (cameraData.isPreviewCamera || cameraData.isSceneViewCamera)
            return;

        renderer.EnqueuePass(m_ColorBlindnessPass);
    }

    protected override void Dispose(bool disposing)
    {
        if (m_ColorBlindnessPass != null)
        {
            m_ColorBlindnessPass.Dispose();
            m_ColorBlindnessPass = null;
        }

        if (Application.isPlaying)
        {
            Destroy(m_Material);
        }
        else
        {
            DestroyImmediate(m_Material);
        }
    }
}
