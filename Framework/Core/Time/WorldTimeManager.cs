/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 世界时间管理器
	/// </summary>
	public class WorldTimeManager : Node
		, FieldOf<WorldLine>
		, AsFieldBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 真实时间定时器 
		/// </summary>
		public CascadeTicker Timer;

		/// <summary>
		/// 世界时间定时器 
		/// </summary>
		public CascadeTicker WorldTimer;

		/// <summary>
		/// 定帧器 
		/// </summary>
		public CascadeTicker Framer;

		/// <summary>
		/// WorldTask 的委托定时器。和法则定时器分开。
		/// </summary>
		private TimerCallback[] timerCallbacks;

		/// <summary>
		/// 委托定时器数量
		/// </summary>
		private int timerCount;

		/// <summary>
		/// 委托定时器槽位
		/// </summary>
		private struct TimerCallback
		{
			/// <summary>
			/// 到期的 Utc Ticks
			/// </summary>
			public long DueTicks;

			/// <summary>
			/// 到期回调
			/// </summary>
			public Action Callback;

			/// <summary>
			/// 是否有效
			/// </summary>
			public bool Active;
		}

		/// <summary>
		/// 累计帧数 
		/// </summary>
		public long TotalFrames => totalFrames;

		/// <summary>
		/// 当前帧时间
		/// </summary>
		public TimeSpan FrameTime => frameTime;

		/// <summary>
		/// 累计游戏时间
		/// </summary>
		public TimeSpan TotalTime => totalTime;


		/// <summary>
		/// 当前帧时间
		/// </summary>
		private TimeSpan frameTime;

		/// <summary>
		/// 累计游戏时间
		/// </summary>
		private TimeSpan totalTime;

		/// <summary>
		/// 累计帧数 
		/// </summary>
		private long totalFrames;

		public WorldTimeManager()
		{
			totalTime = TimeSpan.Zero;
		}

		/// <summary>
		/// 累加时间
		/// </summary>
		/// <param name="deltaTime">时间增量</param>
		public void UpdateTime(TimeSpan deltaTime)
		{
			totalFrames++;
			frameTime = deltaTime;
			totalTime += frameTime;
		}

		/// <summary>
		/// 添加委托定时器，时序为绝对 Utc Ticks。返回槽位，可用来取消。
		/// </summary>
		public int AddTimerCallback(long dueTicks, Action callback)
		{
			if (timerCallbacks == null)
			{
				timerCallbacks = new TimerCallback[4];
				timerCount = 0;
			}

			for (int i = 0; i < timerCount; i++)
			{
				if (timerCallbacks[i].Active) continue;
				timerCallbacks[i].DueTicks = dueTicks;
				timerCallbacks[i].Callback = callback;
				timerCallbacks[i].Active = true;
				return i;
			}

			if (timerCount >= timerCallbacks.Length)
			{
				Array.Resize(ref timerCallbacks, timerCallbacks.Length * 2);
			}

			timerCallbacks[timerCount] = new TimerCallback
			{
				DueTicks = dueTicks,
				Callback = callback,
				Active = true,
			};
			return timerCount++;
		}

		/// <summary>
		/// 取消委托定时器
		/// </summary>
		public void RemoveTimerCallback(int id)
		{
			if (timerCallbacks == null || id < 0 || id >= timerCount) return;
			timerCallbacks[id].Active = false;
			timerCallbacks[id].Callback = null;
		}

		/// <summary>
		/// 执行已经到期的委托定时器
		/// </summary>
		public void UpdateTimerCallbacks(long nowTicks)
		{
			if (timerCallbacks == null) return;
			int count = timerCount;
			for (int i = 0; i < count; i++)
			{
				if (!timerCallbacks[i].Active || timerCallbacks[i].DueTicks > nowTicks) continue;
				Action callback = timerCallbacks[i].Callback;
				timerCallbacks[i].Active = false;
				timerCallbacks[i].Callback = null;
				try
				{
					callback?.Invoke();
				}
				catch (Exception e)
				{
					this.LogError(e);
				}
			}
		}
	}

	public static partial class WorldTimeManagerRule
	{
		private class AwakeRule : AwakeRule<WorldTimeManager>
		{
			protected override void Execute(WorldTimeManager self)
			{
				self.AddField(out self.Timer);
				self.AddField(out self.WorldTimer);
				self.AddField(out self.Framer);
			}
		}

		private class UpdateTime : UpdateTimeRule<WorldTimeManager>
		{
			protected override void Execute(WorldTimeManager self, TimeSpan arg1)
			{
				self.UpdateTime(arg1);
				long utcTicks = self.World.Line.Core.RealTimeManager.UtcNow.Ticks;
				self.Timer.Update(utcTicks);
				self.UpdateTimerCallbacks(utcTicks);
				self.WorldTimer.Update(self.TotalTime.Ticks);
				self.Framer.Update(self.TotalFrames);
			}
		}

		/// <summary>
		/// 添加定帧器 
		/// </summary>
		public static long AddFramer<R>(this WorldTimeManager self, long frame, INode node, TreeTaskToken token = null)
			where R : ISendRule
		{
			return self.Framer.AddTicker<R>(frame, node, token);
		}

		/// <summary>
		/// 添加定帧延迟
		/// </summary>
		public static long AddFramerDelay<R>(this WorldTimeManager self, long delayFrame, INode node, TreeTaskToken token = null)
			where R : ISendRule
		{
			return self.Framer.AddTicker<R>(self.TotalFrames + delayFrame, node, token);
		}

		/// <summary>
		/// 添加真实时间定时器，时序为绝对 Utc Ticks
		/// </summary>
		public static long AddTimer<R>(this WorldTimeManager self, long utcTicks, INode node, TreeTaskToken token = null)
			where R : ISendRule
		{
			return self.Timer.AddTicker<R>(utcTicks, node, token);
		}

		/// <summary>
		/// 添加真实时间延迟，时序为当前 Utc Ticks 加上延迟
		/// </summary>
		public static long AddTimerDelay<R>(this WorldTimeManager self, long delayTicks, INode node, TreeTaskToken token = null)
			where R : ISendRule
		{
			return self.Timer.AddTicker<R>(self.World.Line.Core.RealTimeManager.UtcNow.Ticks + delayTicks, node, token);
		}
	}
}