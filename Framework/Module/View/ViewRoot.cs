namespace WorldTree
{
	/// <summary>
	/// 视图根：只挂在 ViewManager 下。八个层按名称 Layer0 到 Layer7 挂在收集器上，不写字段。
	/// </summary>
	public class ViewRoot : ViewObject
		, ViewOf<ViewManager>
	{
		/// <summary>
		/// 根上的层数。
		/// </summary>
		public const int LayerSlotCount = 8;

		/// <summary>
		/// 已打开的层。下标就是层号。
		/// </summary>
		private NodeRef<ViewLayer>[] layerSlots;

		/// <summary>
		/// 按下标取层。还没有建出来就是空。
		/// </summary>
		public ViewLayer GetLayerSlot(int layer)
		{
			if (layerSlots == null || (uint)layer >= (uint)layerSlots.Length) return null;
			return layerSlots[layer];
		}

		/// <summary>
		/// 按下标写入层。
		/// </summary>
		public void SetLayerSlot(int layer, ViewLayer viewLayer)
		{
			if ((uint)layer >= LayerSlotCount) return;
			if (layerSlots == null || layerSlots.Length != LayerSlotCount)
				layerSlots = new NodeRef<ViewLayer>[LayerSlotCount];
			layerSlots[layer] = viewLayer;
		}
	}

	public static class ViewRootRule
	{
		/// <summary>
		/// 根的壳接上之后建八个层。层打开用协程拉起，查找在这次调用里走完。
		/// 某一层没挂上就全部拆掉并失败，根不算打开。
		/// </summary>
		class Open : OpenRule<ViewRoot>
		{
			protected override void Execute(ViewRoot self)
			{
				try
				{
					for (int layer = 0; layer < ViewRoot.LayerSlotCount; layer++)
						AddLayer(self, layer);
				}
				catch
				{
					ClearLayers(self);
					throw;
				}
			}
		}

		/// <summary>
		/// 关闭时放下层引用。节点仍挂在 ViewBranch 上，随后由 RemoveAllView 回收。
		/// </summary>
		class Close : CloseRule<ViewRoot>
		{
			protected override void Execute(ViewRoot self)
			{
				for (int layer = 0; layer < ViewRoot.LayerSlotCount; layer++) self.SetLayerSlot(layer, null);
			}
		}

		/// <summary>
		/// 按层号取已经打开的层。不建节点。
		/// </summary>
		public static ViewLayer GetLayer(this ViewRoot self, int layer)
		{
			if ((uint)layer >= ViewRoot.LayerSlotCount) throw new System.Exception($"层号 {layer} 超出范围");
			ViewLayer viewLayer = self.GetLayerSlot(layer);
			if (viewLayer == null || viewLayer.IsDisposed) throw new System.Exception($"层号 {layer} 还没有打开");
			return viewLayer;
		}

		/// <summary>
		/// 建层并用协程打开。名称写成 Layer 加层号，和面板上的名称对上。
		/// </summary>
		private static void AddLayer(ViewRoot self, int layer)
		{
			self.AddView(out ViewLayer viewLayer);
			viewLayer.Layer = (byte)layer;
			viewLayer.ViewName = "Layer" + layer;
			self.SetLayerSlot(layer, viewLayer);
			viewLayer.OnOpen().Coroutine();
		}

		/// <summary>
		/// 拆掉这次已经建出来的层。
		/// </summary>
		private static void ClearLayers(ViewRoot self)
		{
			for (int layer = 0; layer < ViewRoot.LayerSlotCount; layer++)
			{
				ViewLayer viewLayer = self.GetLayerSlot(layer);
				self.SetLayerSlot(layer, null);
				if (viewLayer != null && !viewLayer.IsDisposed) viewLayer.Dispose();
			}
		}
	}
}
