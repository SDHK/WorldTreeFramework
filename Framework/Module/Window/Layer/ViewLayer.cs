using System;

namespace WorldTree
{
	/// <summary>
	/// 视图层级元素接口 
	/// </summary>
	public interface IViewLayerElement : IViewElement
	{

	}

	/// <summary>
	/// 视图层 
	/// </summary>
	public class ViewLayer : ViewContainer
		, ChildOf<ViewLayerGroup>
		, ComponentOf<View>
		, AsComponentBranch
		, AsRule<SubViewClose>
		, AsRule<Awake>

	{
		/// <summary>
		/// 节点列表
		/// </summary>
		public UnitList<NodeRef<View>> ViewList;

		/// <summary>
		/// ID 索引
		/// </summary>
		public UnitDictionary<long, int> IdIndexDict;

		/// <summary>
		/// 层级数量
		/// </summary>
		public byte LayerCount => (byte)(ViewList == null ? 0 : ViewList.Count);
	}

	public static partial class ViewLayerRule
	{
		/// <summary>
		/// 层绑定的列表在 Awake 里分配
		/// </summary>
		class Awake : AwakeRule<ViewLayer>
		{
			protected override void Execute(ViewLayer self)
			{
				self.World.PoolGetUnit(out self.ViewList);
				self.World.PoolGetUnit(out self.IdIndexDict);
			}
		}

		class Remove : RemoveRule<ViewLayer>
		{
			protected override void Execute(ViewLayer self)
			{
				self.ViewList?.Dispose();
				self.ViewList = null;
				self.IdIndexDict?.Dispose();
				self.IdIndexDict = null;
			}
		}

		[NodeRule(nameof(Close))]
		private static void OnCloseRule(this ViewLayer self)
		{
			// 关闭时清理所有视图
			self.CloseAllView();
		}

		/// <summary>
		/// 尝试添加视图:类型
		/// </summary>
		public static bool TryAddView(this ViewLayer self, Type type, out ViewLayerItem view)
		{
			view = null;
			long typeCode = self.TypeToCode(type);
			if (NodeBranchHelper.TryGetBranch(self, out ComponentBranch branch) && branch.Contains(typeCode)) return false;
			NodeBranchHelper.AddNode(self, default(ComponentBranch), typeCode, type, out INode node);
			view = (ViewLayerItem)node;
			return true;
		}

		/// <summary>
		/// 尝试添加视图
		/// </summary>
		public static bool TryAddView<T>(this ViewLayer self, out T view)
			where T : ViewLayerItem
		{
			if (self.TryGetComponent(out view)) return false;
			self.AddComponent(out view);
			return true;
		}

		/// <summary>
		/// 打开视图 : 类型码
		/// </summary>
		public static void OpenView(this ViewLayer self, long typeCode)
		{
			if (self?.ViewList == null) return;
			if (!(NodeBranchHelper.TryGetBranch(self, out ComponentBranch branch) && branch.TryGetNode(typeCode, out INode node))) return;
			if (node is not ViewLayerItem view) return;
			if (!view.IsOpen)
			{
				if (self.ViewList.Count == 15) throw new System.Exception("当前视图层级堆叠达到最大15，无法继续打开新视图!");
				self.IdIndexDict[view.Id] = self.ViewList.Count;
				self.ViewList.Add(new(view));
				view.Layer = (byte)self.ViewList.Count;
				view.OnOpen();
			}
			else
			{
				// 已经打开则提升到顶层 
				self.SetTopView(typeCode);
			}
		}

		/// <summary>
		/// 打开视图
		/// </summary>
		public static void OpenView<T>(this ViewLayer self) where T : ViewLayerItem => self.OpenView(self.TypeToCode<T>());

		/// <summary>
		/// 关闭视图 : 类型码
		/// </summary>
		public static void CloseView(this ViewLayer self, long typeCode)
		{
			if (self?.ViewList == null || self.ViewList.Count == 0) return;
			if (!(NodeBranchHelper.TryGetBranch(self, out ComponentBranch branch) && branch.TryGetNode(typeCode, out INode node))) return;
			if (node is not ViewLayerItem view) return;

			view.OnClose();
			self.RemoveFromList(view);
		}

		/// <summary>
		/// 关闭所有视图 
		/// </summary>
		public static void CloseAllView(this ViewLayer self)
		{
			if (self?.ViewList == null || self.ViewList.Count == 0) return;
			// 从顶层向下关闭
			for (int i = self.ViewList.Count - 1; i >= 0; i--)
			{
				var nodeRef = self.ViewList[i];
				if (nodeRef.IsNull) continue;
				nodeRef.Value.OnClose();
			}
			self.ViewList.Clear();
			self.IdIndexDict.Clear();
		}

		/// <summary>
		/// 关闭视图
		/// </summary>
		public static void CloseView<T>(this ViewLayer self) where T : ViewLayerItem => self.CloseView(self.TypeToCode<T>());

		/// <summary>
		/// 设置视图到顶层 : 类型码
		/// </summary>
		public static void SetTopView(this ViewLayer self, long typeCode)
		{
			if (self?.ViewList == null || self.ViewList.Count == 0) return;
			if (!(NodeBranchHelper.TryGetBranch(self, out ComponentBranch branch) && branch.TryGetNode(typeCode, out INode node))) return;
			if (node is not ViewLayerItem view) return;
			if (!self.IdIndexDict.Remove(view.Id, out int index)) return;

			self.ViewList.RemoveAt(index);
			self.IdIndexDict[view.Id] = self.ViewList.Count;
			self.ViewList.Add(new(view));
			Reindex(self);
			view.LayerChange();
		}

		/// <summary>
		/// 设置视图到顶层（带空判断并移除空项）
		/// </summary>
		public static void SetTopView<T>(this ViewLayer self) where T : ViewLayerItem => self.SetTopView(self.TypeToCode<T>());

		/// <summary>
		/// 尝试获取顶层窗口
		/// </summary>
		public static bool TryGetTopView<T>(this ViewLayer self, out T view)
			where T : ViewLayerItem
		{
			view = null;

			if (self?.ViewList == null || self.ViewList.Count == 0) return false;
			for (int i = self.ViewList.Count - 1; i >= 0; i--)
			{
				var nodeRef = self.ViewList[i];
				if (nodeRef.IsNull) continue;
				if (nodeRef.Value is T tWindow)
				{
					view = tWindow;
					return true;
				}
			}
			return false;
		}

		private static void RemoveFromList(this ViewLayer self, View view)
		{
			if (!self.IdIndexDict.Remove(view.Id, out int index)) return;
			self.ViewList.RemoveAt(index);
			Reindex(self);
		}

		private static void Reindex(this ViewLayer self)
		{
			for (int i = 0; i < self.ViewList.Count; i++)
			{
				self.ViewList[i].Value.Layer = (byte)(i + 1);
			}
		}
	}
}
