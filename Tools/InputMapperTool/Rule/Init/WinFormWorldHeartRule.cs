namespace WorldTree
{
	public static partial class WinFormWorldHeartRule
	{
		[NodeRule(nameof(AddRule<WinFormWorldHeart>))]
		private static void OnAdd(this WinFormWorldHeart self)
		{
			MessageBox.Show($"世界之心启动！");
			self.Core.WorldLineManager.MainUpdate += self.worldUpdate.Update;
		}

		[NodeRule(nameof(RemoveRule<WinFormWorldHeart>))]
		private static void OnRemove(this WinFormWorldHeart self)
		{
			self.Core.WorldLineManager.MainUpdate -= self.worldUpdate.Update;
		}
	}
}
