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


        #define As_UniversalShaderLibraryCore
        #define As_IrisMatrix
        #define As_IrisMath

        #include "../IrisEntryUnity.hlsl"

        // CBUFFER_START(UnityPerMaterial)

        sampler2D _MainTex;
        float4 _MainTex_ST;
        // CBUFFER_END

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
            
            struct VertData 
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };
            
            struct FragData
            {
                float4  SV_position : SV_POSITION;
            };
            
            FragData vert(VertData vertData)
            {
                FragData fragData;
                // 沿法线方向扩展顶点
                float3 position = Scale(vertData.position.xyz, vertData.normal, _Offset);
                // 手动计算裁剪空间位置
                fragData.SV_position =  WorldToCamera(position);
                return fragData;
            }
            
            half4 frag(FragData fragData) : SV_Target
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
            struct VertData
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0; // 第一个纹理坐标
            };

            //片元数据结构
            struct FragData
            {
                float4 sv_pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            FragData vert(VertData vertData)
            {
                FragData fragData;
                fragData.sv_pos =  mul(Iris_Matrix_MVP, vertData.position);
                fragData.uv = TRANSFORM_TEX(vertData.uv, _MainTex);
                return fragData;
            }

            half4 frag(FragData fragData) : SV_Target
            {
                return tex2D(_MainTex, fragData.uv);
            }
            ENDHLSL
        }
    }
    
    // Fallback "Universal Render Pipeline/Lit"
}