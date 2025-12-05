/****************************************

* 作者： 闪电黑客
* 日期： 2025/12/5 10:29

* 描述： Shader 库引用文件

*/
#ifndef IrisShaderLibrary
#define IrisShaderLibrary

#ifdef Use_IrisDistort 
#include "Tools/IrisDistort.hlsl" 
#endif

#ifdef Use_IrisHash
#include "Tools/IrisHash.hlsl"
#endif

#ifdef Use_IrisMath
#include "Tools/IrisMath.hlsl"
#endif

#ifdef Use_IrisNoise
#include "Tools/IrisNoise.hlsl"
#endif

#endif