/****************************************

* 作者：闪电黑客
* 日期：2024/3/20 17:59

* 描述：资源初始化辅助类

*/

using YooAsset;
using System;
using UnityEngine;

namespace WorldTree.AOT
{
	public static partial class YooAssetsHelper
	{
		/// <summary>
		/// 资源服务器地址
		/// </summary>
		private static string hostServerIP = "http://127.0.0.1:9999";

		/// <summary>
		/// 资源包初始化
		/// </summary>
		private static ResourcePackage PackageInitialize()
		{
			// 初始化资源系统
			YooAssets.Initialize();

			// 创建默认的资源包
			ResourcePackage package = YooAssets.CreatePackage(packageName);

			// 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
			YooAssets.SetDefaultPackage(package);
			return package;
		}

		#region 编辑器

		/// <summary>
		/// 编辑器模拟模式初始化
		/// </summary>
		private static InitializationOperation EditorInitialize(ResourcePackage package)
		{
			EditorSimulateModeParameters initParameters = new();
			initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage");
			return package.InitializeAsync(initParameters);
		}

		#endregion

		#region 单机

		/// <summary>
		/// 单机模式初始化
		/// </summary>
		private static InitializationOperation SingleInitialize(ResourcePackage package)
		{
			return package.InitializeAsync(new OfflinePlayModeParameters());
		}

		#endregion

		#region 网络

		private static InitializationOperation NetInitialize(ResourcePackage package)
		{
			string defaultHostServer = GetHostServerURL();
			string fallbackHostServer = GetHostServerURL();
			HostPlayModeParameters initParameters = new HostPlayModeParameters();
			initParameters.BuildinQueryServices = new BuildinQueryService();
			initParameters.DecryptionServices = new FileStreamDecryption();
			initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
			return package.InitializeAsync(initParameters);
		}

		#endregion

		#region Web

		/// <summary>
		/// Web模式初始化
		/// </summary>
		private static InitializationOperation WebInitialize(ResourcePackage package)
		{
			string defaultHostServer = GetHostServerURL();
			string fallbackHostServer = GetHostServerURL();
			WebPlayModeParameters initParameters = new WebPlayModeParameters();
			initParameters.BuildinQueryServices = new BuildinQueryService();
			initParameters.DecryptionServices = new FileStreamDecryption();
			initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
			return package.InitializeAsync(initParameters);
		}

		#endregion

		/// <summary>
		/// 获取资源服务器地址
		/// </summary>
		public static string GetHostServerURL()
		{
#if UNITY_EDITOR
			if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
				return $"{hostServerIP}/FTP/Android";
			else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
				return $"{hostServerIP}/FTP/IPhone";
			else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
				return $"{hostServerIP}/FTP/WebGL";
			else
				return $"{hostServerIP}/FTP/PC";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/FTP/Android";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/FTP/IPhone";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostServerIP}/FTP/WebGL";
        else
            return $"{hostServerIP}/FTP/PC";
#endif
		}
	}
}