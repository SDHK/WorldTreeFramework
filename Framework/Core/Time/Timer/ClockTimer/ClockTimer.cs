/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/29

* 描述： 可重启的节点倒计时器。到点发送规则，Restart 会删除旧计时并重新注册。

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 时钟定时器
	/// </summary>
	public class ClockTimer : Node
		, ChildOf<INode>
		, AsRule<Awake>
	{
		/// <summary>
		/// 当前定时器 Id
		/// </summary>
		private long timerId;

		/// <summary>
		/// 到点时间（Utc Ticks）
		/// </summary>
		private long dueTick;

		/// <summary>
		/// 到点规则类型
		/// </summary>
		public Type RuleType { get; private set; } = typeof(ClockTimerTimeout);

		/// <summary>
		/// 规则目标；空表示父节点
		/// </summary>
		public INode Target { get; private set; }

		/// <summary>
		/// 设置到点规则；正在计时时保留剩余时间
		/// </summary>
		public void SetRule<R>(INode target = null) where R : ISendRule
		{
			INode node = target ?? Parent;
			long ruleCode = World.Line.Core.TypeInfo.TypeToCode(typeof(R));

			if (!World.Line.Core.RuleManager.TryGetRuleList(node.Type, ruleCode, out _))
				throw new Exception($"目标节点没有规则: {typeof(R).Name}");

			bool running = dueTick > World.Line.Core.RealTimeManager.UtcNow.Ticks;
			long keepDueTick = dueTick;

			Stop();

			RuleType = typeof(R);
			Target = target;

			if (running)
			{
				dueTick = keepDueTick;
				AddTicker();
			}
		}

		/// <summary>
		/// 重启倒计时
		/// </summary>
		public void Restart(TimeSpan delay)
		{
			Stop();
			if (delay <= TimeSpan.Zero) delay = TimeSpan.FromTicks(1);

			dueTick = World.Line.Core.RealTimeManager.UtcNow.Ticks + delay.Ticks;
			AddTicker();
		}

		/// <summary>
		/// 注册 ticker
		/// </summary>
		private void AddTicker()
		{
			INode node = Target ?? Parent;
			long ruleCode = World.Line.Core.TypeInfo.TypeToCode(RuleType);

			timerId = World.Line.WorldTimeManager.Timer
				.AddTicker(dueTick, node, ruleCode);

			if (timerId < 0)
				throw new Exception($"目标节点没有规则: {RuleType.Name}");
		}

		/// <summary>
		/// 停止倒计时
		/// </summary>
		public void Stop()
		{
			if (timerId == 0)
			{
				dueTick = 0;
				return;
			}

			World.Line.WorldTimeManager.Timer.RemoveTicker(timerId);
			timerId = 0;
			dueTick = 0;
		}

		public override void OnDispose()
		{
			Stop();
			base.OnDispose();
		}
	}

	/// <summary>
	/// 时钟定时器到点规则
	/// </summary>
	public interface ClockTimerTimeout : ISendRule { }

}
