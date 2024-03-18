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

	/// <summary>
	/// 根目录名称（保持和YooAssets资源系统一致）
	/// </summary>
	public const string RootFolderName = "yoo";

	/// <summary>
	/// 查询内置文件的时候，是否比对文件哈希值
	/// </summary>
	public static bool CompareFileCRC = false;

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
		string defaultHostServer = GetHostServerURL();
		string fallbackHostServer = GetHostServerURL();
		HostPlayModeParameters initParameters = new HostPlayModeParameters();
		initParameters.BuildinQueryServices = new BuildinQueryService();
		initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);

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
		public static bool CompareFileCRC = false;

		public bool Query(string packageName, string fileName, string fileCRC)
		{
			// 检查文件是否存在
			return FileExists(packageName, fileName, fileCRC);
		}

		public static bool FileExists(string packageName, string fileName, string fileCRC)
		{
			string filePath = Path.Combine(Application.streamingAssetsPath, RootFolderName, packageName, fileName);
			if (File.Exists(filePath))
			{
				if (BuildinQueryService.CompareFileCRC)
				{
					string crc32 = YooAsset.Editor.EditorTools.GetFileCRC32(filePath);
					return crc32 == fileCRC;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}
	}

	/// <summary>
	/// 远端资源地址查询服务类
	/// </summary>
	private class RemoteServices : IRemoteServices
	{
		private readonly string _defaultHostServer;
		private readonly string _fallbackHostServer;

		public RemoteServices(string defaultHostServer, string fallbackHostServer)
		{
			_defaultHostServer = defaultHostServer;
			_fallbackHostServer = fallbackHostServer;
		}

		public string GetRemoteFallbackURL(string fileName)
		{
			return $"{_defaultHostServer}/{fileName}";
		}

		public string GetRemoteMainURL(string fileName)
		{
			return $"{_fallbackHostServer}/{fileName}";
		}
	}

	/// <summary>
	/// 获取资源服务器地址
	/// </summary>
	private string GetHostServerURL()
	{
		//string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
		string hostServerIP = "http://127.0.0.1";
		string appVersion = "v1.0";

#if UNITY_EDITOR
		if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
			return $"{hostServerIP}/CDN/Android/{appVersion}";
		else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
			return $"{hostServerIP}/CDN/IPhone/{appVersion}";
		else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
			return $"{hostServerIP}/CDN/WebGL/{appVersion}";
		else
			return $"{hostServerIP}/CDN/PC/{appVersion}";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
	}

	/// <summary>
	/// 资源文件流加载解密类
	/// </summary>
	private class FileStreamDecryption : IDecryptionServices
	{
		/// <summary>
		/// 同步方式获取解密的资源包对象 注意：加载流对象在资源包对象释放的时候会自动释放
		/// </summary>
		AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
		{
			BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			managedStream = bundleStream;
			return AssetBundle.LoadFromStream(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
		}

		/// <summary>
		/// 异步方式获取解密的资源包对象 注意：加载流对象在资源包对象释放的时候会自动释放
		/// </summary>
		AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
		{
			BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			managedStream = bundleStream;
			return AssetBundle.LoadFromStreamAsync(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
		}

		private static uint GetManagedReadBufferSize()
		{
			return 1024;
		}
	}

	/// <summary>
	/// 资源文件解密流
	/// </summary>
	public class BundleStream : FileStream
	{
		public const byte KEY = 64;

		public BundleStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
		{
		}

		public BundleStream(string path, FileMode mode) : base(path, mode)
		{
		}

		public override int Read(byte[] array, int offset, int count)
		{
			var index = base.Read(array, offset, count);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] ^= KEY;
			}
			return index;
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