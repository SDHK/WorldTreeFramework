/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件通道操作码。

*/

namespace WorldTree
{
	/// <summary>
	/// 文件通道操作码
	/// </summary>
	public enum FileChannel
	{
		/// <summary>
		/// 只读内容
		/// </summary>
		Pack,

		/// <summary>
		/// 持久可写
		/// </summary>
		User,

		/// <summary>
		/// 临时可写
		/// </summary>
		Temp,
	}
}
