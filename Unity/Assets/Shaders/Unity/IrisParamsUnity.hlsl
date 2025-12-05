/****************************************

* 作者： 闪电黑客
* 日期： 2025/12/4 20:33

* 描述： Unity着色器环境变量宏

*/
#ifndef IrisParamsUnity
#define IrisParamsUnity

//定义Unity的环境变量宏

//===[相机]===

//float3 世界空间中的相机位置。
#define Env_WorldSpaceCameraPos _WorldSpaceCameraPos 
//float4 投影参数
#define Env_ProjectionParams _ProjectionParams
//float4 屏幕参数
#define Env_ScreenParams _ScreenParams
//float4 Z缓存参数
#define Env_ZBufferParams _ZBufferParams
//float4 正交参数
#define Env_OrthoParams unity_OrthoParams
//float4x4 相机投影矩阵
#define Env_CameraProjection unity_CameraProjection
//float4x4 相机投影矩阵的逆矩阵
#define Env_CameraInvProjection unity_CameraInvProjection
//float4 相机投影参数
#define Env_CameraProjectionParams unity_CameraProjectionParams

//===[矩阵]===

//float4x4 模型视图投影矩阵
#define Env_Matrix_MVP UNITY_MATRIX_MVP
//float4x4 模型视图矩阵
#define Env_Matrix_MV UNITY_MATRIX_MV
//float4x4 视图投影矩阵
#define Env_Matrix_VP UNITY_MATRIX_VP
//float4x4 模型矩阵
#define Env_Matrix_M UNITY_MATRIX_M
//float4x4 视图矩阵
#define Env_Matrix_V UNITY_MATRIX_V
//float4x4 投影矩阵
#define Env_Matrix_P UNITY_MATRIX_P

//float4x4 模型视图矩阵的转置
#define Env_Matrix_T_MV UNITY_MATRIX_T_MV
//float4x4 模型视图矩阵的逆转置
#define Env_Matrix_IT_MV UNITY_MATRIX_IT_MV
//float4x4 对象到世界矩阵
#define Env_ObjectToWorld unity_ObjectToWorld
//float4x4 世界到对象矩阵
#define Env_WorldToObject unity_WorldToObject

//===[时间]===

//float4 当前时间。x:时间/20，y:时间，z:时间x2，w:时间x3
#define Env_Time _Time
//float4 正弦时间。x:sin(时间)，y:sin(时间/20)，z:sin(时间/200)，w:sin(时间x2)
#define Env_SinTime _SinTime
//float4 余弦时间。x:cos(时间)，y:cos(时间/20)，z:cos(时间/200)，w:cos(时间x2)
#define Env_CosTime _CosTime
//float4 上一帧的时间间隔。x:帧间隔时间，y:帧间隔时间/20，z:帧间隔时间/200，w:帧间隔时间x2
#define Env_DeltaTime _DeltaTime
#endif