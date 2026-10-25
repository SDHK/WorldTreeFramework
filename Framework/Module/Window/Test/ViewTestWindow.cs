/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16

* 描述： 测试窗口与绑定。

*/

namespace WorldTree
{
	/// <summary>
	/// 测试视图
	/// </summary>
	public partial class ViewTestWindow : ViewWindow
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string Name;

		/// <summary>
		/// 测试文本（子级 widget，绑定契约；实例挂在 Bind 下，用 NodeRef 防悬空）
		/// </summary>
		public NodeRef<ViewText> text;

		/// <summary>
		/// 测试按钮（子级 widget，绑定契约；实例挂在 Bind 下，用 NodeRef 防悬空）
		/// </summary>
		public NodeRef<ViewButton> button;

	}

	[NodeRuleTarget(nameof(ViewTestWindow))]
	public static partial class ViewTestWindowRule
	{
		[NodeRule(nameof(Open))]
		private static void OnOpenRule(this ViewTestWindow self)
		{
			self.Log($"视图窗口节点添加！！!{self.Depth}:{self.Layer}:{self.Order}");
		}

		[NodeRule(nameof(LayerChange))]
		private static void OnLayerChangeRule(this ViewTestWindow self)
		{
			self.Log($"视图窗口层级变更！！!{self.Depth}:{self.Layer}:{self.Order}");
		}

	}
}
