namespace WorldTree
{
	public static partial class InputBindPageViewModelRule
	{
		[NodeRule(nameof(AwakeRule<InputBindPageViewModel, InputBindPage>))]
		private static void OnAwake(this InputBindPageViewModel self, InputBindPage page)
		{
			page.DataContext = self;
		}


		class NodeListenerAdd : NodeListenerAddRule<InputBindPageViewModel, InputArchive>
		{
			protected override void Execute(InputBindPageViewModel self, InputArchive node)
			{
				self.Log($"监听添加：{node.GetKey<string>()}");
			}
		}
	}
}
