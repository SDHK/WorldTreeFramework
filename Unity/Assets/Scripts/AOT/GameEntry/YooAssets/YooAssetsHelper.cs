/****************************************

* 作者：闪电黑客
* 日期：2024/3/19 17:42

* 描述：

*/

using YooAsset;

namespace WorldTree.AOT
{
	/// <summary>
	/// 资源加载辅助类
	/// </summary>
	public static partial class YooAssetsHelper
	{
		/// <summary>
		/// 根目录名称（保持和YooAssets资源系统一致）
		/// </summary>
		public const string RootFolderName = "yoo";

		/// <summary>
		/// 默认资源包名称
		/// </summary>
		public static string packageName = "DefaultPackage";

		/// <summary>
		/// 资源包
		/// </summary>
		public static ResourcePackage package;

		/// <summary>
		/// 初始化
		/// </summary>
		public static InitializationOperation Initialize(GamePlayMode playMode)
		{
			package = PackageInitialize();

			InitializationOperation initialization = null;

			//设置操作系统最大时间片
			YooAssets.SetOperationSystemMaxTimeSlice(30);
			switch (playMode)
			{
				case GamePlayMode.OfflinePlayMode:
					initialization = SingleInitialize(package);
					break;

				case GamePlayMode.EditorSimulateMode:
					initialization = EditorInitialize(package);
					break;

				case GamePlayMode.NetPlayMode:
					initialization = NetInitialize(package);
					break;

				case GamePlayMode.WebPlayMode:
					initialization = WebInitialize(package);
					break;
			}
			return initialization;
		}
	}
}