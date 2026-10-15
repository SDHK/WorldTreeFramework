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