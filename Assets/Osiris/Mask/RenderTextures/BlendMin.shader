Shader "Osiris/BlendMin"
{
    Properties
    {
        _MainTex("Main", 2D) = "white"{}
        _BlendTex("Blend", 2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "UnityCG.cginc"

            struct Attributes
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _BlendTex;

            float4 _MainTex_ST;
            float4 _BlendTex_ST;

            Varyings Vertex(Attributes v)
            {
                Varyings o;
                o.position = UnityObjectToClipPos(v.position);
                o.uv0 = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv1 = TRANSFORM_TEX(v.uv, _BlendTex);
                return o;
            }

            fixed4 Fragment(Varyings i) : SV_Target
            {
                fixed4 c1 = tex2D(_MainTex, i.uv0);
                fixed4 c2 = tex2D(_BlendTex, i.uv1);
                return min(c1, c2);
            }

            ENDCG
        }
    }
}
