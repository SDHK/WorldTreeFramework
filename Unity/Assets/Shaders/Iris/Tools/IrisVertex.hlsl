/****************************************

* 作者： 闪电黑客
* 日期： 2025/12/10 18:29

* 描述： Iris 顶点工具函数 

*/
#ifndef Def_IrisVertex
#define Def_IrisVertex


#include "../Params/IrisParams.hlsl"

// 顶点沿法线方向缩放
float4 VertScale(float4 position ,float3 normal,  float scale)
{
    // 顶点沿法线方向扩展顶点
    float3 position3 = position.xyz + normal * scale;
     // 计算裁剪空间位置
    return mul(Iris_Matrix_MVP, float4(position3, 1.0));
}

// 顶点位置默认变换
float4 VertDefault(float4 position)
{
    return mul(Iris_Matrix_MVP, position);
}


#endif
