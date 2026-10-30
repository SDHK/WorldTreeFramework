/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件源能力标志。

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 文件源能力标志
	/// </summary>
	[Flags]
	public enum FileCapabilities
	{
		/// <summary>
		/// 无
		/// </summary>
		None = 0,

		/// <summary>
		/// 可读
		/// </summary>
		Read = 1 << 0,

		/// <summary>
		/// 可写
		/// </summary>
		Write = 1 << 1,

		/// <summary>
		/// 可定位
		/// </summary>
		Seek = 1 << 2,

		/// <summary>
		/// 可枚举目录
		/// </summary>
		List = 1 << 3,

		/// <summary>
		/// 可监听变更
		/// </summary>
		Watch = 1 << 4,

		/// <summary>
		/// 支持范围请求（断点续传）
		/// </summary>
		Range = 1 << 5,

		/// <summary>
		/// 支持异步
		/// </summary>
		Async = 1 << 6,
	}
}
