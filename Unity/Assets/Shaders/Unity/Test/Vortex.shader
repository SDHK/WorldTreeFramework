Shader "Unlit/Vortex"
{
    Properties 
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Angle  ("Angle", Range(-10, 10)) = 45.0
        _Scale  ("Scale", Range(-10, 10)) = 45.0
    }

    SubShader 
    {
         //一些性能处理
        Cull Off
        //灯光
        Lighting Off
        //深度
        ZWrite Off
        //雾
        Fog { Mode Off }

        Pass 
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define Use_IrisDistort
            #include "../IrisEntryUnity.hlsl"

            sampler2D _MainTex;
            float _Angle;
            float _Scale;
            float2 _Center;

            struct v2f 
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            // 顶点着色器
            v2f vert(appdata_full v)
            {
                v2f o;
                //模型空间转裁剪空间
                o.position = UnityObjectToClipPos (v.vertex);
                //获取uv
                o.uv = v.texcoord;
                return o;
            }

            //片段着色器
            float4 frag (v2f o) : SV_Target
            {
                return tex2D(_MainTex,DistortVortex(o.uv,_Angle,_Scale,_Center ));

                // return tex2D(_MainTex,NoiseWhite(o.uv ));
            }

            ENDCG
        }
    }
}
