/****************************************
*
* 作者： 闪电黑客
* 日期： 2025/12/5 10:29
*
* 说明： Iris Shader 库外部统一入口文件
* 
* 设计理念：
* - 本文件是外部 Shader 文件使用的统一入口，避免路径写错
* - 内部工具文件之间使用直接路径引用
* 
* 使用方法：
* 通过条件编译宏（As_IrisXXX）控制工具模块的加载
* 
* 注意：
* - 本文件是给外部 Shader 文件使用的
* - 内部工具文件（Tools/）之间直接使用路径引用，不通过本文件
* 
*/

#ifndef Def_IrisEntry
#define Def_IrisEntry

// 加载参数接口
#include "Params/IrisParams.hlsl"

#ifdef As_IrisVertex
#include "Tools/IrisVertex.hlsl"
#endif


#ifdef As_IrisDistort 
#include "Tools/IrisDistort.hlsl" 
#endif

#ifdef As_IrisHash
#include "Tools/IrisHash.hlsl"
#endif

#ifdef As_IrisMath
#include "Tools/IrisMath.hlsl"
#endif

#ifdef As_IrisNoise
#include "Tools/IrisNoise.hlsl"
#endif

#ifdef As_IrisMatrix
#include "Tools/IrisMatrix.hlsl"
#endif



#endif