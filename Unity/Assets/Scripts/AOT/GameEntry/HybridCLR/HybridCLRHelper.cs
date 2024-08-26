/****************************************

* 作者： 闪电黑客
* 日期： 2024/3/19 17:43

* 描述：

*/

using HybridCLR;
using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace WorldTree.AOT
{
	/// <summary>
	/// HybridCLR辅助类
	/// </summary>
	public static class HybridCLRHelper
	{
		/// <summary>
		/// 是否运行模式
		/// </summary>
#if UNITY_EDITOR
		public static bool IsRun = false;
#else
		public static bool IsRun = true;

#endif

		/// <summary>
		/// 加载AOT
		/// </summary>
		public static async Task LoadAOT()
		{
			if (IsRun)
			{
				ResourcePackage package = YooAssets.GetPackage("DefaultPackage");

				foreach (AssetInfo assetInfo in YooAssets.GetAssetInfos("aotDlls"))
				{
					AssetHandle handle = package.LoadAssetAsync<TextAsset>(assetInfo.Address);
					await handle.Task;
					TextAsset textAsset = (handle.AssetObject as TextAsset);
					Debug.Log($"AOT加载:{assetInfo.Address} : {textAsset.bytes.Length}");
					RuntimeApi.LoadMetadataForAOTAssembly(textAsset.bytes, HomologousImageMode.SuperSet);
				}
			}

			await Task.CompletedTask;
		}

		/// <summary>
		/// 加载热更
		/// </summary>
		public static async Task LoadHotUpdate()
		{
			if (IsRun)
			{
				ResourcePackage package = YooAssets.GetPackage("DefaultPackage");

				foreach (AssetInfo assetInfo in YooAssets.GetAssetInfos("hotUpdateDlls"))
				{
					AssetHandle handle = package.LoadAssetAsync<TextAsset>(assetInfo.Address);
					await handle.Task;
					TextAsset textAsset = (handle.AssetObject as TextAsset);
					Debug.Log($"HotUpdate加载 {assetInfo.Address}:{textAsset.bytes.Length}");
					Assembly.Load(textAsset.bytes);
				}
			}

			await Task.CompletedTask;
		}
	}
}