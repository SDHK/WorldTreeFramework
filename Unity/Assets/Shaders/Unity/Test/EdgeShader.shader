
Shader "Unlit/EdgeShader"
{
    Properties

    {

        _MainTex ("MainTex", 2D) = "white" {}

        _RimColor ("Rim Color", Color) = (0.0, 0.7, 0.7,1)

        _RimMin ("Rim Min", Range(0, 1)) = 0.6

        _RimMax ("Rim Max", Range(0, 1)) = 0.6

        _RimSmooth ("Rim Smooth", Range(0, 1)) = 0.6
    }



    SubShader
    {
        Tags 
        {
            // "LightMode" ="UniversalForward"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
           
            CGPROGRAM

            #pragma vertex vert //顶点着色器
            #pragma fragment frag //片元着色器
        	// #pragma surface surf Lambert 	//表面着色器

            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"

            #include "Lighting.cginc"

            #include "AutoLight.cginc"

            #define Use_IrisNoise
            #define Use_IrisMath
            #include "../IrisEntryUnity.hlsl"


            sampler2D _MainTex;

            float4 _MainTex_ST;


            half4 _RimColor;

            half _RimMin;

            half _RimMax;

            half _RimSmooth;



            struct a2v
            {

                float4 vertex : POSITION;

                float2 uv : TEXCOORD0;

                float3 normal : NORMAL;
            };


            struct v2f
            {

                float4 pos : SV_POSITION;//表示顶点的屏幕空间位置，通常用于顶点着色器的输出。 

                float2 uv : TEXCOORD0;

                float3 worldNormal : TEXCOORD1;

                float3 worldPos : TEXCOORD2;
            };

            v2f vert (a2v v) //顶点着色器，控制定点位置
            {
                v2f o = (v2f)0;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);//纹理设置,v.uv应用_MainTex的变换 赋值给 o.uv

                float a=  NoiseFBMvalue(o.uv*10);

                o.pos = UnityObjectToClipPos(v.vertex+v.vertex *a);//顶点设置

                //局部转世界，将对象空间中的法线向量转换为世界空间中的法线向量。
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                //局部转世界，矩阵和向量的乘法运算,将顶点位置从对象空间转换到世界空间
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; 

                return o;
            }
            //SV_Target
            half4 frag (v2f i) : SV_Target //片元着色器，控制像素颜色
            {
                //half 其实是16位的float

               float a=  NoiseFBMvalue(i.uv*10);

                half4 col = 0;
                //_WorldSpaceCameraPos 是 Unity 中的内置变量，表示摄像机的世界空间位置。
                //normalize是归一化
                //这里获取了worldPos指向_WorldSpaceCameraPos的向量，然后对其进行归一化。
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

                //法线向量归一化
                half3 worldNormal = normalize(i.worldNormal);

                //dot计算的是 视线方向*法线方向的点积，夹角越小值越大，完全一致则为1
                //saturate 函数的作用是将输入值限制在 [0, 1] 的范围内。
                half f = saturate(dot(viewDir, worldNormal));
                
                //为了让边缘显示，这里取反，让夹角大的变成1，视线完全一致则变成0
                f = 1.0 - f;
                
                //smoothstep 的作用是平滑，参数是（min，max，value）
                half rim = smoothstep(_RimMin, _RimMax, f);

                //再次平滑，最小为0
                rim = smoothstep(0, _RimSmooth, rim);

                //这里a被当成了颜色的明暗
                half3 rimColor = rim * _RimColor.rgb *  _RimColor.a;

                //获取纹理颜色
                half4 col2 = tex2D(_MainTex, i.uv);
                //_LightColor0 是 Unity 中的一个内置变量，表示第一个光源的颜色。它通常用于前向渲染路径中的光照计算。
                col.rgb =saturate( rimColor * _LightColor0.rgb  + col2*a);
                col.a = a;
                // col.a = 0.5 + 0.5 * sin(_Time.w);

                return col;

                Vector _PerlinController = (1, 1, 10,10);
                Vector _PerlinInt = (1, 1, 1, 1);
            }

            ENDCG
        }

         Pass
        {
           
            CGPROGRAM

            #pragma vertex vert //顶点着色器
            #pragma fragment frag //片元着色器
        	// #pragma surface surf Lambert 	//表面着色器

            #pragma multi_compile_fwdbase


            #include "Lighting.cginc"

            #include "AutoLight.cginc"

            #define Use_IrisNoise
            #include "../IrisEntryUnity.hlsl"


            sampler2D _MainTex;

            float4 _MainTex_ST;


            half4 _RimColor;

            half _RimMin;

            half _RimMax;

            half _RimSmooth;



            struct a2v
            {

                float4 vertex : POSITION;

                float2 uv : TEXCOORD0;

                float3 normal : NORMAL;
            };


            struct v2f
            {

                float4 pos : SV_POSITION;//表示顶点的屏幕空间位置，通常用于顶点着色器的输出。 

                float2 uv : TEXCOORD0;

                float3 worldNormal : TEXCOORD1;

                float3 worldPos : TEXCOORD2;
            };

            v2f vert (a2v v) //顶点着色器，控制定点位置
            {
                v2f o = (v2f)0;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);//纹理设置,v.uv应用_MainTex的变换 赋值给 o.uv

                float a=  NoiseFBMvalue(o.uv*_Time);

                o.pos = UnityObjectToClipPos(v.vertex+v.vertex *a);//顶点设置

                //局部转世界，将对象空间中的法线向量转换为世界空间中的法线向量。
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                //局部转世界，矩阵和向量的乘法运算,将顶点位置从对象空间转换到世界空间
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; 

                return o;
            }
            //SV_Target
            half4 frag (v2f i) : SV_Target //片元着色器，控制像素颜色
            {
                //half 其实是16位的float

               float a=  NoiseFBMvalue(i.uv*10);

                half4 col = 0;
                //_WorldSpaceCameraPos 是 Unity 中的内置变量，表示摄像机的世界空间位置。
                //normalize是归一化
                //这里获取了worldPos指向_WorldSpaceCameraPos的向量，然后对其进行归一化。
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

                //法线向量归一化
                half3 worldNormal = normalize(i.worldNormal);

                //dot计算的是 视线方向*法线方向的点积，夹角越小值越大，完全一致则为1
                //saturate 函数的作用是将输入值限制在 [0, 1] 的范围内。
                half f = saturate(dot(viewDir, worldNormal));
                
                //为了让边缘显示，这里取反，让夹角大的变成1，视线完全一致则变成0
                f = 1.0 - f;
                
                //smoothstep 的作用是平滑，参数是（min，max，value）
                half rim = smoothstep(_RimMin, _RimMax, f);

                //再次平滑，最小为0
                rim = smoothstep(0, _RimSmooth, rim);

                //这里a被当成了颜色的明暗
                half3 rimColor = rim * _RimColor.rgb *  _RimColor.a;

                //获取纹理颜色
                half4 col2 = tex2D(_MainTex, i.uv);
                //_LightColor0 是 Unity 中的一个内置变量，表示第一个光源的颜色。它通常用于前向渲染路径中的光照计算。
                col.rgb =saturate( rimColor * _LightColor0.rgb  + col2*a);
                col.a = a;
                // col.a = 0.5 + 0.5 * sin(_Time.w);

                return col;

                Vector _PerlinController = (1, 1, 10,10);
                Vector _PerlinInt = (1, 1, 1, 1);
            }

            ENDCG
        }
    }
    FallBack Off
}
