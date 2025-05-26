using UnityEngine;
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
        }
    }
}
