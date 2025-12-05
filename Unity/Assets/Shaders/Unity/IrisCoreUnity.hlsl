/****************************************
*
* 作者： 闪电黑客
* 日期： 2025/12/4 14:16
*
* 说明： Unity 引擎的 Iris Shader 库入口
* 
* 功能：
* - 加载 Unity 环境参数映射
* - 加载 Iris 核心库
* - 可选加载 Unity URP/CG 库
*
*/

#ifndef IrisCoreUnity
#define IrisCoreUnity

// 引用 Unity 着色器环境变量定义
#include "IrisParamsUnity.hlsl"

// 引用 Iris 着色器库
#include "../Iris/IrisCore.hlsl"


#ifdef Use_UniversalShaderLibraryCore
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

#ifdef Use_UnityCG
    #include "UnityCG.cginc"
#endif


#endif
