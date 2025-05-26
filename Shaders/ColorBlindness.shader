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
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            fixed3 _RedChannel;
            fixed3 _GreenChannel;
            fixed3 _BlueChannel;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                return fixed4
                (
                    c.r * _RedChannel.r + c.g * _RedChannel.g + c.b * _RedChannel.b,
                    c.r * _GreenChannel.r + c.g * _GreenChannel.g + c.b * _GreenChannel.b,
                    c.r * _BlueChannel.r + c.g * _BlueChannel.g + c.b * _BlueChannel.b,
                    c.a
                );
            }
            ENDCG
        }
    }
}
