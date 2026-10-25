/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16

* 描述： 图片视图。

*/

namespace WorldTree
{
	/// <summary>
	/// 图片
	/// </summary>
	public class ViewImage : ViewWidget
	{
		/// <summary>
		/// 图片地址
		/// </summary>
		public string Url;

		/// <summary>
		/// 改变事件
		/// </summary>
		public RuleUnicast<ISendRule> OnChange;
	}
}
