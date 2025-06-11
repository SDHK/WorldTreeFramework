using System;
using System.Windows.Forms;

namespace WorldTree
{
	/// <summary>
	/// 世界之心：WinForm线程
	/// </summary>
	public class WinFormWorldHeart : WorldHeartBase
		, AsComponentBranch
		, CoreManagerOf<WorldLine>
		, AsAwake<int>
	{

		/// <summary>
		/// From主线程
		/// </summary>
		public Timer m_Thread;

		/// <summary>
		/// 上一次运行时间
		/// </summary>
		public DateTime afterTime;

		#region 世界脉搏
		/// <summary>
		/// 世界脉搏 UpdateTime
		/// </summary>
		public WorldPulse<UpdateTime> worldUpdate;

		#endregion

		#region 全局事件法则

		/// <summary>
		/// 全局事件法则 Enable
		/// </summary>
		public IRuleExecutor<Enable> enable;
		/// <summary>
		/// 全局事件法则 Disable
		/// </summary>
		public IRuleExecutor<Disable> disable;
		/// <summary>
		/// 全局事件法则 Update
		/// </summary>
		public IRuleExecutor<Update> update;
		/// <summary>
		/// 全局事件法则 UpdateTime
		/// </summary>
		public IRuleExecutor<UpdateTime> updateTime;

		#endregion

		/// <summary>
		/// 运行
		/// </summary>
		public override void Run()
		{
			isRun = true;
		}

		/// <summary>
		/// 暂停
		/// </summary>
		public override void Pause()
		{
			isRun = false;
		}

		/// <summary>
		/// 单帧运行
		/// </summary>
		public override void OneFrame()
		{
			isRun = false;
			worldUpdate?.Update((DateTime.Now - afterTime));
		}

	}


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