/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System.IO;

namespace WorldTree
{
	/// <summary>
	/// 文件目录工具类
	/// </summary>
	public static partial class DirectoryHelper
	{
		/// <summary>
		/// 尝试创建文件所在的目录
		/// </summary>
		/// <param name="filePath">文件路径</param>
		public static bool TryCreateFilePath(string filePath)
		{
			return TryCreate(Path.GetDirectoryName(filePath));
		}

		/// <summary>
		/// 尝试创建文件夹
		/// </summary>
		public static bool TryCreate(string directory)
		{
			directory = Path.GetFullPath(directory);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 尝试删除文件夹及子目录
		/// </summary>
		public static void Delete(string directory)
		{
			directory = Path.GetFullPath(directory);
			if (Directory.Exists(directory)) Directory.Delete(directory, true);
		}

		/// <summary>
		/// 清空文件夹
		/// </summary>
		public static void Clear(string directory)
		{
			directory = Path.GetFullPath(directory);

			if (Directory.Exists(directory) == false) return;

			// 删除文件
			string[] allFiles = Directory.GetFiles(directory);
			for (int i = 0; i < allFiles.Length; i++)
			{
				File.Delete(allFiles[i]);
			}

			// 删除文件夹
			string[] allFolders = Directory.GetDirectories(directory);
			for (int i = 0; i < allFolders.Length; i++)
			{
				Directory.Delete(allFolders[i], true);
			}
		}

		/// <summary>
		/// 拷贝文件夹
		/// 注意：包括所有子目录的文件
		/// </summary>
		public static void CopyDirectory(string sourcePath, string destPath)
		{
			sourcePath = PathHelper.GetRegularPath(sourcePath);

			// If the destination directory doesn't exist, create it.
			if (Directory.Exists(destPath) == false)
				Directory.CreateDirectory(destPath);

			string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
			foreach (string file in files)
			{
				string temp = PathHelper.GetRegularPath(file);
				string savePath = temp.Replace(sourcePath, destPath);
				FileHelper.Copy(file, savePath, true);
			}
		}

		/// <summary>
		/// 递归查找目标文件夹路径
		/// </summary>
		/// <param name="root">搜索的根目录</param>
		/// <param name="folderName">目标文件夹名称</param>
		/// <returns>返回找到的文件夹路径，如果没有找到返回空字符串</returns>
		public static bool TryFind(string root, string folderName, out string fullPath)
		{
			DirectoryInfo rootInfo = new DirectoryInfo(root);
			DirectoryInfo[] infos = rootInfo.GetDirectories();
			for (int i = 0; i < infos.Length; i++)
			{
				//获取子目录的全路径
				fullPath = infos[i].FullName;

				//如果找到目标文件夹, 返回true
				if (infos[i].Name == folderName) return true;

				//递归查找子目录
				if (TryFind(fullPath, folderName, out fullPath)) return true;
			}
			fullPath = string.Empty;
			return false;
		}
	}
}