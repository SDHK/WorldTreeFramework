﻿# ShaderLab

+ 
    <details><summary>SubShader - 子着色器。</summary>

    + 
        <details><summary>Tags - 这个着色器的标签。</summary>

        + 
            <details><summary>Queue - 这个着色器的渲染队列。</summary>

            |名称|值|说明|
            |----|----|----|
            |Background|1000|天空盒着色器。|
            |Geometry|2000|(默认) 它用于大多数的对象，不透明的几何体使用这个队列。|
            |AlphaTest|2450|用于Alpha测试的着色器,用于需要透明度测试的着色器。|
            |GeometryLast|2500|用于最后渲染的几何体。|
            |Transparent|3000|用于半透明着色器（透明着色器、粒子着色器、字体着色器、地形额外通道的着色器）。|
            |Overlay|4000|光晕着色器、闪光着色器。|
            </details>

        + 
            <details><summary>RenderType - 这个着色器的渲染类型。</summary>

            |名称|说明|
            |----|----|
            |Opaque|用于大多数着色器（法线着色器、自发光着色器、反射着色器以及地形的着色器）。|
            |Transparent|用于半透明着色器（透明着色器、粒子着色器、字体着色器、地形额外通道的着色器）。|
            |TransparentCutout|用于蒙皮透明着色器（Transparent Cutout，两个通道的植被着色器）。|
            |Background|用于天空盒着色器。|
            |Overlay|用于光晕着色器、闪光着色器。|
            |TreeOpaque|用于树木着色器。|
            |TreeTransparentCutout|用于树木透明着色器。|
            |TreeBillboard|用于树木广告牌着色器。|
            |Grass|用于草着色器。|
            |GrassBillboard|用于草广告牌着色器。|
            </details>

        + 
            <details><summary>PreviewType - 预览类型。</summary>

            |名称|说明|
            |----|----|
            |Plane|平面。|
            |Skybox|天空盒。|
            |Sphere|球体。|
            </details>

        + 
            <details><summary>LightMode - 光照模式。</summary>

            |名称|说明|
            |----|----|
            |Always|（默认）总是渲染。|
            |ForwardBase|在前向渲染中使用；应用环境光、主方向光、顶点/SH 光源和光照贴图|
            |ForwardAdd|在前向渲染中使用；应用附加的每像素光源（每个光源有一个通道）|
            |Deferred|在延迟渲染中使用；渲染 G 缓冲区|
            |ShadowCaster|将对象深度渲染到阴影贴图或深度纹理中|
            |MotionVectors|用于计算每个对象的运动矢量|
            |Vertex|用于旧版顶点光照渲染（当对象不进行光照贴图时）；应用所有顶点光源|
            |VertexLMRGBM|用于旧版顶点光照渲染（当对象不进行光照贴图时），以及光照贴图为 RGBM 编码的平台（PC 和游戏主机）|
            |VertexLM|用于旧版顶点光照渲染（当对象不进行光照贴图时），以及光照贴图为双 LDR 编码的平台上（移动平台）|
            |Meta|用于渲染预览图标。此通道不用于常规渲染，仅用于光照贴图烘焙或光照实时全局照明|
            </details>

        + ForceNoShadowCasting - True强制不投射阴影,在 HDRP 中，这不会阻止几何体投射接收阴影。
        + DisableBatching  - 禁用批处理。动态批处理会将所有几何体都变换为世界空间，这意味着着色器程序无法再访问对象空间。 因此，依赖于对象空间的着色器程序不会正确渲染。为避免此问题，请使用此子着色器标签阻止 Unity 应用动态批处理。
        + IgnoreProjector - 是否忽略投影器。True不接受投影,不会被 Projector 影响。
        + CanUseSpriteAtlas - 是否可以使用精灵图集。False 不可以使用 SpriteAtlas ,如果该 shader 是为精灵写的，并且当他们被包装为图集的时候，该 shader 将不会起作用。

        + DisableNoSubshadersMessage - 禁用无子着色器消息。

        </details>
    
    + LOD - 着色器的LOD（细节层次）。0 是最高细节，200 是最低细节。SubShader必须是降序设置这个值
    
    + 
        <details><summary>Cull Front - 剔除模式。</summary>
    
        |名称|说明|
        |----|----|
        |Off|不剔除。|
        |Front|正面剔除。|
        |Back|背面剔除。|
		</details>

    + 
        <details><summary> ZTest Always - 深度测试模式。</summary>
        
        |名称|说明|
        |----|----|
        |Always|总是通过深度测试。|
        |Never|从不通过深度测试。|
        |Less|小于深度测试。|
        |Equal|等于深度测试。|
        |LessEqual|小于等于深度测试。|
        |Greater|大于深度测试。|
        |NotEqual|不等于深度测试。|
        |GreaterEqual|大于等于深度测试。|
        |Off|关闭深度测试。|
        </details>

    + 
        <details><summary> Blend SrcAlpha OneMinusSrcAlpha - 混合模式。</summary>
        
        混合公式是：混合颜色 = (源颜色 * 源Alpha) + (目标颜色 * (1 - 源Alpha))

        | 名称 | 说明 | 
        | ---- | ---- | 
        | Zero | 零。所有颜色分量都为0。 | 
        | One | 一。所有颜色分量都为1。 | 
        | DstColor | 目标颜色。使用当前帧缓冲区中的颜色值。 | 
        | SrcColor | 源颜色。使用当前绘制的片段颜色值。 | 
        | OneMinusDstColor | 1-目标颜色。使用1减去当前帧缓冲区中的颜色值。 | 
        | OneMinusSrcColor | 1-源颜色。使用1减去当前绘制的片段颜色值。 | 
        | DstAlpha | 目标Alpha。使用当前帧缓冲区中的Alpha值。 | 
        | SrcAlpha | 源Alpha。使用当前绘制的片段Alpha值。 | 
        | OneMinusDstAlpha | 1-目标Alpha。使用1减去当前帧缓冲区中的Alpha值。 | 
        | OneMinusSrcAlpha | 1-源Alpha。使用1减去当前绘制的片段Alpha值。 |
        </details>

    + 
        <details><summary> ColorMask  RGB - 颜色掩码</summary>
    
        | 名称 | 说明 |
        | ---- | ---- |
        | R | 红色通道。 |
        | G | 绿色通道。 |
        | B | 蓝色通道。 |
        | A | Alpha通道。 |
        | 0 | 关闭通道。 |
        | RGBA |开启所有通道。|

        </details>

    + 
        <details><summary> Fog - 雾效。 </summary>

        语法：
        Fog 
        {
            Mode Linear
            Color (0.5, 0.5, 0.5, 1.0)
            Start 10.0
            End 100.0
        }

		| 名称 | 说明 |
		| ---- | ---- |
        | Mode | 雾效模式。Off关闭。Linear 线性雾效。Exp 指数雾效。Exp2 指数平方雾效。 |
        | Color | 雾效颜色。 |
        | Start | 雾效开始距离。 |
        | End | 雾效结束距离。 |

    + ZWrite  On\Off - 是否写入深度缓冲区。通常用于透明物体，以避免深度缓冲区的更新，从而防止透明物体遮挡后面的物体。
    + Lighting  On\Off - 是否开启光照模式。对于不需要光照效果的Shader（如Unlit Shader），可以关闭光照计算以提高性能。
    + Name  "名称" - 子着色器的名称。可以帮助开发者识别和区分不同的子着色器。
    + Dependency "名称" "shader路径" - 依赖项。标签用于指定当前着色器依赖的其他资源或着色器。
    + Fallback "Diffuse名称" -备用着色器。
    + Stencil - 模板缓冲区。Ref Comp Pass Fail ZFail。Ref是模板缓冲区的参考值，Comp是模板缓冲区的比较函数，Pass是模板缓冲区的通过操作，Fail是模板缓冲区的失败操作，ZFail是模板缓冲区的深度失败操作。
    </details>



