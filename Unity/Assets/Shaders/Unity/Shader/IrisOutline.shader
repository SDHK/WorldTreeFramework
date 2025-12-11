Shader "Iris/IrisOutline"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Pass 1 Color", Color) = (1, 0, 0, 1)  // 红色
        _Scale ("Outline Scale", Float) = 0.1
    }

    SubShader
    {
        Tags 
        { 
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
            Tags { "LightMode" = "SRPDefaultUnlit" } 
             // 只渲染背面
            Cull Front 
            HLSLPROGRAM
            #include "IrisOutlineDefaultPass.hlsl"
            ENDHLSL
        }


        // ========== 第二个Pass：正常渲染 ==========
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "UniversalForward" }
            Cull Back

            ZWrite On
            ZTest LEqual 
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #include "IrisOutlineForwardPass.hlsl"
            ENDHLSL
        }
    }
    // Fallback "Universal Render Pipeline/Lit"
}