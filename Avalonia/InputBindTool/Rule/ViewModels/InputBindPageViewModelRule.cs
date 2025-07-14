namespace WorldTree
{
	public static partial class InputBindPageViewModelRule
	{
		[NodeRule(nameof(AwakeRule<InputBindPageViewModel, InputBindPage>))]
		private static void OnAwake(this InputBindPageViewModel self, InputBindPage page)
		{
			page.DataContext = self;
		}
	}
}
