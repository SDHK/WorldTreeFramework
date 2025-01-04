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
	public class GameTimeManager : Node, ComponentOf<WorldTreeCore>
		, AsAwake
	{
		/// <summary>
		/// 时间缩放比例
		/// </summary>
		public float timeScale = 1;

		/// <summary>
		/// 当前帧时间
		/// </summary>
		public TimeSpan FrameTime => frameTime;

		/// <summary>
		/// 累计游戏时间
		/// </summary>
		public TimeSpan TotalTime => totalTime;

		/// <summary>
		/// 无缩放的当前帧时间
		/// </summary>
		public TimeSpan UnscaleFrameTime => unscaleFrameTime;

		/// <summary>
		/// 无缩放的累计游戏时间
		/// </summary>
		public TimeSpan UnscaleTotalTimeTime => unscaleTotalTime;

		/// <summary>
		/// 当前帧时间
		/// </summary>
		private TimeSpan frameTime;

		/// <summary>
		/// 累计游戏时间
		/// </summary>
		private TimeSpan totalTime;

		/// <summary>
		/// 无缩放的当前帧时间
		/// </summary>
		private TimeSpan unscaleFrameTime;

		/// <summary>
		/// 无缩放的累计游戏时间
		/// </summary>
		private TimeSpan unscaleTotalTime;

		public GameTimeManager()
		{
			totalTime = TimeSpan.MinValue;
			unscaleTotalTime = TimeSpan.MinValue;
		}

		/// <summary>
		/// 累加时间
		/// </summary>
		/// <param name="deltaTime">时间增量</param>
		public void UpdateTime(TimeSpan deltaTime)
		{
			unscaleFrameTime = deltaTime;
			unscaleTotalTime += deltaTime;
			frameTime = deltaTime * timeScale;
			totalTime += frameTime;
		}
	}

	public static partial class GameTimeManagerRule
	{
		private class UpdateTime : UpdateTimeRule<GameTimeManager>
		{
			protected override void Execute(GameTimeManager self, TimeSpan arg1)
			{
				self.UpdateTime(arg1);
			}
		}
	}
}