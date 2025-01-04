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
		/// 结束
		/// </summary>
		End,
		/// <summary>
		/// 开始
		/// </summary>
		Start = 1,

		/// <summary>
		/// 活跃中
		/// </summary>
		Active = 1 << 1,
	}
}
