using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorBlindnessRenderPass : ScriptableRenderPass, IDisposable
{
    private bool _isDisposed;
    public bool isDisposed => _isDisposed;

    private Material _material;
    private Dictionary<ColorBlindnessMode, ColorBlindnessChannels> _settings;

    private ColorBlindnessChannels _currentChannels;

    private RenderTargetIdentifier _source;

    private const string _profilerTag = "ColorBlindness";
    private static readonly ProfilingSampler _profilingSampler = new ProfilingSampler(_profilerTag);

    private static readonly int _redChannelID = Shader.PropertyToID("_RedChannel");
    private static readonly int _greenChannelID = Shader.PropertyToID("_GreenChannel");
    private static readonly int _blueChannelID = Shader.PropertyToID("_BlueChannel");

    public ColorBlindnessRenderPass(Material material, Dictionary<ColorBlindnessMode, ColorBlindnessChannels> settings)
    {
        _material = material;
        _settings = settings;
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
        _currentChannels = null;
    }

    private void UpdateSettings(Material material)
    {
        var component = VolumeManager.instance.stack.GetComponent<ColorBlindness>();
        if (component == null || component.IsActive() == false)
        {
            _currentChannels = null;
            return;
        }

        var mode = component.blindnessMode.overrideState ? component.blindnessMode.value : ColorBlindnessMode.Normal;
        var channels = GetChannel(mode, defaultValue: null);

        if (_currentChannels != channels)
        {
            _currentChannels = channels;
            if (_currentChannels != null)
            {
                material.SetVector(_redChannelID, _currentChannels.red);
                material.SetVector(_greenChannelID, _currentChannels.green);
                material.SetVector(_blueChannelID, _currentChannels.blue);
            }
        }
    }

    private ColorBlindnessChannels GetChannel(ColorBlindnessMode mode, ColorBlindnessChannels defaultValue)
    {
        if (_settings == null)
            return defaultValue;

        if (_settings.TryGetValue(mode, out var channels))
        {
            return channels;
        }
        else
        {
            return defaultValue;
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

        if (_material == null)
            return;

        UpdateSettings(_material);

        if (_currentChannels == null)
            return;

        ref var cameraData = ref renderingData.cameraData;

        var cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, _profilingSampler))
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Blit(cmd, _source, _source, _material);
#pragma warning restore CS0618 // Type or member is obsolete
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
        CommandBufferPool.Release(cmd);
    }
}
