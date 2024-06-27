/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/28 19:56

* 描述： 游戏时间管理器，通过累加增量时间计时，用于全局暂停，加速，减速等
*
* 这个时间可能比真实时间慢，属于框架内部虚拟的时间，不受外部时间影响
*
* 游戏暂停时，时间不会增加，因为它是通过增量时间计算的
*

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