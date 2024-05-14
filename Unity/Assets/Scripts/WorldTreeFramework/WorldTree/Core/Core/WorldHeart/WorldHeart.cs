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

		public DateTime afterTime;

		#region 世界脉搏

		public WorldPulse<UpdateTime> worldUpdate;

		#endregion

		#region 全局事件法则

		public GlobalRuleActuator<Enable> enable;
		public GlobalRuleActuator<Disable> disable;
		public GlobalRuleActuator<Update> update;
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
		private class AwakeRule : AwakeRule<WorldHeart, int>
		{
			protected override void Execute(WorldHeart self, int frameTime)
			{
				self.frameTime = frameTime;

				self.GetOrNewGlobalRuleActuator(out self.enable);
				self.GetOrNewGlobalRuleActuator(out self.update);
				self.GetOrNewGlobalRuleActuator(out self.updateTime);
				self.GetOrNewGlobalRuleActuator(out self.disable);

				self.AddComponent(out self.worldUpdate, frameTime).Run();

				self.afterTime = DateTime.Now;
				self.m_Thread = new Thread(self.Update);
				self.m_Thread.Start();
			}
		}

		private class RemoveRule : RemoveRule<WorldHeart>
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

		private class UpdateTimeRule : UpdateTimeRule<WorldHeart>
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