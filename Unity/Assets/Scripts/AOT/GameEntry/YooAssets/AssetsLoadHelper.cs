/****************************************

* 作者：闪电黑客
* 日期：2024/3/20 18:00

* 描述：资源下载流程

*/

using System;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace WorldTree.AOT
{
	public static partial class YooAssetsHelper
	{
		/// <summary>
		/// 初始化资源包
		/// </summary>
		public static async void InitializePackage()
		{
			Debug.Log("初始化资源包！");
			InitializationOperation initialization = Initialize(GamePlayMode.OfflinePlayMode);
			await initialization.Task;

			// 如果初始化失败弹出提示界面
			if (initialization.Status != EOperationStatus.Succeed)
			{
				Debug.LogWarning($"{initialization.Error}");

				//初始化资源包失败事件
			}
			else
			{
				var version = initialization.PackageVersion;
				Debug.Log($"Init资源包版本 : {version}");
				UpdatePackageVersion();
			}
		}

		/// <summary>
		/// 更新资源版本号
		/// </summary>
		public static async void UpdatePackageVersion()
		{
			await Task.Delay(500);
			ResourcePackage package = YooAssets.GetPackage(packageName);
			UpdatePackageVersionOperation operation = package.UpdatePackageVersionAsync();
			await operation.Task;
			if (operation.Status != EOperationStatus.Succeed)
			{
				Debug.LogWarning(operation.Error);

				//资源版本号更新失败事件
			}
			else
			{
				GameEntry.instance.packageVersion = operation.PackageVersion;
				UpdatePackageManifest();
			}
		}

		/// <summary>
		/// 更新资源清单
		/// </summary>
		public static async void UpdatePackageManifest()
		{
			await Task.Delay(500);
			ResourcePackage package = YooAssets.GetPackage(packageName);
			bool savePackageVersion = true;
			UpdatePackageManifestOperation operation = package.UpdatePackageManifestAsync(GameEntry.instance.packageVersion, savePackageVersion);
			await operation.Task;
			if (operation.Status != EOperationStatus.Succeed)
			{
				Console.WriteLine(operation.Error);

				//资源清单更新失败事件
			}
			else
			{
				CreatePackageDownloader();
			}
		}

		/// <summary>
		/// 创建资源包下载器
		/// </summary>
		public static async void CreatePackageDownloader()
		{
			await Task.Delay(500);
			ResourcePackage package = YooAssets.GetPackage(YooAssetsHelper.packageName);
			int downloadingMaxNum = 10;
			int failedTryAgain = 3;
			ResourceDownloaderOperation downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

			await downloader.Task;
			if (downloader.TotalDownloadCount == 0)
			{
				Debug.Log("没有找到任何下载文件!");

				//创建补丁包下载器失败事件
			}
			else
			{
				// 发现新更新文件后，挂起流程系统

				// 注意：开发者需要在下载前检测磁盘空间不足
				int totalDownloadCount = downloader.TotalDownloadCount;
				long totalDownloadBytes = downloader.TotalDownloadBytes;
				Debug.Log($"发现新更新文件! 总共{totalDownloadCount}个文件，总共{totalDownloadBytes}字节");

				downloader.OnDownloadErrorCallback += (fileName, error) =>
				{
					Debug.Log($"下载失败! {fileName}: {error}");
				};

				downloader.OnDownloadProgressCallback += (totalDownloadCount, currentDownloadCount, totalDownloadSizeBytes, currentDownloadSizeBytes) =>
				{
					//Debug.Log($"下载进度! {fileName}: {progress}");
				};

				// 开始下载
				downloader.BeginDownload();

				await downloader.Task;

				if (downloader.Status != EOperationStatus.Succeed)
				{
					Debug.Log($"下载失败! {downloader.Error}");

					//下载失败事件
				}
				else
				{
					Debug.Log("下载成功!");
					ClearPackageCache();
				}
			}
		}

		/// <summary>
		/// 清理资源包缓存
		/// </summary>
		public static async void ClearPackageCache()
		{
			var package = YooAssets.GetPackage(packageName);
			var operation = package.ClearUnusedCacheFilesAsync();
			await operation.Task;
			if (operation.Status != EOperationStatus.Succeed)
			{
				Debug.LogWarning(operation.Error);
			}
			else
			{
				Debug.Log("清理资源包缓存成功!");
			}
		}
	}
}