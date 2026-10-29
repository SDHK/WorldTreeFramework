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
		/// 打开的 ViewObject
		/// </summary>
		public UnitDictionary<long, NodeRef<ViewObject>> viewDict = new UnitDictionary<long, NodeRef<ViewObject>>();

		/// <summary>
		/// 视图层组（懒建）
		/// </summary>
		public ViewLayerGroup layerGroup;
	}

	public static class ViewManagerRule
	{
		/// <summary>
		/// 取层组，没有就创建并打开
		/// </summary>
		public static async TreeTask<ViewLayerGroup> GetLayerGroup(this ViewManager self)
		{
			if (self.layerGroup == null)
			{
				self.AddView(out self.layerGroup);
				await self.layerGroup.Open();
			}
			return self.layerGroup;
		}

		/// <summary>
		/// 取层（委托给层组）
		/// </summary>
		public static async TreeTask<ViewLayer> GetLayer(this ViewManager self, int layer)
		{
			ViewLayerGroup layerGroup = await self.GetLayerGroup();
			return await layerGroup.GetLayer(layer);
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
