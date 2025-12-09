/****************************************

* 作者： 闪电黑客
* 日期： 2024/12/12 20:27

* 描述： 各种噪声函数合集 

*/
#ifndef Def_IrisNoise
#define Def_IrisNoise

#include "IrisHash.hlsl" //引用Hash库

//根据角度计算半径为的圆上的点 0~1
float2 AngleToUV(float angle)
{
    angle %= 360;
    return frac(float2(cos(angle) + 1, sin(angle) + 1) * 0.5);
}

float3 Random3(float3 c)
{
    float j = 4096.0 * sin(dot(c, float3(17.0, 59.4, 15.0)));
    float3 r;
    r.z = frac(512.0 * j);
    j *= .125;
    r.x = frac(512.0 * j);
    j *= .125;
    r.y = frac(512.0 * j);
    return r - 0.5;
}

float2 Random2(float2 c)
{
    float j = 4096.0 * sin(dot(c, float2(17.0, 59.4)));
    float2 r;
    r.x = frac(512.0 * j);
    j *= .125;
    r.y = frac(512.0 * j);
    return r - 0.5;
}

float Random(float c)
{
    float j = 4096.0 * sin(dot(c, 17.0));
    float r;
    r = frac(512.0 * j);
    j *= .125;
    return r - 0.5;
}

//一维噪声
float Noise1D(float x)
{
    return frac(sin(x) * 1000);
}

//栅格噪声
float NoiseGrid(float x)
{
    return noise(floor(x));
}

//平滑噪声
float NoiseLerp(float x)
{
    float t = frac(x);
    float u = t * t * (3 - 2 * t);
    return lerp(noise(floor(x)), noise(floor(x + 1)), u);
    //return lerp(noise(floor(x)), noise(floor(x + 1)), frac(x));
}



//白噪声
float NoiseWhite(float2 uv, float time = 0)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233) + time)) * 43758.5453123);
}

//值噪声
float NoiseValue(float2 uv)
{
    // 将时间参数引入到 uv 的计算中
    float2 intPos = floor(uv); // uv 晶格化, 取 uv 整数值，相当于晶格id
    float2 fracPos = frac(uv); // 取 uv 小数值，相当于晶格内局部坐标，取值区间：(0,1)

    // 二维插值权重，一个类似 smoothStep 的函数，叫 Hermit 插值函数，也叫 S 曲线：S(x) = -2 x^3 + 3 x^2
    // 利用 Hermit 插值特性：可以在保证函数输出的基础上保证插值函数的导数在插值点上为 0，这样就提供了平滑性
    float2 u = fracPos * fracPos * (3.0 - 2.0 * fracPos);

    // 四方取点，由于 intPos 是固定的，所以栅格化了（同一晶格内四点值相同，只是小数部分不同拿来插值）
    float va = hash2to1(intPos + float2(0.0, 0.0)); // hash2to1 二维输入，映射到 1 维输出
    float vb = hash2to1(intPos + float2(1.0, 0.0));
    float vc = hash2to1(intPos + float2(0.0, 1.0));
    float vd = hash2to1(intPos + float2(1.0, 1.0));

    // lerp 的展开形式，完全可以用 lerp(a, b, c) 嵌套实现
    float k0 = va;
    float k1 = vb - va;
    float k2 = vc - va;
    float k4 = va - vb - vc + vd;
    float value = k0 + k1 * u.x + k2 * u.y + k4 * u.x * u.y;

    return value;
}


//柏林噪声
float NoisePerlin(float2 uv)
{
    float2 intPos = floor(uv);
    float2 fracPos = frac(uv);

    float2 u = fracPos * fracPos * (3.0 - 2.0 * fracPos);

    float2 ga = hash22(intPos + float2(0.0, 0.0)); //四角hash向量
    float2 gb = hash22(intPos + float2(1.0, 0.0));
    float2 gc = hash22(intPos + float2(0.0, 1.0));
    float2 gd = hash22(intPos + float2(1.0, 1.0));

    float va = dot(ga, fracPos - float2(0.0, 0.0)); //方向向量、点积
    float vb = dot(gb, fracPos - float2(1.0, 0.0));
    float vc = dot(gc, fracPos - float2(0.0, 1.0));
    float vd = dot(gd, fracPos - float2(1.0, 1.0));

    float value = va + u.x * (vb - va) + u.y * (vc - va) + u.x * u.y * (va - vb - vc + vd); //插值

    return value;
}

