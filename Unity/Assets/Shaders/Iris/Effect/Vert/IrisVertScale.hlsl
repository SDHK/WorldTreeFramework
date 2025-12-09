#ifndef Def_IrisVertScale
#define Def_IrisVertScale

#include "../../IrisParams.hlsl"

struct appdata
{
    float4 positionOS : POSITION; // 对象空间位置
    float3 normalOS : NORMAL;     // 对象空间法线
    float offset : TEXCOORD0;     // 扩展偏移量
};
struct v2f
{
    float4 positionCS : SV_POSITION; // 裁剪空间位置
};

v2f vert(appdata data)
{
    v2f output;
       // 沿法线方向扩展顶点
    float3 positionOS = data.positionOS.xyz + data.normalOS * data.offset;
                
     // 手动计算裁剪空间位置
    output.positionCS = mul(Iris_Matrix_MVP,float4(positionOS, 1.0));
    return output;
}

#endif
