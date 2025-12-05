/****************************************
*
* 作者： 闪电黑客
* 日期： 2025/12/5 10:29
*
* 说明： Iris Shader核心工具库入口文件
* 
* 设计理念：
* - 本文件是工具库的统一入口，负责按需加载工具模块
* - 通过条件编译宏（Use_IrisXXX）控制工具模块的加载，避免引入不必要的代码，也防止路径写错
* 
* 使用方法：
* 1. 在 Shader 中定义需要的工具模块宏，如：#define Use_IrisMath
* 2. 然后 include 对应的引擎库文件（如 IrisCoreUnity.hlsl）
* 3. 即可使用对应工具模块中的函数
* 
*/
#ifndef IrisCore
#define IrisCore

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