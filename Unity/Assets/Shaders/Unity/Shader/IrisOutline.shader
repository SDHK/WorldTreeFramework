Shader "Iris/IrisOutline"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color1 ("Pass 1 Color", Color) = (1, 0, 0, 1)  // 红色
        _Scale ("Outline Scale", Float) = 0.1
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
     
        #define IrisShader

        sampler2D _MainTex;
        float4 _MainTex_ST;

        ENDHLSL


        // ========== 第一个Pass：描边（红色） ==========
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "SRPDefaultUnlit" }  // URP必须！
            // Tags {"LightMode" = "ForwardBase"}
            Cull Front  // 只渲染背面
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define As_UniversalShaderLibraryCore
            #define As_IrisMatrix
            #define As_IrisMath
            #define As_IrisVertex

            #define VD_Position
            #define VD_Normal

            #define FD_Position
            #define FD_T0F2

            #include "../IrisEntryUnity.hlsl"
            
            float4 _Color1;
            float _Scale;

            FragData vert(VertData vertData)
            {
                FragData fragData;
                fragData.Position = VertScale(vertData.Position, vertData.Normal, _Scale);
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
            // #pragma multi_compile __ _MAIN_LIGHT_SHADOWS
            // #pragma multi_compile __ _MAIN_LIGHT_SHADOWS_CASCADE
            // #pragma multi_compile __ _SHADOWS_SOFT

            #define As_UniversalShaderLibraryCore
            #define As_IrisMatrix
            #define As_IrisMath
            #define As_IrisVertex

            #define VD_Position
            #define VD_Normal
            #define VD_T0F2

            #define FD_Position
            #define FD_T0F2
            #define FD_T1F3  // 用于传递世界空间法线

            #include "../IrisEntryUnity.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            FragData vert(VertData vertData)
            {
                FragData fragData;
                fragData.Position = VertDefault(vertData.Position);
                fragData.T0F2 = TRANSFORM_TEX(vertData.T0F2, _MainTex);
                
                // 将法线转换到世界空间
                float3 normalWS = TransformObjectToWorldNormal(vertData.Normal);
                fragData.T1F3 = normalWS;
                
                return fragData;
            }

            half4 frag(FragData fragData) : SV_Target
            {
                // 采样纹理
                half4 albedo = tex2D(_MainTex, fragData.T0F2);
                
                // 获取主光源信息
                Light mainLight = GetMainLight();
                
                // 计算简单的 Lambert 光照
                half NdotL = saturate(dot(normalize(fragData.T1F3), mainLight.direction));
                half3 lighting = mainLight.color * NdotL + unity_AmbientSky.rgb;
                
                // 应用光照
                half4 finalColor = albedo;
                finalColor.rgb *= lighting;
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    // Fallback "Universal Render Pipeline/Lit"
}