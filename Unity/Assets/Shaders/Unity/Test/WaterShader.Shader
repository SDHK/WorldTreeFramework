Shader "Unlit/WaterShader"
{
	Properties
    {
        [Header(Base)]
        _WaterColor1("WaterColor1",Color) = (1,1,1,1)
        _WaterColor2("WaterColor2",Color) = (1,1,1,1)
        [PowerSlider(3)]_WaterSpeed("WaterSpeed",Range(0,1)) = 0.1
        _WaterBase("Direction(XY) Atten(Z) Null(W)",Vector) = (1,1,1,1)

        [Header(Foam)]
        _FoamTex("FoamTex",2D) = "white"{}
        _FoamColor("FoamColor",Color) = (1,1,1,1)
        _FoamRange("FoamRange",Range(0,5)) = 1
        _FoamNoise("FoamNoise",Range(0,3)) = 1

        [Header(Distort)]
        _NormalTex("NormalTex",2D) = "white"{}
        [PowerSlider(3)]_Distort("Distort",Range(0,1)) = 0

        [Header(Specular)]
        _SpecularColor("Specular Color",Color) = (1,1,1,1)
        _SpecularBase("Specular : Intensity(X) Smoothness(Y) Distort(Z) Null(W)",Vector) = (0.6,10,0.5,1)

        [Header(Reflection)]
        _ReflectionTex("ReflectionTex",Cube) = "white"{}

        [Header(Caustics)]
        _CausticsTex("CausticsTex",2D) = "white"{}
    }
    SubShader
    {
        Tags
        {
            //告诉引擎，该Shader只用于 URP 渲染管线
            "RenderPipeline"="UniversalPipeline"
            //渲染类型
            "RenderType"="Transparent"
            //渲染队列
            "Queue"="Transparent"
        }
        // Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // Pragmas
            #pragma target 2.0


            // Includes
          

            #define Use_IrisNoise
            #define UniversalShaderLibraryCore
            #include "../IrisEntryUnity.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            

            CBUFFER_START(UnityPerMaterial)
                half4 _WaterColor1;
                half4 _WaterColor2;

                half _WaterSpeed;
                half4 _WaterBase;

                half4 _FoamColor;
                half _FoamRange;
                half _FoamNoise;
                half4 _FoamTex_ST;

                half _Distort;
                half4 _NormalTex_ST;

                half4 _SpecularColor;
                half4 _SpecularBase;

                half4 _CausticsTex_ST;
            CBUFFER_END

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            TEXTURE2D(_FoamTex);
            SAMPLER(sampler_FoamTex);
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            TEXTURE2D(_NormalTex);
            SAMPLER(sampler_NormalTex);
            TEXTURECUBE(_ReflectionTex);
            SAMPLER(sampler_ReflectionTex);
            TEXTURE2D(_CausticsTex);
            SAMPLER(sampler_CausticsTex);
            //struct appdata
            //顶点着色器的输入
            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half3 normalOS : NORMAL;
            };

            //struct v2f
            //片元着色器的输入
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0; //foamUV
                float4 screenPos : TEXCOORD1;
                float3 positionVS : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
                float3 normalWS : TEXCOORD4;
                float4 normalUV : TEXCOORD5;
                float2 timeOffset : TEXCOORD6;
            };

            //v2f vert(Attributes v)
            //顶点着色器
            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                o.positionWS = TransformObjectToWorld(v.positionOS);
                o.positionVS = TransformWorldToView(o.positionWS);
                o.positionCS = TransformWViewToHClip(o.positionVS);

                o.screenPos = ComputeScreenPos(o.positionCS);
                //计算得到泡沫纹理采样需要的顶点世界空间下的坐标值的流动效果
                o.uv += o.positionWS.xz * _FoamTex_ST.xy + _Time.y * _WaterSpeed;
                //计算得到水下扭曲纹理的流动UV
                o.timeOffset = _Time.y * _WaterSpeed * _WaterBase.xy;
                o.normalUV.xy = TRANSFORM_TEX(v.uv, _NormalTex) + o.timeOffset;
                o.normalUV.zw = TRANSFORM_TEX(v.uv, _NormalTex) + o.timeOffset * float2(-1.3, 1.5);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);

                return o;
            }

            //fixed4 frag(v2f i) : SV_TARGET
            //片元着色器
            half4 frag(Varyings i) : SV_TARGET
            {
                //1、水的深度
                //获取屏幕空间下的 UV 坐标
                float2 screenUV = i.positionCS.xy / _ScreenParams.xy;
                half depthTex = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV).x;
                //深度图转化到观察空间下
                float depthScene = LinearEyeDepth(depthTex, _ZBufferParams);
                //获取水面模型顶点在观察空间下的Z值（可以在顶点着色器中，对其直接进行转化得到顶点观察空间下的坐标）
                float4 depthWater = depthScene + i.positionVS.z;

                //2、水的颜色，线性插值得到水 和 接触物体的水的 颜色的过度
                half4 waterColor = lerp(_WaterColor1, _WaterColor2, depthWater * _WaterBase.z);


                //3、水面泡沫
                //对泡沫纹理进行采样(这里使用顶点世界空间下的坐标进行纹理采样，防止水体缩放影响泡沫的平铺和重复方式)
                half4 foamTex = SAMPLE_TEXTURE2D(_FoamTex, sampler_FoamTex, i.uv.xy);


                foamTex.rgb +=  NoiseVoronoi(i.uv*100);//!!!

                foamTex = pow(abs(foamTex), _FoamNoise);

                //这里增加一个调整深度图范围的功能
                half4 foamRange = depthWater * _FoamRange;

                //使用泡沫纹理 和 泡沫范围 比较得到泡沫遮罩
                half4 foamMask = step(foamRange, foamTex);

                //给泡沫加上颜色
                half4 foamColor = foamMask * _FoamColor;

                //4、水下的扭曲
                half4 normalTex1 = SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, i.normalUV.xy);
                half4 normalTex2 = SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, i.normalUV.zw);
                half4 normalTex = normalTex1 * normalTex2;
                //float2 distortUV = lerp(screenUV,normalTex.xy,_Distort);
                float2 distortUV = screenUV * _Distort + normalTex.xy;
                half depthDistortTex = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, distortUV).x;
                half depthDistortScene = LinearEyeDepth(depthDistortTex, _ZBufferParams);
                half depthDistortWater = depthDistortScene + i.positionVS.z;
                float2 opaqueUV = distortUV;
                if (depthDistortWater < 0) opaqueUV = screenUV;
                half4 opaqueTex = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, opaqueUV);
                
                //5、水的高光
                //Specular = SpecularColor * Ks * pow(max(0,dot(N,H)), Shininess)
                Light light = GetMainLight();
                half3 L = light.direction;
                half3 V = normalize(_WorldSpaceCameraPos.xyz - i.positionWS.xyz);
                //修改法线实现，波光粼粼的效果
                half4 N = lerp(half4(i.normalWS, 1), normalize(normalTex), _SpecularBase.z);
                half3 H = normalize(L + V);
                half4 specular = _SpecularColor * _SpecularBase.x * pow(max(0, dot(N.xyz, H)), _SpecularBase.y);

                //6、水的反射
                half3 normalReflection = lerp(half4(i.normalWS, 1), normalize(normalTex), _Distort).xyz;
                half3 reflectionUV = reflect(-V, normalReflection.xyz);
                half4 reflectionTex = SAMPLE_TEXTURECUBE(_ReflectionTex, sampler_ReflectionTex, reflectionUV);
                half fresnel = 1 - saturate(dot(normalReflection, V));
                half4 reflection = reflectionTex * fresnel;

                //7、水的焦散
                float4 depthVS = 1;
                depthVS.xy = i.positionVS.xy * depthDistortScene / -i.positionVS.z;
                depthVS.z = depthDistortScene;
                float4 depthWS = mul(unity_CameraToWorld, depthVS);
                float4 depthOS = mul(unity_WorldToObject, depthWS);

                float2 uv1 = depthOS.xz * _CausticsTex_ST.xy + depthWS.y * 0.1 + i.timeOffset;
                float2 uv2 = depthOS.xz * _CausticsTex_ST.xy + depthWS.y * 0.1 + i.timeOffset * float2(-1.3, 1.5);

                half4 causticsTex1 = SAMPLE_TEXTURE2D(_CausticsTex, sampler_CausticsTex, uv1);
                half4 causticsTex2 = SAMPLE_TEXTURE2D(_CausticsTex, sampler_CausticsTex, uv2);
                half4 causticsTex = min(causticsTex1, causticsTex2);

                half4 col = (foamColor + waterColor) * opaqueTex + (specular * reflection) + causticsTex;

                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"


}