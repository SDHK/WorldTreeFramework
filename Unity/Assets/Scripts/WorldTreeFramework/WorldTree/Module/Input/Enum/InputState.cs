/****************************************

* 作者：闪电黑客
* 日期：2024/12/23 16:41

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 输入状态
	/// </summary>
	[Flags]
	public enum InputState : byte
	{
		/// <summary>
		/// 无状态
		/// </summary>
		None = 0,

		/// <summary>
		/// 活跃中
		/// </summary>
		Active = 1,

		/// <summary>
		/// 开始
		/// </summary>
		Start = 1 << 1,

		/// <summary>
		/// 结束
		/// </summary>
		End = 1 << 2,
	}
}
