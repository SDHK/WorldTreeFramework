/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/28 03:56:05

* 描述： 世界之心：C#多线程

*/

using System;
using System.Threading;

namespace WorldTree
{
	/// <summary>
	/// 世界之心：C#多线程
	/// </summary>
	public class WorldHeart : WorldHeartBase
		, AsComponentBranch
		, ComponentOf<WorldTreeRoot>
		, AsAwake<int>
	{
		/// <summary>
		/// 线程退出标记
		/// </summary>
		public bool isExit = false;

		/// <summary>
		/// 运行线程
		/// </summary>
		public Thread m_Thread;

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
		public GlobalRuleActuator<Enable> enable;
		/// <summary>
		/// 全局事件法则 Disable
		/// </summary>
		public GlobalRuleActuator<Disable> disable;
		/// <summary>
		/// 全局事件法则 Update
		/// </summary>
		public GlobalRuleActuator<Update> update;
		/// <summary>
		/// 全局事件法则 UpdateTime
		/// </summary>
		public GlobalRuleActuator<UpdateTime> updateTime;

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

		/// <summary>
		/// 线程刷新
		/// </summary>
		public void Update()
		{
			while (!isExit)
			{
				if (isRun)
				{
					Thread.Sleep(frameTime);
					worldUpdate?.Update(DateTime.Now - afterTime);
				}
				afterTime = DateTime.Now;
			}
		}
	}

	public static class WorldHeartRule
	{
		private class Awake : AwakeRule<WorldHeart, int>
		{
			protected override void Execute(WorldHeart self, int frameTime)
			{
				self.frameTime = frameTime;

				self.Core.GetOrNewGlobalRuleActuator(out self.enable);
				self.Core.GetOrNewGlobalRuleActuator(out self.update);
				self.Core.GetOrNewGlobalRuleActuator(out self.updateTime);
				self.Core.GetOrNewGlobalRuleActuator(out self.disable);

				self.AddComponent(out self.worldUpdate, frameTime).Run();

				self.afterTime = DateTime.Now;
				self.m_Thread = new Thread(self.Update);
				self.m_Thread.Start();
			}
		}

		private class Remove : RemoveRule<WorldHeart>
		{
			protected override void Execute(WorldHeart self)
			{
				self.isExit = true;
				self.m_Thread = null;

				self.worldUpdate = null;

				self.enable = null;
				self.update = null;
				self.updateTime = null;
				self.disable = null;
			}
		}

		private class UpdateTime : UpdateTimeRule<WorldHeart>
		{
			protected override void Execute(WorldHeart self, TimeSpan deltaTime)
			{
				self.enable?.Send();
				self.update?.Send();
				self.updateTime?.Send(deltaTime);
				self.disable?.Send();
			}
		}
	}
}