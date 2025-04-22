using EnvDTE;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace OperateLinkFile
{

	/// <summary>
	/// 命令帮助类
	/// </summary>
	public static class CommandHelper
	{
		/// <summary>
		/// 找到链接文件夹的原始路径
		/// </summary>
		public static string GetOriginalPath(ProjectItem projectItem)
		{
			// 加载项目文件
			XDocument projectFile = XDocument.Load(projectItem.ContainingProject.FullName);

			// 查找所有的<Compile>元素
			IEnumerable<XElement> compileElements = projectFile.Descendants("Compile");

			// 获取到的是链接文件夹的虚拟路径
			string fullPath = projectItem.Properties.Item("FullPath").Value.ToString();

			// 项目的父级文件夹的虚拟路径
			string projectPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);

			//文件虚拟路径裁剪掉项目虚拟路径，得到虚拟的相对地址
			string linkPath = fullPath.Substring(projectPath.Length).TrimStart(Path.DirectorySeparatorChar);

			// 遍历<Compile>元素，查找包含<Link>元素的元素
			foreach (XElement compileElement in compileElements)
			{
				XElement linkElement = compileElement.Element("Link");
				if (linkElement != null)
				{
					//linkElement.Value 就是 <Link>元素的值
					//检查<Link>元素的值是否与链接文件在项目中的路径匹配

					int RecursiveDirIndex = linkElement.Value.IndexOf("%(RecursiveDir)");
					int FileNameIndex = linkElement.Value.IndexOf("%(FileName)");
					//int ExtensionIndex = linkElement.Value.IndexOf("%(Extension)");
					int RecursiveDirLength = "%(RecursiveDir)".Length;
					//int FileNameLength = "%(FileName)".Length;
					//int ExtensionLength = "%(Extension)".Length;

					//获取%(RecursiveDir)之前的字符
					string linkPatternRecursiveDir = linkElement.Value.Substring(0, RecursiveDirIndex);
					//获取 %(RecursiveDir)与 %(FileName) 之间的字符
					string linkPatternFileName = linkElement.Value.Substring(RecursiveDirIndex + RecursiveDirLength, FileNameIndex - RecursiveDirIndex - RecursiveDirLength);

					//判断 相对地址 是否包含了 linkPatternRecursiveDir 之前的虚拟地址
					if (linkPath.Contains(linkPatternRecursiveDir))
					{
						//剔除 linkPatternRecursiveDir 之前的虚拟地址
						linkPath = linkPath.Replace(linkPatternRecursiveDir, "");

						// 剔除 %(RecursiveDir)与 %(FileName) 之间的字符
						if (linkPatternFileName != string.Empty) linkPath = linkPath.Replace(linkPatternFileName, "");
						// 此时 linkPath 应该只剩下 原始的相对路径

						// 这个是<Compile>元素的Include的值
						string basePath = compileElement.Attribute("Include").Value;
						// 移除通配符部分
						int wildcardIndex = basePath.IndexOf('*');
						// 此时拿到的就是项目文件夹的路径父级
						if (wildcardIndex != -1) basePath = basePath.Substring(0, wildcardIndex);

						// 拼接出链接文件的原始路径
						string originalPath = Path.GetFullPath(Path.Combine(projectPath, basePath, linkPath));

						return originalPath;
					}
				}
			}

			// 如果没有找到匹配的<Link>元素，返回null
			return null;
		}

		/// <summary>
		/// 刷新项目配置
		/// </summary>
		public static void RefreshProject(string ProjectPath)
		{
			string text = File.ReadAllText(ProjectPath);
			text += " "; // 在文件尾部添加空格
			File.WriteAllText(ProjectPath, text); // 保存文件

			text.TrimEnd(); // 删除文件尾部空格
			File.WriteAllText(ProjectPath, text); // 保存文件
		}
	}
}
