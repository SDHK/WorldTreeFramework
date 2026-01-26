using Avalonia.Threading;
using System;

namespace WorldTree
{
	/// <summary>
	/// Avalonia世界之心
	/// </summary>
	public class AvaloniaWorldHeart : WorldHeartBase
		, AsComponentBranch
		, CoreManagerOf<WorldLine>
		, AsRule<Awake<int>>
	{

		/// <summary>
		/// Avalonia主线程
		/// </summary>
		public DispatcherTimer m_Thread;

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
		public RuleBroadcast<Enable> enable;
		/// <summary>
		/// 全局事件法则 Disable
		/// </summary>
		public RuleBroadcast<Disable> disable;
		/// <summary>
		/// 全局事件法则 Update
		/// </summary>
		public RuleBroadcast<Update> update;
		/// <summary>
		/// 全局事件法则 UpdateTime
		/// </summary>
		public RuleBroadcast<UpdateTime> updateTime;

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


	public static partial class AvaloniaWorldHeartRule
	{
		[NodeRule(nameof(AwakeRule<AvaloniaWorldHeart, int>))]
		private static void OnAwakeRule(this AvaloniaWorldHeart self, int frameTime)
		{
			self.frameTime = frameTime;

			self.Core.GetRuleBroadcast(out self.enable);
			self.Core.GetRuleBroadcast(out self.update);
			self.Core.GetRuleBroadcast(out self.updateTime);
			self.Core.GetRuleBroadcast(out self.disable);

			self.AddComponent(out self.worldUpdate, frameTime).Run();

			self.afterTime = DateTime.Now;
			self.m_Thread = new DispatcherTimer();
			self.m_Thread.Interval = new(frameTime);
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

		[NodeRule(nameof(RemoveRule<AvaloniaWorldHeart>))]
		private static void OnRemoveRule(this AvaloniaWorldHeart self)
		{
			self.m_Thread?.Stop();
			self.m_Thread = null;

			self.worldUpdate = null;

			self.enable = null;
			self.update = null;
			self.updateTime = null;
			self.disable = null;
		}

		[NodeRule(nameof(UpdateTimeRule<AvaloniaWorldHeart>))]
		private static void OnUpdateTimeRule(this AvaloniaWorldHeart self, TimeSpan deltaTime)
		{
			self.enable?.Send();
			self.update?.Send();
			self.updateTime?.Send(deltaTime);
			self.disable?.Send();
		}
	}
}
