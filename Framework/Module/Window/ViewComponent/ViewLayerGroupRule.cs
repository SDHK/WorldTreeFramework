namespace WorldTree
{
	public static class ViewLayerGroupRule
	{
		/// <summary>
		/// 获取层级
		/// </summary>
		public static ViewLayer GetLayer(this ViewLayerGroup self, int layer)
		{
			if (!self.ViewBind.TryGetGeneric(layer, out ViewLayer viewLayer))
			{
				self.ViewBind.AddGeneric(layer, out viewLayer);
				viewLayer.Layer = (byte)layer;
				viewLayer.OnOpen();
			}
			else if (!viewLayer.IsOpen)
			{
				viewLayer.OnOpen();
			}
			return viewLayer;
		}
	}
}
