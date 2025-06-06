namespace WorldTree
{
	public static partial class WinFormWorldHeartRule
	{
		[NodeRule(nameof(AwakeRule<WinFormWorldHeart, int>))]
		private static void OnAwake(this WinFormWorldHeart self, int frameTime)
		{
			self.frameTime = frameTime;
			self.Core.GetGlobalRuleExecutor(out self.updateTime);
			self.AddComponent(out self.worldUpdate, frameTime).Run();
		}

		[NodeRule(nameof(AddRule<WinFormWorldHeart>))]
		private static void OnAdd(this WinFormWorldHeart self)
		{
			self.Core.WorldLineManager.MainUpdate += self.worldUpdate.Update;
		}

		[NodeRule(nameof(RemoveRule<WinFormWorldHeart>))]
		private static void OnRemove(this WinFormWorldHeart self)
		{
			self.Core.WorldLineManager.MainUpdate -= self.worldUpdate.Update;
		}


		private class UpdateTime : UpdateTimeRule<WinFormWorldHeart>
		{
			protected override void Execute(WinFormWorldHeart self, TimeSpan deltaTime)
			{
				self.enable?.Send();
				self.update?.Send();
				self.updateTime?.Send(deltaTime);
				self.disable?.Send();
			}
		}
	}
}
