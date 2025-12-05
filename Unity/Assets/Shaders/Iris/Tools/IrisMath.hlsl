/****************************************

* 作者： 闪电黑客
* 日期： 2025/12/2 19:50

* 描述： 各种数学函数合集 

*/
#ifndef IrisMath
#define IrisMath

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

// 颜色映射透明度划分？？？
float4 MapColorByAlpha(float colorWeight, float4 colors[8], int colorCount = 8)
{
    // 1. 计算总透明度
    float totalAlpha = 0.0;
    for (int i = 0; i < colorCount; i++)
    {
        totalAlpha += colors[i].a; // 累加每个颜色的 alpha 值
    }

    // 2. 计算每个颜色的透明度百分比
    float alphaPercentages[4];
    float cumulativePercentage = 0.0;
    for (int i = 0; i < colorCount; i++)
    {
        alphaPercentages[i] = colors[i].a / totalAlpha; // 当前颜色的透明度百分比
        cumulativePercentage += alphaPercentages[i]; // 累计百分比
        alphaPercentages[i] = cumulativePercentage; // 累计范围
    }

    // 3. 根据透明度百分比划分范围并映射颜色
    for (int i = 0; i < colorCount - 1; i++)
    {
        if (colorWeight < alphaPercentages[i])
        {
            float minWeight = i == 0 ? 0.0 : alphaPercentages[i - 1];
            float maxWeight = alphaPercentages[i];
            return lerp(colors[i], colors[i + 1], ClampMap(colorWeight, minWeight, maxWeight));
        }
    }

    // 4. 超出范围返回最后一个颜色
    return colors[colorCount - 1];
}

#endif