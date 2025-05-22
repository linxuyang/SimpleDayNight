Shader "Marko/DayNight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DayTime ("DayTime", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent"
        }
        ZTest Always Cull Off ZWrite Off
        Blend Zero SrcColor

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        half4 _MainTex_ST;
        half _DayTime;

        struct a2v
        {
            uint vertexID :SV_VertexID;
        };

        struct v2f
        {
            float4 posCS:SV_POSITION;
            half2 uv:TEXCOORD0;
        };

        v2f Vert(a2v v)
        {
            v2f o;
            o.posCS = GetFullScreenTriangleVertexPosition(v.vertexID);
            o.uv = GetFullScreenTriangleTexCoord(v.vertexID);
            return o;
        }

        half4 Frag(v2f i):SV_Target
        {
            half4 timeCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, half2(_DayTime,0.5));
            return timeCol;
        }
        ENDHLSL

        Pass
        {
            Tags
            {
                "RenderPipeline" = "UniversalPipeline"
            }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }

    }
}