//简单噪声
float NoiseSimple(float2 uv, float time = 0)
{
    const float K1 = 0.366025404; // (sqrt(3)-1)/2;
    const float K2 = 0.211324865; // (3-sqrt(3))/6;

    // 使用时间参数扰动 uv
    float2 i = floor(uv + (uv.x + uv.y + time) * K1); // 使用 float2 替换 vec2，并将 p 替换为 uv
    float2 a = uv - (i - (i.x + i.y) * K2); // 使用 float2 替换 vec2，并将 p 替换为 uv
    float2 o = (a.x < a.y) ? float2(0.0, 1.0) : float2(1.0, 0.0); // 使用 float2 替换 vec2
    float2 b = a - o + K2;
    float2 c = a - 1.0 + 2.0 * K2;
    float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0); // 使用 float3 替换 vec3
    float3 n = h * h * h * h * float3(dot(a, hash22(i)), dot(b, hash22(i + o)), dot(c, hash22(i + 1.0))); // 使用 float3 替换 vec3
    return dot(float3(70.0, 70.0, 70.0), n); // 使用 float3 替换 vec3
}



//泰森多边形
float NoiseVoronoi(float2 uv, float time = 0)
{
    float dist= 16;
    float2 intPos = floor(uv); //取整
    float2 fracPos = frac(uv); //取小数
    for (int x = -1; x <= 1; x++) //3x3九宫格采样
    {
        for (int y = -1; y <= 1; y++)
        {
            float2 offset = float2(x, y); //周围的偏移
            float2 pos = hash22(intPos + offset); //生成随机特征点
            pos = sin(time + 6.2831 * pos) * 0.5 + 0.5; //特征点随时间变化
            float d = distance(pos + float2(x, y), fracPos); //fracPos作为采样点，hash22(intPos)作为生成点，来计算dist
            dist = min(dist, d);
        }
    }
    return dist;
}


//泰森多边形
float NoiseWorley(float2 uv, float time = 0)
{
    float2 index = floor(uv);
    float2 fracPos = frac(uv);
    float2 d = float2(1.5, 1.5);
    for (int i = -1; i < 2; i++)
        for (int j = -1; j < 2; j++)
        {
            float2 pos = hash22(index + float2(i, j));
            pos = sin(time + 6.2831 * pos) * 0.5 + 0.5; //特征点随时间变化
            float dist = distance(pos + float2(i, j), fracPos);
            if (dist < d.x)
            {
                d.y = d.x;
                d.x = dist;
            }
            else
                d.y = min(dist, d.y);
        }
    return d.y - d.x;
}

//分形布朗运动
float NoiseFBMvalue(float2 uv)
{
    float value = 0;
    float amplitude = 0.5;
    float frequency = 0;

    for (int i = 0; i < 8; i++)
    {
        value += amplitude * NoiseValue(uv); //使用最简单的value噪声做分形，其余同理。
        uv *= 2.0;
        amplitude *= .5;
    }
    return value;
}

//云雾噪声
// uv: 输入的UV坐标
// time: 时间参数，用于动画效果
// uvSpeed: UV移动速度
// scale: 缩放
// dir: 扰动方向
// 返回值: 生成的云雾噪声值,当色彩权重用
float NoiseSmoke(float2 uv, float time = 0, float uvSpeed=10, float scale =1, float2 dir = float2(0.0, 0.10))
{
    float2 q = float2(0.0, 0.0);
    uv += dir * time * uvSpeed;
    //水面流动效果
    q.x = NoiseFBMvalue(uv + time * dir);
    q.y = NoiseFBMvalue(uv + float2(1.0, 0.0));

    //云雾细节效果
    float2 r = float2(0.0, 0.0);
    r.x = NoiseFBMvalue(uv + q * scale + time * dir ); // + float2(1.7, 9.2)
    r.y = NoiseFBMvalue(uv + q * scale + time * dir ); // + float2(8.3, 2.8)
    
    float f = NoiseFBMvalue(uv + r);
    return (f * f * f + 0.6 * f * f + 0.5 * f);
}

// 高斯模糊
// _MainTex: 输入的纹理
// uv: 输入的UV坐标
// blur: 模糊的程度
float4 BlurGaussian(sampler2D _MainTex, float2 uv, float blur)
{
    // 1 / 16
    float offset = blur * 0.0625f;
    // 左上
    float4 color = tex2D(_MainTex, float2(uv.x - offset, uv.y - offset)) * 0.0947416f;
    // 上
    color += tex2D(_MainTex, float2(uv.x, uv.y - offset)) * 0.118318f;
    // 右上
    color += tex2D(_MainTex, float2(uv.x + offset, uv.y + offset)) * 0.0947416f;
    // 左
    color += tex2D(_MainTex, float2(uv.x - offset, uv.y)) * 0.118318f;
    // 中
    color += tex2D(_MainTex, float2(uv.x, uv.y)) * 0.147761f;
    // 右
    color += tex2D(_MainTex, float2(uv.x + offset, uv.y)) * 0.118318f;
    // 左下
    color += tex2D(_MainTex, float2(uv.x - offset, uv.y + offset)) * 0.0947416f;
    // 下
    color += tex2D(_MainTex, float2(uv.x, uv.y + offset)) * 0.118318f;
    // 右下
    color += tex2D(_MainTex, float2(uv.x + offset, uv.y - offset)) * 0.0947416f;
    color.rgb *= color.a;
    return color;
}

#endif