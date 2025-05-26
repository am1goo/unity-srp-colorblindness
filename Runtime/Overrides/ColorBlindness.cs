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
        Protanomaly     = 2,
        Deuteranopia    = 3,
        Deuteranomaly   = 4,
        Tritanopia      = 5,
        Tritanomaly     = 6,
        Achromatopsia   = 7,
        Achromatomaly   = 8,
    }

    [Serializable]
    public class ColorBlindnessChannels
    {
        public Vector3 red;
        public Vector3 green;
        public Vector3 blue;

        public ColorBlindnessChannels(Vector3 red, Vector3 green, Vector3 blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
    }
}
