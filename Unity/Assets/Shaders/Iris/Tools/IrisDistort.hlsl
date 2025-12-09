/****************************************

* 作者： 闪电黑客
* 日期： 2024/12/13 19:01

* 描述： 各种扭曲函数合集 

*/
#ifndef Def_IrisDistort 
#define Def_IrisDistort

//极坐标扭曲 
float2 DistortPolar(float2 uv, float distortAngle, float scale = 1)
{
    // 定义中心点坐标
    float2 center = (0.5, 0.5);
    
    // 计算 UV 坐标相对于中心点的偏移量,中心指向uv的向量
    float2 offset = uv - center;
    
    // 计算偏移量的距离（即从中心点到 UV 坐标的距离）
    float distance = sqrt(offset.x * offset.x + offset.y * offset.y) * (1 / scale);
    
    // 计算偏移量的角度（即从中心点到 UV 坐标的角度）
    float uvAngle = atan2(offset.y, offset.x);
    
    // 根据距离和时间对角度进行调整
    uvAngle = uvAngle + (distance * distortAngle);
    
    // 计算新的 UV 坐标
    float2 new_uv = float2(distance * cos(uvAngle), distance * sin(uvAngle)) + center;
    
    // 返回新的 UV 坐标
    return new_uv;
}


//旋涡扭曲
float2 DistortVortex(float2 uv, float distortAngle, float scale = 1, float2 center = (0.5, 0.5))
{
    // 计算 UV 坐标相对于中心点的偏移量,中心指向uv的向量
    float2 offset = uv - center;
    
    // 计算偏移量的距离（即从中心点到 UV 坐标的距离）
    float distance = sqrt(offset.x * offset.x + offset.y * offset.y);
    
    // 计算偏移量的角度（即从中心点到 UV 坐标的角度）
    float uvAngle = atan2(offset.y, offset.x);
    
    // 计算 distance 大于 scale 时 a 变成 0，小于 scale 时 a 变成 1
    float IsDistort = step(distance, scale);

    // 根据距离和时间对角度进行调整
    uvAngle = uvAngle + ((scale - distance) * (distortAngle))  * IsDistort;
    
    // 计算新的 UV 坐标
    float2 new_uv = float2(distance * cos(uvAngle), distance * sin(uvAngle)) + center;
    
    // 返回新的 UV 坐标
    return new_uv;
}


// 球面扭曲
// uv: 输入的UV坐标
// center: 扭曲的中心点
// radius: 扭曲的半径
float2 DistortSphere(float2 uv, float radius, float2 center = (0.5, 0.5))
{
    // 计算 UV 坐标相对于中心点的偏移量
    float2 offset = uv - center;
    
    // 计算偏移量的距离
    float distance = length(offset);
    
    // 计算偏移量的角度
    float uvAngle = atan2(offset.y, offset.x);
    
    // 计算 distance 大于 scale 时 a 变成 0，小于 scale 时 a 变成 1
    float IsDistort = step(distance, radius);
    
    distance = ((distance * distance) / (radius * radius)) * (distance / radius) * radius * IsDistort;
    
    // 计算新的 UV 坐标
    float2 new_uv = float2(distance * cos(uvAngle), distance * sin(uvAngle)) + center;
    
    // 返回新的 UV 坐标
    return new_uv;
}


// 波浪扭曲
// uv: 输入的UV坐标
// amplitude: 扭曲的振幅
// frequency: 扭曲的频率
// phase: 扭曲的相位
float2 DistortWave(float2 uv, float amplitude, float frequency, float phase)
{
    uv.x += sin(uv.y * frequency + phase) * amplitude;
    uv.y += sin(uv.x * frequency + phase) * amplitude;
    return uv;
}


#endif