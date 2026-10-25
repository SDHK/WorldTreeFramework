/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16

* 描述： 文本视图。

*/

namespace WorldTree
{
	/// <summary>
	/// 文本
	/// </summary>
	public class ViewText : ViewWidget
	{
		/// <summary>
		/// 文本内容
		/// </summary>
		public string Content;

		/// <summary>
		/// 改变事件
		/// </summary>
		public RuleUnicast<ISendRule> OnChange;
	}
}
