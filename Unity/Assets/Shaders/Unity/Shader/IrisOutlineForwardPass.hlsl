#pragma vertex vert
#pragma fragment frag
// #pragma multi_compile __ _MAIN_LIGHT_SHADOWS
// #pragma multi_compile __ _MAIN_LIGHT_SHADOWS_CASCADE
// #pragma multi_compile __ _SHADOWS_SOFT

#define Use_UniversalShaderLibraryCore
#define Use_IrisMatrix
#define Use_IrisMath
#define Use_IrisVertex
#define Use_UniversalShaderLibraryLighting

#include "../IrisEntryUnity.hlsl"

struct VertData
{
    Var_PositionOS
    Var_Normal
    Var_T0(float2,UV)
};
         
struct FragData
{
    Var_PositionCS
    Var_T0(float2,UV)
    Var_T1(float3,NormalWS)
};

FragData vert(VertData vertData)
{
    FragData fragData;
    fragData.PositionCS = VertDefault(vertData.PositionOS);
    fragData.UV = TRANSFORM_TEX(vertData.UV, _MainTex);
                
    // 将法线转换到世界空间
    float3 normalWS = TransformObjectToWorldNormal(vertData.Normal);
    fragData.NormalWS = normalWS;
                
    return fragData;
}

half4 frag(FragData fragData) : SV_Target
{
    // 采样纹理
    half4 albedo = tex2D(_MainTex, fragData.UV);
                
    // 获取主光源信息
    Light mainLight = GetMainLight();
                
    // 计算简单的 Lambert 光照
    half NdotL = saturate(dot(normalize(fragData.NormalWS), mainLight.direction));
    half3 lighting = mainLight.color * NdotL + unity_AmbientSky.rgb;
                
    // 应用光照
    half4 finalColor = albedo;
    finalColor.rgb *= lighting;
                
    return finalColor;
}