namespace WorldTree
{
	public static partial class InputBindPageViewModelRule
	{
		[NodeRule(nameof(AwakeRule<InputBindPageViewModel, InputBindPage>))]
		private static void OnAwake(this InputBindPageViewModel self, InputBindPage page)
		{
			page.DataContext = self;
		}

		[NodeRule(nameof(ListenerNodeAddRule<InputBindPageViewModel, InputArchive>))]







		private static void OnListenerNodeAdd(this InputBindPageViewModel self, InputArchive arg1)
		{
			self.Log($"监听添加：{arg1.GetKey<string>()}");
		}
	}
}
