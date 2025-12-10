/****************************************

* 作者： 闪电黑客
* 日期： 2025/12/9 19:53

* 描述： 矩阵计算集

*/

#ifndef Def_IrisMatrix
#define Def_IrisMatrix

#include "../Params/IrisParams.hlsl"

// 转换坐标系：从空间坐标转到摄像机坐标系
float4 WorldToCamera(float3 worldPos)
{
    return mul(Iris_Matrix_MVP, float4(worldPos, 1.0));
}



#endif
