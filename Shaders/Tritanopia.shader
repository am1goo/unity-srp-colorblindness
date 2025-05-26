Shader "Hidden/Tritanopia"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
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

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 _R = fixed4(0.95, 0.05, 0, 1);
                fixed4 _G = fixed4(0, 0.43333, 0.56667, 1);
                fixed4 _B = fixed4(0, 0.475, 0.525, 1);

                fixed4 c = tex2D(_MainTex, i.uv);
                return fixed4
                (
                    c.r * _R.r + c.g * _R.g + c.b * _R.b,
                    c.r * _G.r + c.g * _G.g + c.b * _G.b,
                    c.r * _B.r + c.g * _B.g + c.b * _B.b,
                    c.a
                );
            }
            ENDCG
        }
    }
}
