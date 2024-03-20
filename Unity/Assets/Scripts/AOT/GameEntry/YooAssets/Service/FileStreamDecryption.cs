/****************************************

* 作者： 闪电黑客
* 日期： 2024/3/19 20:30

* 描述：

*/

using System.IO;
using UnityEngine;
using YooAsset;

namespace WorldTree.AOT
{
	/// <summary>
	/// 资源文件流加载解密类
	/// </summary>
	public class FileStreamDecryption : IDecryptionServices
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
}