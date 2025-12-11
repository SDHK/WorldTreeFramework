/****************************************

* 作者： 闪电黑客
* 日期： 2025/12/10 20:38

* 描述： 

*/


#pragma vertex vert
#pragma fragment frag

#define Use_UniversalShaderLibraryCore
#define Use_IrisMatrix
#define Use_IrisMath
#define Use_IrisVertex
#include "../IrisEntryUnity.hlsl"

struct VertData
{
    Var_PositionOS
    Var_Normal
};

struct FragData
{
    Var_PositionCS
};
            
float4 _Color;
float _Scale;

FragData vert(VertData vertData)
{
    FragData fragData;
    fragData.PositionCS = VertScale(vertData.PositionOS, vertData.Normal, _Scale);
    return fragData;
}
            
half4 frag(FragData fragData) : SV_Target
{
    return _Color; // 返回红色
}
