/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件访问方式。

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 文件访问方式
	/// </summary>
	[Flags]
	public enum FileOpenAccess
	{
		/// <summary>
		/// 无
		/// </summary>
		None = 0,

		/// <summary>
		/// 只读
		/// </summary>
		Read = 1 << 0,

		/// <summary>
		/// 只写
		/// </summary>
		Write = 1 << 1,

		/// <summary>
		/// 读写
		/// </summary>
		ReadWrite = Read | Write,
	}
}
