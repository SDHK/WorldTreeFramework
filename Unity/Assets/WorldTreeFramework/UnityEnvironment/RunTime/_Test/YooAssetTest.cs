using System.IO;
using UnityEngine;
using YooAsset;

namespace WorldTree
{
	public class YooAssetTest : Node, ComponentOf<InitialDomain>
	, AsRule<IAwakeRule>
	{
	}

	public static class YooAssetTestRule
	{
		class _AddRule : AddRule<InitialDomain>
		{
			protected override void OnEvent(InitialDomain self)
			{

				//                // 初始化资源系统
				//                YooAssets.Initialize();

				//                // 创建默认的资源包
				//                var package = YooAssets.CreatePackage("DefaultPackage");
				//                YooAssets.SetDefaultPackage(package);

				//#if UNITY_EDITOR //编辑器模式
				//                var initParameters = new EditorSimulateModeParameters();
				//                initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
				//#else //非编辑器模式
				//                //var initParameters = new OfflinePlayModeParameters();
				//#endif
				//                var res = await self.GetAwaiter(package.InitializeAsync(initParameters));


				//                package = YooAssets.GetPackage("DefaultPackage");
				//                AssetOperationHandle handle = await self.GetAwaiter(package.LoadAssetAsync<GameObject>("MainWindow"));

				//                GameObject go = handle.InstantiateSync();
			}
		}

		class AddRule : AddRule<YooAssetTest>
		{
			protected override void OnEvent(YooAssetTest self)
			{
				//World.Log("初始域启动2！！");

				//// 初始化资源系统
				//YooAssets.Initialize();
				//// 创建默认的资源包
				//ResourcePackage package = YooAssets.CreatePackage("DefaultPackage");
				//YooAssets.SetDefaultPackage(package);

				//await self.AsyncDelay(1);

				//await NetInitializeYooAsset(self, package);

				//await UpdatePack(self, package);

				//await UpdatePack1(self, package);


				//await Download(self);
				//World.Log("初始域启动2完成！！");
			}

			//初始化
			public async TreeTask NetInitializeYooAsset(INode self, ResourcePackage package)
			{
				string netPath = "http://192.168.31.157/FTP/2023-07-13-1169";

				var initParameters = new HostPlayModeParameters();

				initParameters.QueryServices = new QueryStreamingAssetsFileServices();

				//主资源服务器地址
				initParameters.DefaultHostServer = netPath;
				//备用资源服务器地址
				initParameters.FallbackHostServer = netPath;

				await self.GetAwaiter(package.InitializeAsync(initParameters));
			}

			// 内置文件查询服务类，这个类只需要返回ApplicationstreamingAsset下面的文件存在性就好
			private class QueryStreamingAssetsFileServices : IQueryServices
			{
				public bool QueryStreamingAssets(string fileName)
				{
					//StreamingAssetsHelper.cs是太空战机里提供的一个查询脚本。
					string buildinFolderName = YooAssets.GetStreamingAssetBuildinFolderName();
					return File.Exists($"{Application.streamingAssetsPath}/{buildinFolderName}/{fileName}");
				}
			}
			string packageVersion = "2023-4-20-1108";

			public async TreeTask UpdatePack(INode self, ResourcePackage package)
			{
				//2.获取资源版本
				var operation = package.UpdatePackageVersionAsync();
				await self.GetAwaiter(operation);
				if (operation.Status != EOperationStatus.Succeed)
				{
					self.LogError("版本号更新失败，可能是找不到服务器");
					return;
				}
				//这是获取到的版本号，在下一个步骤要用
				packageVersion = operation.PackageVersion;
			}


			public async TreeTask UpdatePack1(INode self, ResourcePackage package)
			{
				//3.获取补丁清单
				var op = package.UpdatePackageManifestAsync(packageVersion);
				await self.GetAwaiter(op);
				if (op.Status != EOperationStatus.Succeed)
				{
					self.LogError("Mainfest更新失败！");
				}
			}


			int downloadingMaxNum = 10;
			int failedTryAgain = 3;
			int timeout = 60;

			public async TreeTask Download(INode self)
			{
				self.Log("下载!!!");

				var package = YooAssets.GetPackage("DefaultPackage");
				var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain, timeout);
				//下载数量是0，直接就完成了
				if (downloader.TotalDownloadCount == 0)
				{
					self.Log("下载数量是0，直接就完成了");

					await self.TreeTaskCompleted();
					return;
				}

				//注册一些回调
				//downloader.OnDownloadErrorCallback += OnError;
				//downloader.OnDownloadProgressCallback += OnProcess;
				//downloader.OnDownloadOverCallback += OnOver;
				//downloader.OnStartDownloadFileCallback += OnStartDownOne;

				//开始下载
				self.Log("开始下载???");

				downloader.BeginDownload();
				//等待下载完成
				await self.GetAwaiter(downloader);
				self.Log("等待下载完成???");

				await self.TreeTaskCompleted();

				//检查状态
				if (downloader.Status == EOperationStatus.Succeed)
				{
					self.Log("下载完成");

					AssetOperationHandle handle = await self.GetAwaiter(package.LoadAssetAsync<GameObject>("MainWindow"));

					GameObject go = handle.InstantiateSync();
				}
				else
				{
					self.Log("下载失败");
				}
			}
		}
	}
}
