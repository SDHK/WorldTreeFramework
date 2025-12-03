Shader "Unlit/OutlineShader"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color1 ("Pass 1 Color", Color) = (1, 0, 0, 1)  // 红色
        _Offset ("Outline Offset", Float) = 0.1
    }

    SubShader
    {
        Tags 
        { 
            
            // "RenderType" = "Opaque" 
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)

        sampler2D _MainTex;
        float4 _MainTex_ST;
        CBUFFER_END

        ENDHLSL

        // ========== 第一个Pass：描边（红色） ==========
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "SRPDefaultUnlit" }  // URP必须！
            Cull Front  // 只渲染背面
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            
            float4 _Color1;
            float _Offset;
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                // 沿法线方向扩展顶点
                float3 positionOS = input.positionOS.xyz + input.normalOS * _Offset;
                output.positionCS = TransformObjectToHClip(positionOS);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                return _Color1;  // 返回红色
            }
            
            ENDHLSL
        }

        // ========== 第二个Pass：正常渲染 ==========
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "UniversalForward" }
            Cull [_CullMode]
            ZWrite On
            ZTest LEqual 
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            //顶点数据结构
               struct VertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(VertexOutput i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDHLSL
        }
    }
    
    // Fallback "Universal Render Pipeline/Lit"
}