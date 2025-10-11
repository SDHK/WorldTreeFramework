/****************************************

* 作者： 闪电黑客
* 日期： 2025/7/17 20:48

* 描述： 

*/
namespace WorldTree
{
	public static class WindowManagerRule
	{
		//V和VB都是纯数据，V是初始化数据，VB负责桥接具体UI组件。
		//Open，Close是V是否拥有VB。
		//Show,Hide是VB的显示隐藏，融入激活。
		//融入Dispose
		//普通组件没有show,hide

		/// <summary>
		/// 添加视图数据
		/// </summary>
		public static V AddView<V>(this View self, out V subView)
			where V : View
		{
			return self.Bind.Value.AddChild(out subView);
		}

		/// <summary>
		/// 添加视图数据
		/// </summary>
		public static View AddView(this View self, long viewType, out View subView)
		{
			subView = self.Core.PoolGetNode(viewType) as View;
			NodeBranchHelper.AddNodeToTree(self.Bind.Value, default(ChildBranch), subView.Id, subView);
			return subView;
		}

		/// <summary>
		/// 打开视图
		/// </summary>
		public static void OpenView(this View self)
		{
			if (self.Bind.Value == null)
			{
				self.Bind = new(NodeBranchHelper.AddNode(self, default(ComponentBranch), self.ViewBindType, out ViewBind _));
				NodeRuleHelper.SendRule(self, default(Open));
			}
			//UI组件加载
			//Open UI
		}

		/// <summary>
		/// 测试
		/// </summary>
		public static void Test()
		{
			ViewGroup viewGroup = new();

			viewGroup.AddView(out ViewTest view);
			view.Name = "测试窗口";
			view.OpenView();
		}
	}
}
