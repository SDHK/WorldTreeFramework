﻿/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System.IO;

namespace WorldTree
{
	/// <summary>
	/// 路径工具类
	/// </summary>
	public static partial class PathHelper
	{
		/// <summary>
		/// 获取规范的路径
		/// </summary>
		public static string GetRegularPath(string path)
		{
			return path.Replace('\\', '/').Replace("\\", "/"); //替换为Linux路径格式
		}

		/// <summary>
		/// 递归查找目标文件夹路径
		/// </summary>
		/// <param name="root">搜索的根目录</param>
		/// <param name="directoryName">目标文件夹名称</param>
		/// <returns>返回找到的文件夹路径，如果没有找到返回空字符串</returns>
		public static string FindDirectory(string root, string directoryName)
		{
			DirectoryInfo rootInfo = new DirectoryInfo(root);
			DirectoryInfo[] infos = rootInfo.GetDirectories();
			for (int i = 0; i < infos.Length; i++)
			{
				string fullPath = infos[i].FullName;
				if (infos[i].Name == directoryName)
					return fullPath;

				string result = FindDirectory(fullPath, directoryName);
				if (string.IsNullOrEmpty(result) == false)
					return result;
			}
			return string.Empty;
		}
	}
}