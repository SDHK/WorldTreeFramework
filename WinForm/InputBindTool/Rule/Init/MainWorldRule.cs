namespace WorldTree
{
	public static partial class MainWorldRule
	{
		[NodeRule(nameof(AddRule<MainWorld>))]
		private static void OnAdd(this MainWorld self)
		{
			self.Log("Æô¶¯£¡");
			self.AddComponent(out MainFrom _);
		}
	}
}
