/****************************************
*
* 作者： 闪电黑客
* 日期： 2025/12/4 14:16
*
* 说明： Unity 引擎的 Iris Shader 库统一入口
* 
* 功能：
* - 加载 Unity 环境参数映射
* - 加载 Iris 库入口（IrisEntry.hlsl）
* - 可选加载 Unity URP/CG 库
*
*/

#ifndef Def_IrisEntryUnity
#define Def_IrisEntryUnity

#ifdef Use_UniversalShaderLibraryCore
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

#ifdef Use_UniversalShaderLibraryLighting
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

#ifdef Use_UnityCG
    #include "UnityCG.cginc"
#endif

// 引用 Unity 着色器环境变量定义
#include "IrisParamsUnity.hlsl"

// 引用 Iris库目录
#include "../Iris/IrisEntry.hlsl"




#endif
