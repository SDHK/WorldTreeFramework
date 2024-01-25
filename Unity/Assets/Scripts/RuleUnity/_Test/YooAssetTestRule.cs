using System.Collections;
using System.IO;
using UnityEngine;
using WorldTree;
using YooAsset;

namespace WorldTree
{
	public static partial class YooAssetTestRule
	{
		class AddRule : AddRule<YooAssetTest>
		{
			protected override void OnEvent(YooAssetTest self)
			{
				// 初始化资源系统
				YooAssets.Initialize();

				// 创建默认的资源包
				self.package = YooAssets.CreatePackage("DefaultPackage");

				// 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
				YooAssets.SetDefaultPackage(self.package);

				self.InitializeYooAsset().Continue();
			}
		}


		private static async TreeTask InitializeYooAsset(this YooAssetTest self)
		{
			var initParameters = new EditorSimulateModeParameters();
			var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage");
			initParameters.SimulateManifestFilePath = simulateManifestFilePath;
			InitializationOperation a = await self.GetAwaiter(self.package.InitializeAsync(initParameters));

			AssetHandle handle = await self.GetAwaiter(self.package.LoadAssetAsync<GameObject>("MainWindow"));

			GameObject gameObject = handle.AssetObject as GameObject;
			self.Log($"YooAsset ??? : {gameObject.name}");
		}

		/// <summary>
		/// 获取异步等待
		/// </summary>
		public static TreeTask<AssetHandle> GetAwaiter(this INode self, AssetHandle asyncOperation)
		{
			self.AddChild(out TreeTask<AssetHandle> asyncTask);
			asyncOperation.Completed += asyncTask.SetResult;
			return asyncTask;
		}

	}
}
