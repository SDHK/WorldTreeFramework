namespace WorldTree
{
	public static partial class ViewWindowRule
	{
		[NodeRule(nameof(OpenRule<ViewTestWindow>))]
		private static void OnOpenRule(this ViewTestWindow self)
		{
			self.Log($"视图窗口节点添加！！!{self.Depth}:{self.Layer}:{self.Order}");
		}

		[NodeRule(nameof(LayerChangeRule<ViewTestWindow>))]
		private static void OnLayerChangeRule(this ViewTestWindow self)
		{
			self.Log($"视图窗口层级变更！！!{self.Depth}:{self.Layer}:{self.Order}");
		}

	}
}
