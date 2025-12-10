/****************************************

* 作者： 闪电黑客
* 日期： 2025/12/10 10:43

* 描述： 顶点数据结构定义 
*
* Object Space (OS)：物体空间，顶点相对于模型自身的坐标系。
* World Space (WS)：世界空间，顶点相对于整个场景的坐标系。
* View Space (VS)：观察空间，顶点相对于摄像机的坐标系。
* Clip Space (CS)：裁剪空间，用于最终投影到屏幕。

*/

#ifndef Def_IrisVertData
#define Def_IrisVertData

// 判断是否为编辑模式
#ifndef IrisShader
    #define IrisEdit
#endif

// 顶点数据结构
struct VertData
{
    // 模型顶点数据
    #if defined(VD_Position)|| defined(IrisEdit)
        float4 Position   : POSITION;
    #endif
    // 顶点颜色
    #if defined(VD_Color)|| defined(IrisEdit)
        float4 Color        : COLOR;
    #endif
    // 顶点法线
    #if defined(VD_Normal)|| defined(IrisEdit)
        float3 Normal     : NORMAL;
    #endif
    // 顶点切线
    #if defined(VD_Tangent)|| defined(IrisEdit)
        float4 Tangent    : TANGENT;
    #endif
    // 顶点索引
    #if defined(VD_VertexID)|| defined(IrisEdit)
        uint VertexID       : SV_VertexID;
    #endif
    // 实例索引
    #if defined(VD_InstanceID)|| defined(IrisEdit)
        uint InstanceID     : SV_InstanceID;
    #endif
    
    // === 传值通道0~7（TEXCOORD语义）===
    // 支持类型：F1~4 = float~float4
    // 字段命名：TNFM (T=Texcoord, N=通道号0-7, F=Float, M=类型号1-4)
    // 使用方式：#define VD_T0F2  // 通道0，float2类型，字段名：T0F2
    // 常见用途：UV坐标、空间位置、法线、自定义数据等
    
    // 传值通道0
    #if defined(VD_T0F1) || defined(IrisEdit)
        float T0F1 : TEXCOORD0;
    #endif
    #if defined(VD_T0F2) || defined(IrisEdit)
        float2 T0F2 : TEXCOORD0;
    #endif
    #if defined(VD_T0F3) || defined(IrisEdit)
        float3 T0F3 : TEXCOORD0;
    #endif
    #if defined(VD_T0F4) || defined(IrisEdit)
        float4 T0F4 : TEXCOORD0;
    #endif

    // 传值通道1
    #if defined(VD_T1F1) || defined(IrisEdit)
        float T1F1 : TEXCOORD1;
    #endif
    #if defined(VD_T1F2) || defined(IrisEdit)
        float2 T1F2 : TEXCOORD1;
    #endif
    #if defined(VD_T1F3) || defined(IrisEdit)
        float3 T1F3 : TEXCOORD1;
    #endif
    #if defined(VD_T1F4) || defined(IrisEdit)
        float4 T1F4 : TEXCOORD1;
    #endif

    // 传值通道2
    #if defined(VD_T2F1) || defined(IrisEdit)
        float T2F1 : TEXCOORD2;
    #endif
    #if defined(VD_T2F2) || defined(IrisEdit)
        float2 T2F2 : TEXCOORD2;
    #endif
    #if defined(VD_T2F3) || defined(IrisEdit)
        float3 T2F3 : TEXCOORD2;
    #endif
    #if defined(VD_T2F4) || defined(IrisEdit)
        float4 T2F4 : TEXCOORD2;
    #endif

    // 传值通道3
    #if defined(VD_T3F1) || defined(IrisEdit)
        float T3F1 : TEXCOORD3;
    #endif
    #if defined(VD_T3F2) || defined(IrisEdit)
        float2 T3F2 : TEXCOORD3;
    #endif
    #if defined(VD_T3F3) || defined(IrisEdit)
        float3 T3F3 : TEXCOORD3;
    #endif
    #if defined(VD_T3F4) || defined(IrisEdit)
        float4 T3F4 : TEXCOORD3;
    #endif

    // 传值通道4
    #if defined(VD_T4F1) || defined(IrisEdit)
        float T4F1 : TEXCOORD4;
    #endif
    #if defined(VD_T4F2) || defined(IrisEdit)
        float2 T4F2 : TEXCOORD4;
    #endif
    #if defined(VD_T4F3) || defined(IrisEdit)
        float3 T4F3 : TEXCOORD4;
    #endif
    #if defined(VD_T4F4) || defined(IrisEdit)
        float4 T4F4 : TEXCOORD4;
    #endif

    // 传值通道5
    #if defined(VD_T5F1) || defined(IrisEdit)
        float T5F1 : TEXCOORD5;
    #endif
    #if defined(VD_T5F2) || defined(IrisEdit)
        float2 T5F2 : TEXCOORD5;
    #endif
    #if defined(VD_T5F3) || defined(IrisEdit)
        float3 T5F3 : TEXCOORD5;
    #endif
    #if defined(VD_T5F4) || defined(IrisEdit)
        float4 T5F4 : TEXCOORD5;
    #endif

    // 传值通道6
    #if defined(VD_T6F1) || defined(IrisEdit)
        float T6F1 : TEXCOORD6;
    #endif
    #if defined(VD_T6F2) || defined(IrisEdit)
        float2 T6F2 : TEXCOORD6;
    #endif
    #if defined(VD_T6F3) || defined(IrisEdit)
        float3 T6F3 : TEXCOORD6;
    #endif
    #if defined(VD_T6F4) || defined(IrisEdit)
        float4 T6F4 : TEXCOORD6;
    #endif

    // 传值通道7
    #if defined(VD_T7F1) || defined(IrisEdit)
        float T7F1 : TEXCOORD7;
    #endif
    #if defined(VD_T7F2) || defined(IrisEdit)
        float2 T7F2 : TEXCOORD7;
    #endif
    #if defined(VD_T7F3) || defined(IrisEdit)
        float3 T7F3 : TEXCOORD7;
    #endif
    #if defined(VD_T7F4) || defined(IrisEdit)
        float4 T7F4 : TEXCOORD7;
    #endif
};

#endif