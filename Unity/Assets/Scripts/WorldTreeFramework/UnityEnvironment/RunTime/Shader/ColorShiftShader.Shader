Shader "Unlit/ColorShiftShader"
{
     Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Offs("Offset",Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Tools/Noise.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Offs;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col.r=tex2D(_MainTex, i.uv-float2(_Offs,0)).r;
                col.b=tex2D(_MainTex, i.uv+float2(_Offs,0)).b;

             
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                // // col.a = NoiseWorley(i.uv,_Time.z);
                return col;
            }
            ENDCG
        }
    }

}


// Shader "Unlit/TestRenderShader"
// {


// 	SubShader
// 	{

// 		Pass
// 		{
// 			ZTest Always ZWrite Off
// 			CGPROGRAM

// 			#pragma vertex vert_img //vert_img 是内置的顶点着色器
// 			#pragma fragment frag
// 			#include "UnityCG.cginc"

// 			sampler2D _CameraColorTexture; 

// 			//饱和度
//             fixed3  Saturation_float(float3 In, float Saturation)
//             {
//                 // float luma = dot(In, float3(0.2126729, 0.7151522, 0.0721750));
//                 float luma = dot(In, float3(1, 1, 0.0721750));
//                 return luma.xxx + Saturation.xxx * (In - luma.xxx);
//             }
//             //对比度
//             fixed3  Contrast_float(float3 In, float Contrast)
//             {
//                 float midpoint = pow(0.5, 2.2);
//                 return (In - midpoint) * Contrast + midpoint;
//             }

//             fixed4 frag (v2f_img i) : SV_Target
//             {
//                 fixed4 col = tex2D(_CameraColorTexture, i.uv);
//                 fixed3  Saturation =  Saturation_float(col, 5);
//                 fixed3  Contrast = Saturation_float(Saturation, 5);
//                 return fixed4(Contrast.x, Contrast.y, Contrast.z,0.5);
//             }
// 			ENDCG
// 		}
// 	}
//     FallBack Off
// }