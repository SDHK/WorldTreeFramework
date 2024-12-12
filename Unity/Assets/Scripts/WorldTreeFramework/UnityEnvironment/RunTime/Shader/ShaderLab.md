# ShaderLab

+ 
    <details><summary>Properties - 属性。</summary>

    _MainTex ("MainTex", 2D) = "white" {}

    |属性类型|默认值的语法|例子|
    |----|----|----|
    |Int|number|_MyInt ("MyInt", Int) = 2|
    |Float|number|_MyFloat ("MyFloat", Float) = 1.5|
    |Range|number|_MyRange ("MyRange", Range(0, 1)) = 0.5|
    |Color| (number, number, number, number)|_MyColor ("MyColor", Color) = (1, 0, 0, 1)|
    |Vector| (number, number, number, number)|_MyVector ("MyVector", Vector) = (0, 1, 0, 1)|
    |2D|Texture|_My2D ("My2D", 2D) = "white" {}|
    |Cube|Cubemap|_MyCube ("MyCube", Cube) = "white" {}|
    |3D|Volume|_My3D ("My3D", 3D) = "white" {}|
    </details>
+ 
    <details><summary>SubShader - 子着色器。</summary>



    + 
        <details><summary>Tags - 着色器的标签。写在SubShader里则影响所有Pass</summary>

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
        <details><summary>命令</summary>

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
            | Off|（默认）关闭混合。|
            | Zero | 零。所有颜色分量都为0。 | 
            | One | 一。所有颜色分量都为1。 | 
            | DstColor | 目标颜色。使用当前帧缓冲区中的颜色值。 | 
            | SrcColor | 源颜色。使用当前绘制的片段颜色值。 | 
            | OneMinusDstColor | 1-目标颜色。使用1减去当前帧缓冲区中的颜色值。 | 
            | OneMinusSrcColor | 1-源颜色。使用1减去当前绘制的片段颜色值。 |
            | OneMinusDstAlpha | 1-目标Alpha。使用1减去当前帧缓冲区中的Alpha值。 | 
            | OneMinusSrcAlpha | 1-源Alpha。使用1减去当前绘制的片段Alpha值。 |
            | DstAlpha | 目标Alpha。使用当前帧缓冲区中的Alpha值。 | 
            | SrcAlpha | 源Alpha。使用当前绘制的片段Alpha值。 | 
           
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

            </details>

        + ZWrite  On\Off - 是否写入深度缓冲区。通常用于透明物体，以避免深度缓冲区的更新，从而防止透明物体遮挡后面的物体。
        + Lighting  On\Off - 是否开启光照模式。对于不需要光照效果的Shader（如Unlit Shader），可以关闭光照计算以提高性能。
        + Name  "名称" - 子着色器的名称。可以帮助开发者识别和区分不同的子着色器。
        + Dependency "名称" "shader路径" - 依赖项。标签用于指定当前着色器依赖的其他资源或着色器。
        + Fallback "Diffuse名称" -备用着色器。
        + Stencil - 模板缓冲区。Ref Comp Pass Fail ZFail。Ref是模板缓冲区的参考值，Comp是模板缓冲区的比较函数，Pass是模板缓冲区的通过操作，Fail是模板缓冲区的失败操作，ZFail是模板缓冲区的深度失败操作。


        </details>

    + 
        <details><summary>Pass - 渲染通道</summary>
       
        + Pass - 一个渲染通道。
        + UsePass "PassName" - 使用一个已经定义的Pass。
        + GrabPass - 用于在渲染管线中捕获屏幕内容。通常用于后处理效果。
        </details>

    + 
        <details><summary>代码块关键字</summary>

        + CGPROGRAM 和 ENDCG 之间的代码块是用 Cg 或 HLSL 编写的着色器代码。
        + HLSLPROGRAM 和 ENDHLSL 之间的代码块是用 HLSL 编写的着色器代码。
        + GLSLPROGRAM 和 ENDGLSL 之间的代码块是用 GLSL 编写的着色器代码。
        </details>

    + 
        <details><summary>pragma - 编译器指令</summary>

        + #pragma vertex vert - 指定顶点着色器。
        + #pragma fragment frag - 指定片段着色器。
        + #pragma surface - 表面着色器。用于指定表面着色器。

        </details>

    + 
        <details><summary>语义变量</summary>
        
        + 在 ShaderLab 中，VS 是指顶点着色器（Vertex Shader）。

        |语义 | 说明 |
        |----|----|
        |POSITION | 顶点位置。常用于接收模型的顶点位置数据。 |
        |NORMAL | 顶点法线。 |
        |COLOR | 顶点颜色。 |
        |TEXCOORD0~7 | 纹理坐标。 |
        |BINORMAL | 顶点的副法线。 |
        |TANGENT | 顶点的切线。 |
        |SV_Position| 表示顶点的屏幕空间位置，通常用于顶点着色器的输出。 |
        |SV_Target| 表示像素着色器的输出颜色。 |
        |SV_VertexID| 表示当前顶点的索引。 |
        |SV_InstanceID| 表示当前实例的索引。 |
        |SV_PrimitiveID| 表示当前图元的索引。 |
        |SV_Depth| 表示像素的深度值。 |
        |SV_Coverage| 表示多重采样抗锯齿（MSAA）覆盖掩码。 |
        |SV_ClipDistance| 表示裁剪距离。 |
        |SV_CullDistance| 表示剔除距离。 |
        |SV_RenderTargetArrayIndex| 表示渲染目标数组的索引。 |
        |SV_ViewportArrayIndex| 表示视口数组的索引。 |
        |SV_SampleIndex| 表示当前采样的索引。 |
        |SV_IsFrontFace| 表示当前像素是否位于正面。 |
        |SV_GroupID| 表示当前计算着色器工作组的索引。 |
        |SV_DispatchThreadID| 表示当前计算着色器线程的全局索引。 |
        |SV_GroupIndex| 表示当前计算着色器线程在其工作组中的索引。 |
        |SV_GroupThreadID| 表示当前计算着色器线程在其工作组中的索引。 |

        </details>


    </details>

