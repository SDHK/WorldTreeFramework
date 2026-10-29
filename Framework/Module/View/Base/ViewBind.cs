/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16 19:49

* 描述： 视图绑定：View 与平台元素之间的桥接，只持有平台元素引用。

*/
namespace WorldTree
{
	/// <summary>
	/// 视图绑定：持有平台元素引用，并承载本视图子级组件（widget）的挂载。
	/// 子级组件的字段声明在 View 上，实例挂在这里；关闭时随 Bind 一起回收。
	/// </summary>
	public class ViewBind : Node
		, ComponentOf<View>
		, AsRule<Awake>
	{
		/// <summary>
		/// 平台元素实例
		/// </summary>
		public IViewElement ui;
	}
}
