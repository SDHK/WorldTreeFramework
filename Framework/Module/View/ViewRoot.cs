using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 视图根：只挂在 ViewManager 下，管理第 0 层的视图层
	/// </summary>
	public class ViewRoot : ViewContainer
		, ViewOf<ViewManager>
	{
		/// <summary>
		/// 各层（键为层号）
		/// </summary>
		public readonly Dictionary<int, ViewLayer> layerDict = new();
	}

	public static class ViewRootRule
	{
		/// <summary>
		/// 取层，没有就在根下创建并打开
		/// </summary>
		public static async TreeTask<ViewLayer> GetLayer(this ViewRoot self, int layer)
		{
			await self.TreeTaskCompleted();
			if (self.layerDict.TryGetValue(layer, out ViewLayer viewLayer) && viewLayer != null) return viewLayer;

			self.AddView(out viewLayer);
			viewLayer.Layer = (byte)layer;
			self.layerDict[layer] = viewLayer;
			await viewLayer.OnOpen();
			return viewLayer;
		}
	}
}
