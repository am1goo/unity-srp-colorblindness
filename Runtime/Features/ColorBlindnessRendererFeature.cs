using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[DisallowMultipleRendererFeature("Color Blindness")]
public class ColorBlindnessRendererFeature : ScriptableRendererFeature
{
    [SerializeField, Reload("Shaders/Protanopia.shader")]
    private Shader m_ProtanopiaShader;
    [SerializeField, Reload("Shaders/Deuteranopia.shader")]
    private Shader m_DeuteranopiaShader;
    [SerializeField, Reload("Shaders/Tritanopia.shader")]
    private Shader m_TritanopiaShader;
    [SerializeField, Reload("Shaders/Achromatopsia.shader")]
    private Shader m_AchromatopsiaShader;

    private ColorBlindnessRenderPass m_ColorBlindnessPass;
    private Material m_ProtanopiaMaterial;
    private Material m_DeuteranopiaMaterial;
    private Material m_TritanopiaMaterial;
    private Material m_AchromatopsiaMaterial;

    public override void Create()
    {
        if (m_ProtanopiaShader == null)
        {
            Debug.LogError($"Create: {nameof(m_ProtanopiaShader)} is not defined");
        }
        else
        {
            m_ProtanopiaMaterial = new Material(m_ProtanopiaShader);
        }

        if (m_DeuteranopiaShader == null)
        {
            Debug.LogError($"Create: {nameof(m_DeuteranopiaShader)} is not defined");
        }
        else
        {
            m_DeuteranopiaMaterial = new Material(m_DeuteranopiaShader);
        }

        if (m_TritanopiaShader == null)
        {
            Debug.LogError($"Create: {nameof(m_TritanopiaShader)} is not defined");
        }
        else
        {
            m_TritanopiaMaterial = new Material(m_TritanopiaShader);
        }

        if (m_AchromatopsiaShader == null)
        {
            Debug.LogError($"Create: {nameof(m_AchromatopsiaShader)} is not defined");
        }
        else
        {
            m_AchromatopsiaMaterial = new Material(m_AchromatopsiaShader);
        }

        m_ColorBlindnessPass = new ColorBlindnessRenderPass(m_ProtanopiaMaterial, m_DeuteranopiaMaterial, m_TritanopiaMaterial, m_AchromatopsiaMaterial);
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
            Destroy(m_ProtanopiaMaterial);
            Destroy(m_DeuteranopiaMaterial);
            Destroy(m_TritanopiaMaterial);
        }
        else
        {
            DestroyImmediate(m_ProtanopiaMaterial);
            DestroyImmediate(m_DeuteranopiaMaterial);
            DestroyImmediate(m_TritanopiaMaterial);
        }
    }
}
