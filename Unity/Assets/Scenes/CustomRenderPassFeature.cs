using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderPassFeature : ScriptableRendererFeature
{
	TestRenderPass testRenderPass;

	CustomRenderShaderPostPass m_ScriptablePassCreateMesh;

	CustomRenderPassCreateMesh createMesh;

	public Material material;
	public Shader shaderMesh;

	public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;


	public override void Create()
	{
		testRenderPass = new TestRenderPass(RenderPassEvent.BeforeRenderingPostProcessing);


		m_ScriptablePassCreateMesh = new CustomRenderShaderPostPass();
		m_ScriptablePassCreateMesh._Material = new Material(shaderMesh);
		m_ScriptablePassCreateMesh.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

		createMesh = new CustomRenderPassCreateMesh();
		createMesh._Material = new Material(shaderMesh);
		createMesh.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
	}

	//在这里，您可以在渲染器中注入一个或多个渲染通道。
	//当为每个摄像机设置一次渲染器时，将调用此方法。
	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (material == null) return;

		testRenderPass.material = this.material;
		testRenderPass.Setup(renderer);


		renderer.EnqueuePass(testRenderPass);
		renderer.EnqueuePass(m_ScriptablePassCreateMesh);
		renderer.EnqueuePass(createMesh);
	}
}

public class TestRenderPass : ScriptableRenderPass
{
	private ScriptableRenderer currentTarget;
	private TestVolume volume;
	public Material material;
	public TestRenderPass(RenderPassEvent evt)
	{
		renderPassEvent = evt;
	}
	public void Setup(ScriptableRenderer currentTarget)
	{
		this.currentTarget = currentTarget;
	}
	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		// 如果当前相机没有启用后处理效果，则直接返回
		if (!renderingData.cameraData.postProcessEnabled) return;


		// 获取当前的 Volume 堆栈
		var stack = VolumeManager.instance.stack;

		// 从堆栈中获取 TestVolume 组件
		volume = stack.GetComponent<TestVolume>();
		// 如果没有找到 TestVolume 组件，或者组件未激活，则直接返回
		if (volume == null) return;
		if (!volume.IsActive()) return;

		// 设置材质的参数
		material.SetFloat("_Offs", volume.offset.value);

		// 创建一个命令缓冲区，用于存储渲染命令
		CommandBuffer cmd = CommandBufferPool.Get("TestRenderPass");

		// 获取当前相机的颜色渲染目标
		var source = currentTarget.cameraColorTarget;
		// 创建一个临时纹理的 ID
		int temTextureID = Shader.PropertyToID("_TestTex");

		// 获取当前相机的渲染目标描述符
		RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
		// 分配一个临时渲染目标，使用前面获取的描述符
		cmd.GetTemporaryRT(temTextureID, descriptor);

		// 设置材质的 _Offs 属性为 0.5

		// 将 source 渲染目标的内容拷贝到 temTextureID，并应用材质的第一个 pass（索引为 0）
		cmd.Blit(source, temTextureID, material, 0);
		// 将 temTextureID 的内容拷贝回 source 渲染目标
		cmd.Blit(temTextureID, source);

		// 执行命令缓冲区中的所有命令
		context.ExecuteCommandBuffer(cmd);
		// 释放命令缓冲区
		CommandBufferPool.Release(cmd);
	}
}

public class TestVolume : VolumeComponent, IPostProcessComponent
{

	public FloatParameter offset = new FloatParameter(0.1f);

	public bool IsActive()
	{
		//return material.value != null;
		return true;
	}

	public bool IsTileCompatible()
	{
		return false;
	}

}

/// <summary>
/// 后处理效果
/// </summary>
class CustomRenderShaderPostPass : ScriptableRenderPass
{
	//定义渲染材质，通过Create方法赋值
	public Material _Material;


	public void SetMaterial(Material material)
	{
		_Material = material;
	}

	//在执行渲染通道之前调用此方法。
	//它可用于配置渲染目标及其清除状态。此外，还可以创建临时渲染目标纹理。
	//如果为空，则此渲染通道将渲染到活动的摄像机渲染目标。
	//切勿调用 Command Buffer.Set Render Target。请改为调用 <c>Configure Target</c> 和 <c>Configure Clear</c>。
	//渲染管道将确保以高性能方式进行目标设置和清除。
	public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
	{
	}


	// 在这里，您可以实现渲染逻辑。
	// 使用 <c>Scriptable Render Context</c> 发出绘制命令或执行命令缓冲区
	// https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
	// 您不必调用 Scriptable Render Context.submit，渲染管线将在管线中的特定点调用它。

	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		//创建一个CommandBuffer
		CommandBuffer cmd = CommandBufferPool.Get("TestRenderShader");

		//设置渲染目标为相机的颜色缓冲区
		cmd.Blit(colorAttachment, RenderTargetHandle.CameraTarget.Identifier(), _Material);
		//执行CommandBuffer
		context.ExecuteCommandBuffer(cmd);
		////回收CommandBuffer
		//CommandBufferPool.Release(cmd);
		cmd.Clear();
	}

	// 清理在执行此渲染通道期间创建的任何已分配资源。
	public override void OnCameraCleanup(CommandBuffer cmd)
	{
	}
}


//绘制网格
class CustomRenderPassCreateMesh : ScriptableRenderPass
{
	public Material _Material;
	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		CommandBuffer cmd = CommandBufferPool.Get("CreateMesh");
		cmd.DrawMesh(CreateMesh(), Matrix4x4.identity, _Material);
		//这里和后处理一样的操作
		context.ExecuteCommandBuffer(cmd);
		CommandBufferPool.Release(cmd);
	}
	//创建网格
	Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[4] { new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1) };
		int[] indices = new int[8] { 0, 1, 1, 2, 2, 3, 3, 0 };
		//创建简单的线网格
		mesh.SetIndices(indices, MeshTopology.Lines, 0);
		return mesh;
	}
}
