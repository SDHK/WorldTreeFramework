/****************************************

* 作者： 闪电黑客
* 日期： 2024/03/11 11:40:44

* 描述： 文件工具类
*
*/

using System.IO;
using System.Text;

namespace WorldTree
{
	public static partial class FileHelper
	{
		/// <summary>
		/// 重命名文件
		/// </summary>
		public static void Rename(string filePath, string newName)
		{
			string dirPath = Path.GetDirectoryName(filePath);
			string destPath;

			//判断是否有扩展名
			if (Path.HasExtension(filePath))
			{
				string extentsion = Path.GetExtension(filePath);
				destPath = $"{dirPath}/{newName}{extentsion}";
			}
			else
			{
				destPath = $"{dirPath}/{newName}";
			}
			new FileInfo(filePath).MoveTo(destPath);
		}

		/// <summary>
		/// 移动文件, 如果目标文件存在则删除
		/// </summary>
		public static void Move(string filePath, string targetPath)
		{
			if (File.Exists(targetPath)) File.Delete(targetPath);
			new FileInfo(filePath).MoveTo(targetPath);
		}

		/// <summary>
		/// 拷贝文件
		/// </summary>
		public static void Copy(string sourcePath, string targetPath, bool overwrite = true)
		{
			if (!File.Exists(sourcePath)) throw new FileNotFoundException(sourcePath);

			// 创建目录
			DirectoryHelper.TryCreateFilePath(targetPath);

			// 复制文件
			File.Copy(sourcePath, targetPath, overwrite);
		}

		/// <summary>
		/// 获取文件字节大小
		/// </summary>
		public static long GetSize(string filePath)
		{
			return new FileInfo(filePath).Length;
		}

		/// <summary>
		/// 获取文件的哈希值
		/// </summary>
		public static uint GetCRC32(string filePath)
		{
			using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			return CRC32Helper.StreamCRC32(fs);
		}

		/// <summary>
		/// 读取文件的所有文本内容
		/// </summary>
		public static string ReadAllText(string filePath)
		{
			if (File.Exists(filePath) == false) return string.Empty;
			return File.ReadAllText(filePath, Encoding.UTF8);
		}

		/// <summary>
		/// 读取文本的所有文本内容
		/// </summary>
		public static string[] ReadAllLine(string filePath)
		{
			if (File.Exists(filePath) == false) return null;
			return File.ReadAllLines(filePath, Encoding.UTF8);
		}
	}
}