namespace WorldTree
{

	public static partial class MainWorldRule
	{
		[NodeRule(nameof(AddRule<MainWorld>))]
		private static void OnAddRule(this MainWorld self)
		{
			self.Log($"配置表工具主世界启动！！");
		}
	}

}
