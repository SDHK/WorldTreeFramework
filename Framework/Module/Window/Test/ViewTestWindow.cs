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
	public class ViewTestWindow : ViewWindow
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string Name;

		/// <summary>
		/// 测试文本（子级 widget，绑定契约）
		/// </summary>
		public ViewText text;

		/// <summary>
		/// 测试按钮（子级 widget，绑定契约）
		/// </summary>
		public ViewButton button;

	}

	//================================

	public static partial class ViewWindowRule
	{

		[NodeRule(nameof(OpenRule<ViewTestWindow>))]
		private static void OnOpenRule(this ViewTestWindow self)
		{
			self.Log($"视图窗口节点添加！！!{self.Depth}:{self.Layer}:{self.Order}");
		}

		[NodeRule(nameof(LayerChangeRule<ViewTestWindow>))]
		private static void OnLayerChangeRule(this ViewTestWindow self)
		{
			self.Log($"视图窗口层级变更！！!{self.Depth}:{self.Layer}:{self.Order}");
		}

	}
}
