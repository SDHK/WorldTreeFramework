/****************************************

* 作者： 闪电黑客
* 日期： 2025/12/4 19:42

* 描述： 核心着色器环境变量定义模板
* 主要是定义一些常用的环境变量。
* 它不能被使用
*/
#ifndef IrisShaderParams
#define IrisShaderParams


//===[相机]===

//float3 世界空间中的相机位置。
#define Env_WorldSpaceCameraPos  
//float4 投影参数
#define Env_ProjectionParams 
//float4 屏幕参数
#define Env_ScreenParams 
//float4 Z缓存参数
#define Env_ZBufferParams 
//float4 正交参数
#define Env_OrthoParams 
//float4x4 相机投影矩阵
#define Env_CameraProjection 
//float4x4 相机投影矩阵的逆矩阵
#define Env_CameraInvProjection 
//float4 相机投影参数
#define Env_CameraProjectionParams 

//===[矩阵]===

//float4x4 模型视图投影矩阵
#define Env_Matrix_MVP 
//float4x4 模型视图矩阵
#define Env_Matrix_MV 
//float4x4 视图投影矩阵
#define Env_Matrix_VP 
//float4x4 模型矩阵
#define Env_Matrix_M 
//float4x4 视图矩阵
#define Env_Matrix_V 
//float4x4 投影矩阵
#define Env_Matrix_P 

//float4x4 模型视图矩阵的转置
#define Env_Matrix_T_MV 
//float4x4 模型视图矩阵的逆转置
#define Env_Matrix_IT_MV 
//float4x4 对象到世界矩阵
#define Env_ObjectToWorld 
//float4x4 世界到对象矩阵
#define Env_WorldToObject 

//===[时间]===

//float4 当前时间。x:时间/20，y:时间，z:时间x2，w:时间x3
#define Env_Time 
//float4 正弦时间。x:sin(时间)，y:sin(时间/20)，z:sin(时间/200)，w:sin(时间x2)
#define Env_SinTime 
//float4 余弦时间。x:cos(时间)，y:cos(时间/20)，z:cos(时间/200)，w:cos(时间x2)
#define Env_CosTime 
//float4 上一帧的时间间隔。x:帧间隔时间，y:帧间隔时间/20，z:帧间隔时间/200，w:帧间隔时间x2
#define Env_DeltaTime 

#endif