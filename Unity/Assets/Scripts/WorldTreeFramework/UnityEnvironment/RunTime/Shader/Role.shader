Shader "Custom/URP/Cartoon/Role"
{
    Properties
    {
        // 卡通渲染属性
        _FinalColor("FinalColor", Color) = (1, 1, 1, 1)
        _MainTex ("Main Texture", 2D) = "white" {}
        [HDR]_ColorTint ("Base Color", Color) = (1,1,1,1)
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
        
        // 描边参数
        _OutlineWidth ("Outline Width", Range(0,10)) = 0.5
        [HDR] _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        
        // 阴影参数
        _ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
        _ShadowRange ("Shadow Range", Range(0,1)) = 0.5
        _ShadowSmooth ("Shadow Smooth", Range(0,0.2)) = 0.01
        
        // 纹理动画参数
        [Toggle(ENABLE_ANIM)] _EnableAnim ("Enable Anim", Float) = 1
        _PosTex("Position Texture", 2D) = "black"{}
        _DT("Delta Time", float) = 0
        _Length("Animation Length", Float) = 1
        _Speed("Animation Speed", Range(0, 10)) = 1
        [Toggle(ANIM_LOOP)] _Loop("Loop Animation", Float) = 0
        [Toggle(ANIM_PAUSED)] _Paused("Pause Animation", Float) = 0
        _FrozenTime("Frozen Time", Range(0, 1)) = 0 // 暂停时显示的特定帧
    }
    
    SubShader
    {
        Tags { 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
        }
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        
        // 共享材质属性
        CBUFFER_START(UnityPerMaterial)
        float4 _FinalColor;
        float4 _ColorTint;
        float4 _OutlineColor;
        float4 _ShadowColor;
        sampler2D _MainTex;
        float4 _MainTex_ST;
        float _OutlineWidth;
        float _ShadowRange;
        float _ShadowSmooth;
		float _CullMode;
        
        // 动画参数
        sampler2D _PosTex;
        float4 _PosTex_TexelSize;
        float _Length;
        float _DT;
        float _Speed; // 速度变量
        float _FrozenTime; // 冻结时间变量
        CBUFFER_END
        
        // 共享函数：计算动画时间
        float GetAnimationTime()
        { 
            #if ANIM_PAUSED
                // 暂停时使用固定的_FrozenTime值
                return _FrozenTime;
            #else
                // 正常播放时使用带速度的时间计算
                float t = (_Time.y * _Speed - _DT) / _Length;
                #if ANIM_LOOP
                    t = frac(t);
                #else
                    t = saturate(t);
                #endif
                return t;
            #endif
        }
        
        // 共享函数：采样动画纹理
        float3 SampleAnimatedPosition(uint vertexID, float t)
        {
            float x = (vertexID + 0.5) * _PosTex_TexelSize.x;
            float4 animatedPos = tex2Dlod(_PosTex, float4(x, t, 0, 0));
            return animatedPos.xyz;
        }
        ENDHLSL

        Pass
        {
            Name "OUTLINE"

            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // 保留动画宏开关
            #pragma shader_feature _ ENABLE_ANIM
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile ___ ANIM_LOOP
            #pragma multi_compile ___ ANIM_PAUSED
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                uint vertexID : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                // 获取顶点位置 - 根据宏选择实现
                #if ENABLE_ANIM
                    float t = GetAnimationTime();
                    float3 positionOS = SampleAnimatedPosition(input.vertexID, t);
                #else
                    float3 positionOS = input.positionOS.xyz;
                #endif
                
                // 描边计算
                float3 extrudeDir = normalize(input.normalOS) * (_OutlineWidth * 0.01);
                float3 positionWS = TransformObjectToWorld(positionOS + extrudeDir);
                output.positionCS = TransformWorldToHClip(positionWS);
                
                // Z轴偏移优化
                #if defined(UNITY_REVERSED_Z)
                    output.positionCS.z -= 0.005;
                #else
                    output.positionCS.z += 0.005;
                #endif
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                return _OutlineColor * _FinalColor;
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "UniversalForward" }
            Cull [_CullMode]
            Blend SrcAlpha OneMinusSrcAlpha
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // 保留动画宏开关
            #pragma shader_feature _ ENABLE_ANIM
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile ___ ANIM_LOOP
            #pragma multi_compile ___ ANIM_PAUSED

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                uint vertexID : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                // 获取顶点位置 - 根据宏选择实现
                #if ENABLE_ANIM
                    float t = GetAnimationTime();
                    float3 positionOS = SampleAnimatedPosition(input.vertexID, t);
                #else
                    float3 positionOS = input.positionOS.xyz;
                #endif
                
                // 顶点变换
                VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                
                // 基础数据
                float3 normalWS = normalize(input.normalWS);
                half4 baseColor = tex2D(_MainTex, input.uv) * _ColorTint;
                
                // 主光源数据
                Light mainLight = GetMainLight();
                float NdotL = dot(normalWS, mainLight.direction);
                float halfLambert = saturate(NdotL * 0.5 + 0.5);
                float shadowAtten = mainLight.shadowAttenuation;
                
                // 自阴影处理
                float smoothRange = _ShadowSmooth;
                float shadowStep = smoothstep(
                    _ShadowRange - smoothRange, 
                    _ShadowRange + smoothRange, 
                    halfLambert
                );
                
                // 光照混合
                half3 litColor = baseColor.rgb * mainLight.color;
                half3 shadow = litColor * _ShadowColor.rgb;
                half3 directLight = lerp(shadow, litColor, shadowStep) * shadowAtten;
                
                // 环境光
                half3 ambient = SampleSH(normalWS) * 0.5h * baseColor.rgb;
                half3 finalColor = directLight + ambient;
                
                return half4(finalColor * _FinalColor.rgb, baseColor.a);
            }
            ENDHLSL
        }
    }
    
    Fallback "Universal Render Pipeline/Simple Lit"
    CustomEditor "CartoonRoleShaderGUI"
}