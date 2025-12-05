/****************************************
*
* 作者： 
* 日期： 2025/12/4 19:42
*
* 说明： Iris Shader环境参数接口规范/模板
* 
* 设计理念：
* - 本文件定义了统一的参数接口规范
* - 不同引擎需要按照此规范实现对应的参数映射文件
* - 所有环境参数使用 Env_ 前缀，保持命名空间统一
* 
* 实现要求：
* 1. 每个 #define 必须映射到对应引擎的内置变量
* 2. 变量类型必须匹配（float3/float4/float4x4等）
* 3. 参数语义需保持一致
* 
* 注意：本文件只包含接口定义，不包含实际实现
*
*/
#ifndef IrisParams
#define IrisParams


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