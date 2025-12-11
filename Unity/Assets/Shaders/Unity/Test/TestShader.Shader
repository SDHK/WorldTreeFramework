Shader "Unlit/TestShader"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _RimColor ("Rim Color", Color) = (0.0, 0.7, 0.7,1)
        _Value1 ("Value1", Range(-5, 5)) = 0.6
        _Angle ("Angle", float) =0
        _Color0 ("Color 0", Color) = (0.101961, 0.619608, 0.666667, 1)
        _Color1 ("Color 1", Color) = (0.101961, 0.619608, 0.666667, 1)
        _Color2 ("Color 2", Color) = (0.666667, 0.666667, 0.498039, 1)
        _Color3 ("Color 3", Color) = (0.0, 0.0, 0.164706, 1)
        _Color4 ("Color 4", Color) = (0.666667, 1.0, 1.0, 1)
        _Color5 ("Color 5", Color) = (0.666667, 1.0, 1.0, 1)
        _Color6 ("Color 6", Color) = (0.666667, 1.0, 1.0, 1)
        _Color7 ("Color 7", Color) = (0.666667, 1.0, 1.0, 1)
    }

    //后处理
    //描边
    //影子
    //网格生成
    //融球
    //放大镜

    //物体阴影
    //镜面反射
    //凹陷

    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


            #define Use_IrisMath
            #define Use_IrisNoise
            #include "../IrisEntryUnity.hlsl"


            //顶点数据结构
            struct Data
            {
                float4 vertexPos : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 worldNormal : TEXCOORD1;
            };


            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Value;
            float _Angle;

            float4 _Color0;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float4 _Color4;
            float4 _Color5;
            float4 _Color6;
            float4 _Color7;

            float4 colors1[8];


            //云雾效果测试
            fixed4 fragTest1(float2 uv, float time)
            {
                colors1[0] =  _Color0;
                colors1[1] =  _Color1;
                colors1[2] =  _Color2;
                colors1[3] =  _Color3;
                colors1[4] =  _Color4;
                colors1[5] =  _Color5;
                colors1[6] =  _Color6;
                colors1[7] =  _Color7;
                //角度转向量
                float2 dir = float2(cos(_Angle), sin(_Angle));
                //拿到色彩权重
                float colorWeight = NoiseSmoke(uv,time,0.1,1,dir*0.1);
                return MapColor(colorWeight,colors1,7);
            }
         
            Data vert (Data data) //顶点着色器，控制顶点位置
            {
                data.vertexPos = UnityObjectToClipPos(data.vertexPos);
                data.uv = TRANSFORM_TEX(data.uv, _MainTex);
                // 世界空间法线

                float3x3 normalMatrix = transpose((float3x3)unity_ObjectToWorld);
                data.worldNormal = normalize(mul(normalMatrix, data.normal));
                return data;
            }

           

        

            fixed4 frag(Data data) : SV_Target //片元着色器，控制像素颜色
            {
                fixed4 color = tex2D(_MainTex, data.uv);
                float time = Iris_Time.y;
                color = fragTest1(data.uv,time);

                return color;
            }

            ENDCG
        }
    }
}
