/****************************************

* 作者： 闪电黑客
* 日期： 2025/7/17 20:48

* 描述： 

*/
namespace WorldTree
{
	public static class WindowManagerRule
	{


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
			subView = null;
			if (!self.IsOpen) return null;
			subView = self.Core.PoolGetNode(viewType) as View;
			NodeBranchHelper.AddNodeToTree(self.Bind.Value, default(ChildBranch), subView.Id, subView);
			return subView;
		}



		/// <summary>
		/// 测试
		/// </summary>
		public static void Test()
		{
			ViewGroup viewGroup = new();
		}
	}
}
