using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenuForRenderPipeline("Post-processing (am1goo)/Color Blindness", typeof(UniversalRenderPipeline))]
    public sealed partial class ColorBlindness : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Selected blindness mode.")]
        public ColorBlindnessParameter blindnessMode = new ColorBlindnessParameter(ColorBlindnessMode.Normal);

        public bool IsActive()
        {
            return blindnessMode.overrideState && blindnessMode.value != ColorBlindnessMode.Normal;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }

    [Serializable]
    public class ColorBlindnessParameter : VolumeParameter<ColorBlindnessMode>
    {
        public ColorBlindnessParameter(ColorBlindnessMode value, bool overrideState = false) : base(value, overrideState)
        {
        }
    }

    public enum ColorBlindnessMode
    {
        Normal          = 0,
        Protanopia      = 1,
        Deuteranopia    = 2,
        Tritanopia      = 3,
        Achromatopsia   = 4,
    }
}
