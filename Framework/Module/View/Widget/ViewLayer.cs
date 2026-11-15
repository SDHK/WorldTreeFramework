using System;

namespace WorldTree
{
	/// <summary>
	/// 视图层：子级页面管理器。页面按类型唯一，重复项交给列表容器，不放在这里。
	/// </summary>
	public class ViewLayer : ViewContainer
		, ViewOf<ViewRoot>
		, ComponentOf<View>
		, AsComponentBranch
		, AsRule<SubViewClose>
		, AsRule<Awake>

	{
		/// <summary>
		/// 层登记的页面。类型码 → 节点。关掉的页面也留在这里。
		/// </summary>
		public UnitDictionary<long, NodeRef<ViewLayerItem>> ViewDict;

		/// <summary>
		/// 打开栈。从接受打开起占一个槽，直到变成 Close、壳卸掉才摘掉。
		/// Load、OpenAnim、Open、CloseAnim 都在里面。加载被取消后会马上离开。顺序就是叠放顺序，下标加一是 <see cref="View.Layer"/>。
		/// </summary>
		public UnitList<NodeRef<View>> ViewOpenList;

		/// <summary>
		/// 打开栈上的页面数量，含正在加载和正在播动画的。
		/// </summary>
		public byte LayerCount => (byte)(ViewOpenList == null ? 0 : ViewOpenList.Count);

		/// <summary>
		/// 关闭层：先关闭层内视图，再关闭自身。子页面随 ViewBranch 回收。
		/// </summary>
		public override async TreeTask OnClose()
		{
			await this.TreeTaskCompleted();
			if (!IsOpen) return;
			await this.CloseAllView();
			await base.OnClose();
			this.ViewDict?.Clear();
			this.ViewOpenList?.Clear();
		}
	}

	[NodeRuleTarget(nameof(ViewLayer))]
	public static partial class ViewLayerRule
	{
		/// <summary>
		/// 层绑定的列表在 Awake 里分配
		/// </summary>
		class Awake : AwakeRule<ViewLayer>
		{
			protected override void Execute(ViewLayer self)
			{
				self.World.PoolGetUnit(out self.ViewDict);
				self.World.PoolGetUnit(out self.ViewOpenList);
			}
		}

		class Remove : RemoveRule<ViewLayer>
		{
			protected override void Execute(ViewLayer self)
			{
				self.ViewDict?.Dispose();
				self.ViewDict = null;
				self.ViewOpenList?.Dispose();
				self.ViewOpenList = null;
			}
		}

		/// <summary>
		/// 按类型码取已登记的页面。节点已释放时摘掉这个键。
		/// </summary>
		private static bool TryGetPage(this ViewLayer self, long typeCode, out ViewLayerItem view)
		{
			view = null;
			if (self.ViewDict == null) return false;
			if (!self.ViewDict.TryGetValue(typeCode, out NodeRef<ViewLayerItem> nodeRef)) return false;
			if (nodeRef.IsNull)
			{
				self.ViewDict.Remove(typeCode);
				return false;
			}

			view = nodeRef.Value;
			return view != null;
		}

		/// <summary>
		/// 登记一个新页面：挂在字典里，此时尚未打开。
		/// </summary>
		private static void Track(this ViewLayer self, ViewLayerItem view)
		{
			view.Layer = 0;
			self.ViewDict[view.Type] = new(view);
		}

		/// <summary>
		/// 尝试添加视图:类型。已有同类型页面时返回 false，并把已有节点写到 view。
		/// </summary>
		public static bool TryAddView(this ViewLayer self, Type type, out ViewLayerItem view)
		{
			long typeCode = self.TypeToCode(type);
			if (self.TryGetPage(typeCode, out view)) return false;
			INode node = self.World.PoolGetNode(type);
			NodeBranchHelper.AddNodeToTree(self, default(ViewBranch), node.Id, node);
			view = (ViewLayerItem)node;
			self.Track(view);
			return true;
		}

		/// <summary>
		/// 尝试添加视图。已有同类型页面时返回 false，并把已有节点写到 view。
		/// </summary>
		public static bool TryAddView<T>(this ViewLayer self, out T view)
			where T : ViewLayerItem
		{
			if (self.TryGetPage(self.TypeToCode<T>(), out ViewLayerItem item))
			{
				view = (T)item;
				return false;
			}
			self.AddView(out view);
			self.Track(view);
			return true;
		}

		/// <summary>
		/// 打开视图 : 类型码
		/// </summary>
		public static async TreeTask OpenView(this ViewLayer self, long typeCode)
		{
			await self.TreeTaskCompleted();
			if (self?.ViewOpenList == null) return;
			if (!self.TryGetPage(typeCode, out ViewLayerItem view)) return;

			// 不在栈上才占槽。已经在栈上的，回来仍开着才置顶。新开的按压栈顺序，不因为先完成就抢到顶上。
			bool stacked = self.FindOpenIndex(view) >= 0;
			if (!stacked)
			{
				if (self.ViewOpenList.Count == 15) throw new System.Exception("当前视图层级堆叠达到最大15，无法继续打开新视图!");
				self.ViewOpenList.Add(new(view));
				view.Layer = (byte)self.ViewOpenList.Count;
			}

			try
			{
				await view.OnOpen();
			}
			finally
			{
				if (stacked && view.IsOpen) self.SetTopView(typeCode);
				else self.FinishOpen(view);
			}
		}

