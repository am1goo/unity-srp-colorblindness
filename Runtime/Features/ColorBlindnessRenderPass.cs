using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorBlindnessRenderPass : ScriptableRenderPass, IDisposable
{
    private bool _isDisposed;
    public bool isDisposed => _isDisposed;

    private Material _protanopiaMaterial;
    private Material _deuteranopiaMaterial;
    private Material _tritanopiaMaterial;
    private Material _achromatopsiaMaterial;
    private Material _currentMaterial;

    private RenderTargetIdentifier _source;

    private static readonly string _profilerTag = "ColorBlindness";
    private static readonly ProfilingSampler _profilingSampler = new ProfilingSampler(_profilerTag);

    public ColorBlindnessRenderPass(Material protanopiaMaterial, Material deuteranopiaMaterial, Material tritanopiaMaterial, Material achromatopsiaMaterial)
    {
        _protanopiaMaterial = protanopiaMaterial;
        _deuteranopiaMaterial = deuteranopiaMaterial;
        _tritanopiaMaterial = tritanopiaMaterial;
        _achromatopsiaMaterial = achromatopsiaMaterial;
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        OnDispose();
    }

    private void OnDispose()
    {
        _currentMaterial = null;
    }

    private void UpdateSettings()
    {
        var component = VolumeManager.instance.stack.GetComponent<ColorBlindness>();
        if (component == null || component.IsActive() == false)
        {
            _currentMaterial = null;
            return;
        }

        var mode = component.blindnessMode.overrideState ? component.blindnessMode.value : ColorBlindnessMode.Normal;
        _currentMaterial = GetMaterial(mode);
    }

    private Material GetMaterial(ColorBlindnessMode mode)
    {
        switch (mode)
        {
            case ColorBlindnessMode.Protanopia:
                return _protanopiaMaterial;

            case ColorBlindnessMode.Deuteranopia:
                return _deuteranopiaMaterial;

            case ColorBlindnessMode.Tritanopia:
                return _tritanopiaMaterial;

            case ColorBlindnessMode.Achromatopsia:
                return _achromatopsiaMaterial;

            case ColorBlindnessMode.Normal:
            default:
                return null;
        }
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        base.OnCameraSetup(cmd, ref renderingData);

        _source = renderingData.cameraData.renderer.cameraColorTargetHandle;
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        base.OnCameraCleanup(cmd);

        _source = default;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.postProcessingEnabled == false)
            return;

        UpdateSettings();

        if (_currentMaterial == null)
            return;

        ref var cameraData = ref renderingData.cameraData;

        var cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, _profilingSampler))
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Blit(cmd, _source, _source, _currentMaterial);
#pragma warning restore CS0618 // Type or member is obsolete
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
        CommandBufferPool.Release(cmd);
    }
}
