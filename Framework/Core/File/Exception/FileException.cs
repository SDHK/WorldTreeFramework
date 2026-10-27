/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件系统统一异常。

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 文件系统统一异常
	/// </summary>
	public class FileException : Exception
	{
		/// <summary>
		/// 错误码
		/// </summary>
		public FileError Error { get; }

		/// <summary>
		/// 出错路径
		/// </summary>
		public string Path { get; }

		public FileException(FileError error, string path, string message = null, Exception innerException = null)
			: base(message ?? $"{error}: {path}", innerException)
		{
			Error = error;
			Path = path;
		}
	}
}
