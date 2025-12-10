/****************************************

* 作者： 闪电黑客
* 日期： 2025/12/2 19:50

* 描述： 各种数学函数合集 

*/
#ifndef Def_IrisMath
#define Def_IrisMath

#include "../Params/IrisParams.hlsl"


//钳制映射
//将value从min-max映射到targetMin-targetMax之间，并进行钳制
float ClampMap(float value, float min, float max, float targetMin = 0, float targetMax = 1)
{
    //假设要0.5到0.8之间的值，那么就用value-0.5，然后再减去0.8-0.5=0.3
    value -= min;
    max -= min;
    value = clamp(value, 0.0, max);
    value /= max;
    value = value * (targetMax - targetMin) + targetMin;
    return value;
}

// 颜色映射
float4 MapColor(float colorWeight, float4 colors[8], int colorCount = 8)
{
    float step = 1.0 / (colorCount - 1); // 每段的权重范围
    for (int i = 0; i < colorCount - 1; i++)
    {
        float minWeight = i * step;
        float maxWeight = (i + 1) * step;
        if (colorWeight >= minWeight && colorWeight < maxWeight)
        {
            return lerp(colors[i], colors[i + 1], ClampMap(colorWeight, minWeight, maxWeight));
        }
    }
    return colors[colorCount - 1]; // 超出范围返回最后一个颜色
}

// 沿法线方向缩放位置
float3 Scale(float3 position,float3 normal,float scale)
{
    return position + normal * scale;
}



#endif