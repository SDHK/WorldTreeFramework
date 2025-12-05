/****************************************

* 作者： 闪电黑客
* 日期： 2025/12/4 14:16

* 描述： Unity 着色器库引用

*/
#ifndef UnityShaderLibrary // 这个宏的作用是防止重复引用
#define UnityShaderLibrary

// 引用 Unity 着色器环境变量定义
#include "UnityShaderParams.hlsl"

// 引用 Iris 着色器库
#include "../Iris/IrisShaderLibrary.hlsl"



#ifdef Use_UniversalShaderLibraryCore
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

#ifdef Use_UnityCG
    #include "UnityCG.cginc"
#endif


#endif
