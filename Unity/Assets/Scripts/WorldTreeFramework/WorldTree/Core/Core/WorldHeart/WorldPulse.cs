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
		, AsAwake<int>
		where R : ISendRule<TimeSpan>
	{
		/// <summary>
		/// 核心法则
		/// </summary>
		public IRuleList<R> ruleList;

		/// <summary>
		/// 单帧运行
		/// </summary>
		public override void OneFrame()
		{
			isRun = false;
			ruleList?.Send(Parent, time);
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
			time += deltaTime;
			if (time.TotalMilliseconds >= frameTime)
			{
				if (isRun) ruleList?.Send(Parent, time);
				time = TimeSpan.Zero;
			}
		}
	}

	public static class WorldPulseRule
	{
		class Awake<R> : AwakeRule<WorldPulse<R>, int>
			where R : ISendRule<TimeSpan>
		{
			protected override void Execute(WorldPulse<R> self, int frameTime)
			{
				self.frameTime = frameTime;
				self.Core.RuleManager.TryGetRuleList(self.Parent.Type, out self.ruleList);
			}
		}

		class Remove<R> : RemoveRule<WorldPulse<R>>
			where R : ISendRule<TimeSpan>
		{
			protected override void Execute(WorldPulse<R> self)
			{
				self.frameTime = 0;
				self.time = TimeSpan.Zero;
				self.ruleList = null;
			}
		}
	}
}
