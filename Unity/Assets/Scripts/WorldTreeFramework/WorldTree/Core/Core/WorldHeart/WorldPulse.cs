/****************************************

* 作者： 闪电黑客
* 日期： 2024/01/09 08:13:32

* 描述： 世界脉搏

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 世界脉搏
	/// </summary>
	public class WorldPulse<R> : WorldPulseBase
		, ComponentOf<WorldHeartBase>
		, AsRule<IAwakeRule<int>>
		where R : ISendRuleBase<TimeSpan>
	{
		/// <summary>
		/// 核心法则
		/// </summary>
		public IRuleList<R> m_RuleList;

		/// <summary>
		/// 单帧运行
		/// </summary>
		public override void OneFrame()
		{
			isRun = false;
			m_RuleList?.Send(Parent, m_Time);
		}

		/// <summary>
		/// 暂停
		/// </summary>
		public override void Pause()
		{
			isRun = false;
		}

		/// <summary>
		/// 运行
		/// </summary>
		public override void Run()
		{
			isRun = true;
		}

		/// <summary>
		/// 更新:毫秒时间间隔
		/// </summary>
		public override void Update(TimeSpan deltaTime)
		{
			m_Time += deltaTime;
			if (m_Time.TotalMilliseconds >= frameTime * 0.001)
			{
				if (isRun) m_RuleList?.Send(Parent, m_Time);
				m_Time = TimeSpan.Zero;
			}
		}
	}

	public static class WorldPulseRule
	{
		class AwakeRuleGeneric<R> : AwakeRule<WorldPulse<R>, int>
			where R : ISendRuleBase<TimeSpan>
		{
			protected override void Execute(WorldPulse<R> self, int FrameTime)
			{
				self.frameTime = FrameTime;
				self.Core.RuleManager.TryGetRuleList(self.Parent.Type, out self.m_RuleList);
			}
		}

		class RemoveRuleGeneric<R> : RemoveRule<WorldPulse<R>>
			where R : ISendRuleBase<TimeSpan>
		{
			protected override void Execute(WorldPulse<R> self)
			{
				self.frameTime = 0;
				self.m_Time = TimeSpan.Zero;
				self.m_RuleList = null;
			}
		}
	}
}
