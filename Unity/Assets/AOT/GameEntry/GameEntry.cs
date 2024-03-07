using HybridCLR;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YooAsset;

public class GameEntry : MonoBehaviour
{
	public string netPath = "http://127.0.0.1:9999/FTP/2023-04-20-1108";

	private ResourcePackage package;

	private void Start()
	{
		StartCoroutine(StartLoad());
	}

	private IEnumerator StartLoad()
	{
		package = InitializeYooAsset();
#if UNITY_EDITOR
		yield return EditorYooAsset(package);
#else
		yield return SingleYooAsset(package);
		yield return LoadAOT(package);
		yield return LoadHotUpdate(package);
#endif
		StartWorldTree();
	}

	/// <summary>
	/// 启动框架
	/// </summary>
	private void StartWorldTree()
	{
		Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "WorldTree.CoreUnity");
		gameObject.AddComponent(assembly.GetType("WorldTree.UnityWorldTree"));
	}

	/// <summary>
	/// 初始化资源系统
	/// </summary>
	private ResourcePackage InitializeYooAsset()
	{
		// 初始化资源系统
		YooAssets.Initialize();

		// 创建默认的资源包
		ResourcePackage package = YooAssets.CreatePackage("DefaultPackage");

		// 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
		YooAssets.SetDefaultPackage(package);
		return package;
	}

	#region 编辑器

	/// <summary>
	/// 编辑器模拟模式
	/// </summary>
	private IEnumerator EditorYooAsset(ResourcePackage package)
	{
		EditorSimulateModeParameters initParameters = new();
		initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage");
		yield return package.InitializeAsync(initParameters);
	}

	#endregion

	#region 单机

	/// <summary>
	/// 单机模式
	/// </summary>
	private IEnumerator SingleYooAsset(ResourcePackage package)
	{
		yield return package.InitializeAsync(new OfflinePlayModeParameters());
	}

	#endregion

	#region 网络

	private IEnumerator NetInitializeYooAsset(ResourcePackage package)
	{
		HostPlayModeParameters initParameters = new HostPlayModeParameters();
		initParameters.BuildinQueryServices = new BuildinQueryService();
		initParameters.RemoteServices = new RemoteServices(netPath);

		//initParameters.DeliveryQueryServices = new DeliveryQueryService();
		InitializationOperation initOperation = package.InitializeAsync(initParameters);
		yield return initOperation;
		if (initOperation.Status == EOperationStatus.Succeed)
		{
			Debug.Log("资源包初始化成功！");
		}
		else
		{
			Debug.LogError($"资源包初始化失败：{initOperation.Error}");
		}
	}

	/// <summary>
	/// 内置查询服务
	/// </summary>
	private class BuildinQueryService : IBuildinQueryServices
	{
		public bool Query(string packageName, string fileName, string fileCRC)
		{
			// 获取StreamingAssets文件夹的路径
			string path = Path.Combine(Application.streamingAssetsPath, packageName, fileName);

			// 检查文件是否存在
			return File.Exists(path);
		}
	}

	/// <summary>
	/// 配送查询服务
	/// </summary>
	private class DeliveryQueryService : IDeliveryQueryServices
	{
		public string GetFilePath(string packageName, string fileName)
		{
			throw new NotImplementedException();
		}

		public bool Query(string packageName, string fileName, string fileCRC)
		{
			throw new NotImplementedException();
		}
	}

	private class RemoteServices : IRemoteServices
	{
		private string netPath;

		public RemoteServices(string defaultHostServer)
		{
			netPath = defaultHostServer;
		}

		public string GetRemoteFallbackURL(string fileName)
		{
			throw new NotImplementedException();
		}

		public string GetRemoteMainURL(string fileName)
		{
			throw new NotImplementedException();
		}
	}

	#endregion

	#region DLL加载

	/// <summary>
	/// 加载AOT
	/// </summary>
	private IEnumerator LoadAOT(ResourcePackage package)
	{
		foreach (AssetInfo assetInfo in YooAssets.GetAssetInfos("aotDlls"))
		{
			AssetHandle handle = package.LoadAssetAsync<TextAsset>(assetInfo.Address);
			yield return handle;
			TextAsset textAsset = (handle.AssetObject as TextAsset);
			Debug.Log($"AOT加载:{assetInfo.Address} : {textAsset.bytes.Length}");
			RuntimeApi.LoadMetadataForAOTAssembly(textAsset.bytes, HomologousImageMode.SuperSet);
		}
	}

	/// <summary>
	/// 加载热更
	/// </summary>
	private IEnumerator LoadHotUpdate(ResourcePackage package)
	{
		foreach (AssetInfo assetInfo in YooAssets.GetAssetInfos("hotUpdateDlls"))
		{
			AssetHandle handle = package.LoadAssetAsync<TextAsset>(assetInfo.Address);
			yield return handle;
			TextAsset textAsset = (handle.AssetObject as TextAsset);
			Debug.Log($"HotUpdate加载 {assetInfo.Address}:{textAsset.bytes.Length}");
			Assembly.Load(textAsset.bytes);
		}
	}

	#endregion
}