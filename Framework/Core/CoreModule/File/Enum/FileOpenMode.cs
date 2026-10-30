/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件打开模式。

*/

namespace WorldTree
{
	/// <summary>
	/// 文件打开模式
	/// </summary>
	public enum FileOpenMode
	{
		/// <summary>
		/// 打开已存在的文件
		/// </summary>
		Open,

		/// <summary>
		/// 创建或截断
		/// </summary>
		Create,

		/// <summary>
		/// 新建，已存在则失败
		/// </summary>
		CreateNew,

		/// <summary>
		/// 追加
		/// </summary>
		Append,

		/// <summary>
		/// 截断已存在的文件
		/// </summary>
		Truncate,
	}
}
