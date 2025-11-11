namespace WorldTree
{
	public static partial class ViewTestRule
	{
		[NodeRule(nameof(AddRule<ViewTest>))]
		private static void OnAddRule(this ViewTest self)
		{
			self.Log($"视图测试节点添加！！!");

			self.AddComponent(out ViewLayerGroup layerGroup);
			layerGroup.OnOpen();

			ViewLayer viewLayer = layerGroup.GetLayer(0);
			viewLayer.TryAddView(out ViewTestWindow view);
			view.Name = "测试窗口ViewTestWindow";
			viewLayer.OpenView(view.Type);
		}
	}
}
