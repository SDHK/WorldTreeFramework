using System.Collections;
using System.IO;
using UnityEngine;
using WorldTree;
using YooAsset;

namespace WorldTree
{
	public static partial class YooAssetTestRule
	{
		/// <summary>
		/// 获取异步等待
		/// </summary>
		public static TreeTask<AssetHandle> GetAwaiter(this INode self, AssetHandle asyncOperation)
		{
			self.AddChild(out TreeTask<AssetHandle> asyncTask);
			asyncOperation.Completed += asyncTask.SetResult;
			return asyncTask;
		}


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

				self.InitializeYooAsset1().Continue();
			}
		}

		/// <summary>
		/// 编辑器模拟模式
		/// </summary>
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
		/// 单机运行模式
		/// </summary>
		private static async TreeTask InitializeYooAsset1(this YooAssetTest self)
		{
			var initParameters = new OfflinePlayModeParameters();
			InitializationOperation a = await self.GetAwaiter(self.package.InitializeAsync(initParameters));

			AssetHandle handle = await self.GetAwaiter(self.package.LoadAssetAsync<GameObject>("MainWindow"));
			GameObject gameObject = handle.AssetObject as GameObject;
			self.Log($"YooAsset ??? : {gameObject.name}");
		}

		private static async TreeTask InitializeYooAsset2(this YooAssetTest self)
		{
			// 注意：GameQueryServices.cs 太空战机的脚本类，详细见StreamingAssetsHelper.cs
			string defaultHostServer = "http://127.0.0.1/CDN/Android/v1.0";
			string fallbackHostServer = "http://127.0.0.1/CDN/Android/v1.0";
			var initParameters = new HostPlayModeParameters();
			//initParameters.BuildinQueryServices = new GameQueryServices();
			//initParameters.DecryptionServices = new FileOffsetDecryption();
			//initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
			var initOperation = self.package.InitializeAsync(initParameters);

			if (initOperation.Status == EOperationStatus.Succeed)
			{
				Debug.Log("资源包初始化成功！");
			}
			else
			{
				Debug.LogError($"资源包初始化失败：{initOperation.Error}");
			}

			await self.TreeTaskCompleted();

		}


	}

}
