using System;
using Timer = System.Windows.Forms.Timer;

namespace WorldTree
{
	public static partial class WinFormWorldHeartRule
	{
		[NodeRule(nameof(AwakeRule<WinFormWorldHeart, int>))]
		private static void OnAwake(this WinFormWorldHeart self, int frameTime)
		{
			self.frameTime = frameTime;

			self.Core.GetGlobalRuleExecutor(out self.enable);
			self.Core.GetGlobalRuleExecutor(out self.update);
			self.Core.GetGlobalRuleExecutor(out self.updateTime);
			self.Core.GetGlobalRuleExecutor(out self.disable);

			self.AddComponent(out self.worldUpdate, frameTime).Run();

			self.afterTime = DateTime.Now;
			self.m_Thread = new Timer();
			self.m_Thread.Interval = frameTime;
			self.m_Thread.Tick += (s, e) =>
			{
				if (self.isRun)
				{
					self.worldUpdate.Update(DateTime.Now - self.afterTime);
				}
				self.afterTime = DateTime.Now;
			};
			self.m_Thread.Start();
		}

		[NodeRule(nameof(RemoveRule<WinFormWorldHeart>))]
		private static void OnRemove(this WinFormWorldHeart self)
		{
			self.m_Thread?.Stop();
			self.m_Thread?.Dispose();
			self.m_Thread = null;

			self.worldUpdate = null;

			self.enable = null;
			self.update = null;
			self.updateTime = null;
			self.disable = null;
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
