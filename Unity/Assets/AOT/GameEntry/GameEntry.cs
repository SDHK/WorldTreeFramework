using HybridCLR;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using YooAsset;

public class GameEntry : MonoBehaviour
{
	public ResourcePackage package;
	private OfflinePlayModeParameters initParameters = new OfflinePlayModeParameters();

	// Start is called before the first frame update
	private void Start()
	{
		StartCoroutine(Load());
	}

	private IEnumerator Load()
	{
		Debug.Log($"加载物体AB包！！！ ");

		yield return LoadDefaultPackage();
		yield return LoadAOT();
		yield return LoadHotUpdate();
	}

	private IEnumerator LoadDefaultPackage()
	{
		// 初始化资源系统
		YooAssets.Initialize();
		// 创建默认的资源包
		var package = YooAssets.CreatePackage("DefaultPackage");
		// 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
		YooAssets.SetDefaultPackage(package);

		InitializationOperation operation = package.InitializeAsync(initParameters);

		yield return operation;

		if (operation.Status == EOperationStatus.Succeed)
		{
			Debug.Log($"更新默认包的版本: {operation.PackageVersion}");
		}

		AssetHandle assetHandle = package.LoadAssetAsync<GameObject>("MainWindow");
		yield return assetHandle;

		Debug.Log($"加载物体 : {assetHandle.AssetObject.name}");
	}

	private IEnumerator LoadAOT()
	{
		Debug.Log($"AOT开始 ！！！");

		//AOT
		HomologousImageMode mode = HomologousImageMode.SuperSet;
		ResourcePackage package = YooAssets.CreatePackage("AotDlls");
		InitializationOperation operation = package.InitializeAsync(initParameters);
		yield return operation;
		if (operation.Status == EOperationStatus.Succeed)
		{
			Debug.Log($"更新默认包的版本: {operation.PackageVersion}");
			AllAssetsHandle handle = package.LoadAllAssetsAsync<TextAsset>("YooAsset.dll");
			yield return handle;

			foreach (TextAsset assetObject in handle.AllAssetObjects)
			{
				RuntimeApi.LoadMetadataForAOTAssembly(assetObject.bytes, mode);
			}

			Debug.Log($"AOT完成 ！！！");
		}
	}

	private IEnumerator LoadHotUpdate()
	{
		Debug.Log($"HotUpdate开始 ！！！");

		ResourcePackage package = YooAssets.CreatePackage("HotUpdateDlls");
		InitializationOperation operation = package.InitializeAsync(initParameters);
		if (operation.Status == EOperationStatus.Succeed)
		{
			Debug.Log($"更新默认包的版本: {operation.PackageVersion}");

			AllAssetsHandle handle = package.LoadAllAssetsAsync<TextAsset>("WorldTree.Node.dll");
			yield return handle;
			foreach (TextAsset assetObject in handle.AllAssetObjects)
			{
				Assembly assembly = Assembly.Load(assetObject.bytes);
				if (assetObject.name == "WorldTree.CoreUnity")
				{
					Type type = assembly.GetType("UnityWorldTree");
					this.gameObject.AddComponent(type);
				}
			}
			Debug.Log($"HotUpdate完成 ！！！");
		}
	}
}