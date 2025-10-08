Shader "Hidden/Color Blindness"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _RedChannel("Red Channel", Color) = (1, 0, 0, 0)
        _GreenChannel("Green Channel", Color) = (0, 1, 0, 0)
        _BlueChannel("Blue Channel", Color) = (0, 0, 1, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off Cull Off
        Pass
        {
            Name "Color Blindness Pass"
HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            // Set the color texture from the camera as the input texture
            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            half3 _RedChannel;
            half3 _GreenChannel;
            half3 _BlueChannel;

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 c = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, input.texcoord);
                return half4
                (
                    c.r * _RedChannel.r + c.g * _RedChannel.g + c.b * _RedChannel.b,
                    c.r * _GreenChannel.r + c.g * _GreenChannel.g + c.b * _GreenChannel.b,
                    c.r * _BlueChannel.r + c.g * _BlueChannel.g + c.b * _BlueChannel.b,
                    c.a
                );
            }
ENDHLSL
        }
    }
}