+ 
    <details><summary> 变量 </summary>

    |名称|类型|值|
    |----|----|----|
    |_WorldSpaceCameraPos|float3|世界空间中的相机位置。|
    |_ProjectionParams|float4|投影参数。|
    |_ScreenParams|float4|屏幕参数。|
    |_ZBufferParams|float4|Z缓冲区参数。|
    |unity_OrthoParams|float4|正交参数。|
    |unity_CameraProjection|float4x4|相机投影矩阵。|
    |unity_CameraInvProjection|float4x4|相机投影矩阵的逆。|
    |unity_CameraProjectionParams[6]|float4|相机投影参数。|
    |UNITY_MATRIX_MVP|float4x4|模型视图投影矩阵。|
    |UNITY_MATRIX_MV|float4x4|模型视图矩阵。|
    |UNITY_MATRIX_V|float4x4|视图矩阵。|
    |UNITY_MATRIX_P|float4x4|投影矩阵。|
    |UNITY_MATRIX_VP|float4x4|视图投影矩阵。|
    |UNITY_MATRIX_T_MV|float4x4|模型视图矩阵的转置。|
    |UNITY_MATRIX_IT_MV|float4x4|模型视图矩阵的逆转置。|
    |unity_ObjectToWorld|float4x4|对象到世界矩阵。|
    |unity_WorldToObject|float4x4|世界到对象矩阵。|
    |_Time|float4|当前时间。x:时间/20，y:时间，z:时间x2，w:时间x3。|
    |_SinTime|float4|当前时间的正弦值。x:sin(时间)，y:sin(时间/20)，z:sin(时间/200)，w:sin(时间x2)。|
    |_CosTime|float4|当前时间的余弦值。x:cos(时间)，y:cos(时间/20)，z:cos(时间/200)，w:cos(时间x2)。|
    |_DeltaTime|float4|上一帧的时间间隔。x:帧间隔时间，y:帧间隔时间/20，z:帧间隔时间/200，w:帧间隔时间x2。|
    </details>



    