		/// <summary>
		/// 打开视图 : 类型
		/// </summary>
		public static async TreeTask OpenView<T>(this ViewLayer self) where T : ViewLayerItem
		{
			await self.OpenView(self.TypeToCode<T>());
		}

		/// <summary>
		/// 关闭视图 : 类型码
		/// </summary>
		public static async TreeTask CloseView(this ViewLayer self, long typeCode)
		{
			await self.TreeTaskCompleted();
			if (!self.TryGetPage(typeCode, out ViewLayerItem view)) return;

			try
			{
				await view.OnClose();
			}
			finally
			{
				self.FinishOpen(view);
			}
		}

		/// <summary>
		/// 关闭所有视图 
		/// </summary>
		public static async TreeTask CloseAllView(this ViewLayer self)
		{
			await self.TreeTaskCompleted();
			if (self?.ViewOpenList == null || self.ViewOpenList.Count == 0) return;
			// 从顶往下关。还开着或还在加载的留在栈上，等它自己的那次开关结束再摘。
			for (int i = self.ViewOpenList.Count - 1; i >= 0; i--)
			{
				if ((uint)i >= (uint)self.ViewOpenList.Count) continue;
				NodeRef<View> nodeRef = self.ViewOpenList[i];
				if (nodeRef.IsNull)
				{
					self.ViewOpenList.RemoveAt(i);
					continue;
				}
				await self.CloseView(nodeRef.Value.Type);
			}
		}

		/// <summary>
		/// 关闭视图
		/// </summary>
		public static async TreeTask CloseView<T>(this ViewLayer self) where T : ViewLayerItem
		{
			await self.CloseView(self.TypeToCode<T>());
		}

		/// <summary>
		/// 设置视图到顶层 : 类型码
		/// </summary>
		public static void SetTopView(this ViewLayer self, long typeCode)
		{
			if (self?.ViewOpenList == null || self.ViewOpenList.Count == 0) return;
			if (!self.TryGetPage(typeCode, out ViewLayerItem view) || !view.IsOpen) return;
			int index = self.FindOpenIndex(view);
			if (index < 0) return;

			self.MoveToEnd(index);
			self.Reindex();
			view.LayerChange();
		}

		/// <summary>
		/// 设置视图到顶层
		/// </summary>
		public static void SetTopView<T>(this ViewLayer self) where T : ViewLayerItem => self.SetTopView(self.TypeToCode<T>());

		/// <summary>
		/// 尝试获取顶层视图
		/// </summary>
		public static bool TryGetTopView<T>(this ViewLayer self, out T view)
			where T : ViewLayerItem
		{
			view = null;

			if (self?.ViewOpenList == null || self.ViewOpenList.Count == 0) return false;
			for (int i = self.ViewOpenList.Count - 1; i >= 0; i--)
			{
				NodeRef<View> nodeRef = self.ViewOpenList[i];
				if (nodeRef.IsNull || !nodeRef.Value.IsOpen) continue;
				if (nodeRef.Value is T topView)
				{
					view = topView;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 打开栈中的下标。优先用 Layer，对不上再沿打开栈找。
		/// </summary>
		private static int FindOpenIndex(this ViewLayer self, View view)
		{
			int index = view.Layer - 1;
			if ((uint)index < (uint)self.ViewOpenList.Count && self.ViewOpenList[index].Value == view) return index;
			for (int i = 0; i < self.ViewOpenList.Count; i++)
			{
				if (self.ViewOpenList[i].Value == view) return i;
			}
			return -1;
		}

		/// <summary>
		/// 开关回来后，已经是 Close 就从打开栈摘掉。还开着或还在加载就留着。
		/// </summary>
		private static void FinishOpen(this ViewLayer self, View view)
		{
			if (!view.IsOpen && !view.IsLoad) self.RemoveOpen(view);
		}

		/// <summary>
		/// 从打开栈摘掉，并重排剩余页面的槽位。节点本身不释放。
		/// </summary>
		private static void RemoveOpen(this ViewLayer self, View view)
		{
			int index = self.FindOpenIndex(view);
			if (index >= 0) self.ViewOpenList.RemoveAt(index);
			view.Layer = 0;
			self.Reindex();
		}

		/// <summary>
		/// 把下标对应的页面移到打开栈末尾。末尾就是后打开的一侧。
		/// </summary>
		private static void MoveToEnd(this ViewLayer self, int index)
		{
			if (index < 0 || index >= self.ViewOpenList.Count - 1) return;
			NodeRef<View> nodeRef = self.ViewOpenList[index];
			self.ViewOpenList.RemoveAt(index);
			self.ViewOpenList.Add(nodeRef);
		}

		/// <summary>
		/// 按打开栈重写槽位。空引用顺手摘掉。
		/// </summary>
		private static void Reindex(this ViewLayer self)
		{
			int write = 0;
			for (int read = 0; read < self.ViewOpenList.Count; read++)
			{
				NodeRef<View> nodeRef = self.ViewOpenList[read];
				if (nodeRef.IsNull || nodeRef.Value is not View view) continue;
				if (write != read) self.ViewOpenList[write] = nodeRef;
				view.Layer = (byte)(write + 1);
				write++;
			}

			if (write < self.ViewOpenList.Count) self.ViewOpenList.RemoveRange(write, self.ViewOpenList.Count - write);
		}
	}
}
