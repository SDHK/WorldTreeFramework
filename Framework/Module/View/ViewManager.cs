/****************************************

* 作者： 闪电黑客
* 日期： 2025/7/17 20:46

* 描述： 

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 视图管理器
	/// </summary>
	public class ViewManager : Node
		, ComponentOf<World>
		, AsComponentBranch
		, AsChildBranch
		, AsViewBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 视图根（懒建）
		/// </summary>
		public ViewRoot root;
	}

	public static class ViewManagerRule
	{
		/// <summary>
		/// 取视图根，没有就创建并打开
		/// </summary>
		public static async TreeTask<ViewRoot> GetRoot(this ViewManager self)
		{
			await self.TreeTaskCompleted();
			if (self.root == null)
			{
				self.AddView(out self.root);
				await self.root.OnOpen();
			}
			return self.root;
		}

		/// <summary>
		/// 取层（委托给视图根）
		/// </summary>
		public static async TreeTask<ViewLayer> GetLayer(this ViewManager self, int layer)
		{
			ViewRoot root = await self.GetRoot();
			return await root.GetLayer(layer);
		}



		/// <summary>
		/// 添加显示子视图
		/// </summary>
		public static V AddSubView<V>(this View self, out V subView)
			where V : View
		{
			return self.AddView(out subView);
		}

		/// <summary>
		/// 添加显示子视图
		/// </summary>
		public static View AddSubView(this View self, Type viewType, out View subView)
		{
			subView = null;
			if (!self.IsOpen) return null;
			subView = self.World.PoolGetNode(viewType) as View;
			NodeBranchHelper.AddNodeToTree(self, default(ViewBranch), subView.Id, subView);
			return subView;
		}

	}
}
