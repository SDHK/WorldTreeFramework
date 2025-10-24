namespace WorldTree
{
	public static class ViewLayerGroupRule
	{
		/// <summary>
		/// 获取层级
		/// </summary>
		public static ViewLayer GetLayer(ViewLayerGroup self, int layer)
		{
			if (!self.ViewBind.TryGetGeneric(layer, out ViewLayer viewLayer))
			{
				self.ViewBind.AddGeneric(layer, out viewLayer);
				viewLayer.Layer = layer * self.Interval + self.Layer;
				viewLayer.Open();
			}
			else if (!viewLayer.IsOpen)
			{
				viewLayer.Open();
			}
			return viewLayer;
		}
	}
}
