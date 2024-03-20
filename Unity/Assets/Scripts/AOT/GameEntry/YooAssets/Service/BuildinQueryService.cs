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
	/// 内置查询服务
	/// </summary>
	public class BuildinQueryService : IBuildinQueryServices
	{
		public static bool CompareFileCRC = false;

		public bool Query(string packageName, string fileName, string fileCRC)
		{
			// 检查文件是否存在
			return FileExists(packageName, fileName, fileCRC);
		}

		public static bool FileExists(string packageName, string fileName, string fileCRC)
		{
			string filePath = Path.Combine(Application.streamingAssetsPath, YooAssetsHelper.RootFolderName, packageName, fileName);
			if (File.Exists(filePath))
			{
				if (CompareFileCRC)
				{
					string crc32 = YooAsset.Editor.EditorTools.GetFileCRC32(filePath);
					return crc32 == fileCRC;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}
	}
}