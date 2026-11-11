Shader "UNDER/Portal Underworld"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (0.1, 0.14, 0.19, 1)
        _EmissionStrength("Emission Strength", Range(0, 4)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
        }

        Pass
        {
            Name "Portal Underworld"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZTest LEqual
            ZWrite On

            Stencil
            {
                Ref 1
                ReadMask 1
                WriteMask 1
                Comp Equal
                Pass Keep
            }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _EmissionStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half3 normalWS : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                const half3 lightDirection = normalize(half3(0.35h, 0.80h, -0.25h));
                half diffuse = 0.30h + 0.70h * saturate(dot(normalize(input.normalWS), lightDirection));
                half lighting = lerp(diffuse, 1.0h, saturate(_EmissionStrength));
                half3 color = _BaseColor.rgb * (lighting + _EmissionStrength);
                return half4(color, _BaseColor.a);
            }
            ENDHLSL
        }
    }
}
