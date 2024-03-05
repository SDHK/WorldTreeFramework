using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WorldTree;
using YooAsset;

public class GameEntry : MonoBehaviour
{
	public ResourcePackage package;

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
		package = YooAssets.CreatePackage("DefaultPackage");
		// 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
		YooAssets.SetDefaultPackage(package);
		yield return SingleInitializeYooAsset(package);

		AssetHandle assetHandle = package.LoadAssetAsync<GameObject>("MainWindow");
		yield return assetHandle;

		Debug.Log($"加载物体 : {assetHandle.AssetObject.name}");
	}

	private IEnumerator LoadAOT()
	{
		//AOT
		foreach (string address in GetAddressesByTag("aotDlls"))
		{
			Debug.Log($"AOT:{address}");

			AssetHandle handle = package.LoadAssetAsync<TextAsset>(address);
			yield return handle;
			RuntimeApi.LoadMetadataForAOTAssembly((handle.AssetObject as TextAsset).bytes, HomologousImageMode.SuperSet);
		}
	}

	private IEnumerator LoadHotUpdate()
	{
		Dictionary<string, Assembly> assemblys = new();

		foreach (string address in GetAddressesByTag("hotUpdateDlls"))
		{
			AssetHandle handle = package.LoadAssetAsync<TextAsset>(address);
			yield return handle;
			Debug.Log($"HotUpdate {address}:{(handle.AssetObject as TextAsset).bytes.Length}");

			Assembly assembly = Assembly.Load((handle.AssetObject as TextAsset).bytes);
			assemblys.Add(address, assembly);
		}

		//if (assemblys.TryGetValue("WorldTree.CoreUnity.dll", out Assembly assembly1))
		//{
		//	Type type = assembly1.GetType("WorldTree.UnityWorldTree");
		//	Component component = gameObject.AddComponent(type);
		//	//反射设置字段
		//	component.GetType().GetField("assemblies").SetValue(component, assemblys.Values.ToArray());
		//	component.GetType().GetMethod("Start1").Invoke(component, null);
		//}
		UnityWorldTree unityWorldTree = gameObject.AddComponent<UnityWorldTree>();
		unityWorldTree.Start1();
	}

	private IEnumerator SingleInitializeYooAsset(ResourcePackage package)
	{
		var initParameters = new OfflinePlayModeParameters();
		yield return package.InitializeAsync(initParameters);
	}

	public string[] GetAddressesByTag(string tag)
	{
		AssetInfo[] assetInfos = YooAssets.GetAssetInfos(tag);
		string[] addresses = new string[assetInfos.Length];
		for (int i = 0; i < assetInfos.Length; i++)
		{
			addresses[i] = assetInfos[i].Address;
		}

		return addresses;
	}
}