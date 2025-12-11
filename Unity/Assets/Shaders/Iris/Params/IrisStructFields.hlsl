/****************************************
*
* 作者： 闪电黑客
* 日期： 2025/12/10 14:18
*
* 描述： 结构体字段定义宏
* 
* 说明：
* - OS (Object Space)：物体空间，顶点相对于模型自身的坐标系
* - WS (World Space)：世界空间，顶点相对于整个场景的坐标系
* - VS (View Space)：观察空间，顶点相对于摄像机的坐标系
* - CS (Clip Space)：裁剪空间，用于最终投影到屏幕的坐标系
* 
* 空间转换流程：OS → WS → VS → CS
* 
*
*/

#ifndef Def_IrisStructFields
#define Def_IrisStructFields

//===[数据结构字段映射]===
// 顶点位置
#define Var_PositionOS float4 PositionOS : POSITION;
// 顶点颜色
#define Var_Color float4 Color : COLOR;
// 顶点法线
#define Var_Normal float3 Normal : NORMAL;
// 顶点切线
#define Var_Tangent float4 Tangent : TANGENT;
// 顶点ID
#define Var_VertexID uint VertexID : SV_VertexID;
// 实例ID
#define Var_InstanceID uint InstanceID : SV_InstanceID;

// 顶点在屏幕空间位置
#define Var_PositionCS float4 PositionCS : SV_POSITION;
// 正面检测（双面渲染）
#define Var_IsFrontFace bool IsFrontFace : SV_IsFrontFace;


// ===[传值通道字段映射]===
#define Var_T0(type,name) type name : TEXCOORD0;
#define Var_T1(type,name) type name : TEXCOORD1;
#define Var_T2(type,name) type name : TEXCOORD2;
#define Var_T3(type,name) type name : TEXCOORD3;
#define Var_T4(type,name) type name : TEXCOORD4;
#define Var_T5(type,name) type name : TEXCOORD5;
#define Var_T6(type,name) type name : TEXCOORD6;
#define Var_T7(type,name) type name : TEXCOORD7;


#endif
