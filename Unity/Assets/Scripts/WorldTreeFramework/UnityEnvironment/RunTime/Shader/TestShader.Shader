Shader "Unlit/TestShader"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _RimColor ("Rim Color", Color) = (0.0, 0.7, 0.7,1)
        _Value1 ("Value1", Range(-5, 5)) = 0.6
        _Color1 ("Color 1", Color) = (0.101961, 0.619608, 0.666667, 1)
        _Color2 ("Color 2", Color) = (0.666667, 0.666667, 0.498039, 1)
        _Color3 ("Color 3", Color) = (0.0, 0.0, 0.164706, 1)
        _Color4 ("Color 4", Color) = (0.666667, 1.0, 1.0, 1)
    }

    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        // Blend SrcAlpha OneMinusSrcAlpha
        // ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Tools/Noise.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _RimColor;
            float _Value1;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float4 _Color4;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 Noise1(float2 uv, float time)
            {
                float4 col = float4(0.0, 0.0, 0.0,1);

                float2 q = float2(0.0, 0.0);
                q.x = NoiseFBMvalue(uv + 0.00 * time);
                q.y = NoiseFBMvalue(uv + float2(1.0, 0.0));

                float2 r = float2(0.0, 0.0);
                r.x = NoiseFBMvalue(uv + 1.0 * q + float2(1.7, 9.2) + 0.15 * time);
                r.y = NoiseFBMvalue(uv + 1.0 * q + float2(8.3, 2.8) + 0.126 * time);

                float f = NoiseFBMvalue(uv + r);

                col = lerp(_Color1, _Color2, clamp((f * f) * 4.0, 0.0, 1.0));
                col = lerp(col, _Color3, clamp(length(q), 0.0, 1.0));
                col = lerp(col, _Color4, clamp(length(r.x), 0.0, 1.0));

                col.rgb = (f * f * f + 0.6 * f * f + 0.5 * f) * col;
                // col.a = 1.0;
                return col;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float time = _Time.y;

                return Noise1(i.uv,time);


                // col *= NoiseWorley(i.uv,time);
                // return col;
            }

            ENDCG
        }
    }
}
