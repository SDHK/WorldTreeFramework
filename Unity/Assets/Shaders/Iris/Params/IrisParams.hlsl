/****************************************
*
* 作者： 闪电黑客
* 日期： 2025/12/4 19:42
*
* 说明： Iris Shader环境参数接口规范/模板
* 
* 设计理念：
* - 本文件定义了统一的参数接口规范
* - 不同引擎需要按照此规范实现对应的参数映射文件
* - 所有环境参数使用 Iris_ 前缀，保持命名空间统一
* 
* 实现要求：
* 1. 每个 #define 必须映射到对应引擎的内置变量
* 2. 变量类型必须匹配（float3/float4/float4x4等）
* 3. 参数语义需保持一致
*
*/
#ifndef Def_IrisParams
#define Def_IrisParams

#include "IrisStructFields.hlsl"

//===[常量]===

#define float_Zero 0.0
#define float2_Zero float2(0.0,0.0)
#define float3_Zero float3(0.0,0.0,0.0)
#define float4_Zero float4(0.0,0.0,0.0,0.0)

#define float4x4_Zero  float4x4( 0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0)
#define float4x4_Identity  float4x4( 1.0, 0.0, 0.0, 0.0,  0.0, 1.0, 0.0, 0.0,  0.0, 0.0, 1.0, 0.0,  0.0, 0.0, 0.0, 1.0)

//===[相机]===

//float3 世界空间中的相机位置。
#ifndef Iris_WorldSpaceCameraPos
#define Iris_WorldSpaceCameraPos float3_Zero
#endif

//float4 投影参数
#ifndef Iris_ProjectionParams
#define Iris_ProjectionParams float4_Zero
#endif
//float4 屏幕参数
#ifndef Iris_ScreenParams
#define Iris_ScreenParams float4_Zero
#endif
//float4 Z缓存参数
#ifndef Iris_ZBufferParams
#define Iris_ZBufferParams float4_Zero
#endif
//float4 正交参数
#ifndef Iris_OrthoParams
#define Iris_OrthoParams float4_Zero
#endif
//float4x4 相机投影矩阵
#ifndef Iris_CameraProjection
#define Iris_CameraProjection float4x4_Identity
#endif
//float4x4 相机投影矩阵的逆矩阵
#ifndef Iris_CameraInvProjection
#define Iris_CameraInvProjection float4x4_Identity 
#endif
//float4 相机投影参数
#ifndef Iris_CameraProjectionParams
#define Iris_CameraProjectionParams float4_Zero 
#endif

//===[矩阵]===

//float4x4 模型视图投影矩阵
#ifndef Iris_Matrix_MVP
#define Iris_Matrix_MVP float4x4_Identity
#endif

//float4x4 模型视图矩阵
#ifndef Iris_Matrix_MV
#define Iris_Matrix_MV float4x4_Identity
#endif
//float4x4 视图投影矩阵
#ifndef Iris_Matrix_VP
#define Iris_Matrix_VP float4x4_Identity
#endif
//float4x4 模型矩阵
#ifndef Iris_Matrix_M
#define Iris_Matrix_M float4x4_Identity
#endif
//float4x4 视图矩阵
#ifndef Iris_Matrix_V
#define Iris_Matrix_V float4x4_Identity
#endif
//float4x4 投影矩阵
#ifndef Iris_Matrix_P
#define Iris_Matrix_P float4x4_Identity
#endif

//float4x4 模型视图矩阵的转置
#ifndef Iris_Matrix_T_MV
#define Iris_Matrix_T_MV float4x4_Identity
#endif
//float4x4 模型视图矩阵的逆转置
#ifndef Iris_Matrix_IT_MV
#define Iris_Matrix_IT_MV float4x4_Identity
#endif
//float4x4 对象到世界矩阵
#ifndef Iris_ObjectToWorld
#define Iris_ObjectToWorld float4x4_Identity
#endif
//float4x4 世界到对象矩阵
#ifndef Iris_WorldToObject
#define Iris_WorldToObject float4x4_Identity
#endif

//===[时间]===

//float4 当前时间。x:时间/20，y:时间，z:时间x2，w:时间x3
#ifndef Iris_Time
#define Iris_Time float4_Zero 
#endif

//float4 正弦时间。x:sin(时间)，y:sin(时间/20)，z:sin(时间/200)，w:sin(时间x2)
#ifndef Iris_SinTime
#define Iris_SinTime float4_Zero
#endif

//float4 余弦时间。x:cos(时间)，y:cos(时间/20)，z:cos(时间/200)，w:cos(时间x2)
#ifndef Iris_CosTime
#define Iris_CosTime float4_Zero
#endif

//float4 上一帧的时间间隔。x:帧间隔时间，y:帧间隔时间/20，z:帧间隔时间/200，w:帧间隔时间x2
#ifndef Iris_DeltaTime
#define Iris_DeltaTime float4_Zero
#endif

#endif
