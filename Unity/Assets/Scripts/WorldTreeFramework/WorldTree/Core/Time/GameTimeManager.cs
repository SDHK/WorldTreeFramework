/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 游戏时间管理器
	/// </summary>
	public class GameTimeManager : Node
		, CoreManagerOf<WorldLine>
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 定时器 
		/// </summary>
		public CascadeTicker Timer;

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


		public GameTimeManager()
		{
			totalTime = TimeSpan.MinValue;
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

	public static partial class GameTimeManagerRule
	{
		private class AwakeRule : AwakeRule<GameTimeManager>
		{
			protected override void Execute(GameTimeManager self)
			{
				self.AddChild(out self.Timer);
				self.AddChild(out self.Framer);
			}
		}

		private class UpdateTime : UpdateTimeRule<GameTimeManager>
		{
			protected override void Execute(GameTimeManager self, TimeSpan arg1)
			{
				self.UpdateTime(arg1);
				self.Timer.Update(self.TotalTime.Ticks);
				self.Framer.Update(self.TotalFrames);
			}
		}
	}
}