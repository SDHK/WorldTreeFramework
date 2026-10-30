/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16 19:49

* 描述： 视图绑定：View 与平台元素之间的桥接，只持有平台元素引用。

*/
namespace WorldTree
{
	/// <summary>
	/// 视图绑定：持有平台元素引用与平台实例子节点。
	/// 子级 View / widget 挂在 ViewBranch 下，不挂在这里。
	/// </summary>
	public class ViewBind : Node
		, ComponentOf<View>
		, AsChildBranch
		, AsRule<Awake>
		, AsRule<Remove>
	{
		/// <summary>
		/// 平台元素实例
		/// </summary>
		public IViewElement ui;
	}

	/// <summary>
	/// ViewBind 移除时清空平台元素引用；平台实例作为子节点随 Bind 释放。
	/// </summary>
	class ViewBindRemoveRule : RemoveRule<ViewBind>
	{
		protected override void Execute(ViewBind self)
		{
			self.ui = null;
		}
	}
}
