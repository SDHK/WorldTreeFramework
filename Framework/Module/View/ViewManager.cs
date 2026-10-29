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
		public static ViewLayerGroup GetLayerGroup(this ViewManager self)
		{
			if (self.layerGroup == null)
			{
				self.AddChild(out self.layerGroup);
				self.layerGroup.OnOpen();
			}
			return self.layerGroup;
		}

		/// <summary>
		/// 取层（委托给层组）
		/// </summary>
		public static ViewLayer GetLayer(this ViewManager self, int layer) => self.GetLayerGroup().GetLayer(layer);



		/// <summary>
		/// 添加视图数据
		/// </summary>
		public static V AddView<V>(this View self, out V subView)
			where V : View
		{
			return self.AddChild(out subView);
		}

		/// <summary>
		/// 添加视图数据
		/// </summary>
		public static View AddView(this View self, Type viewType, out View subView)
		{
			subView = null;
			if (!self.IsOpen) return null;
			subView = self.World.PoolGetNode(viewType) as View;
			NodeBranchHelper.AddNodeToTree(self, default(ChildBranch), subView.Id, subView);
			return subView;
		}

	}
}
