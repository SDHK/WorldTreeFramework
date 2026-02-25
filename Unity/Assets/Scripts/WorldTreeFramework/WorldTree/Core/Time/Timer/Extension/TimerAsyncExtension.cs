/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

*/
using Codice.Client.Common;
using System;

namespace WorldTree
{
	/// <summary>
	/// 节点定时器异步扩展
	/// </summary>
	public static class TimerAsyncExtension
	{
		/// <summary>
		/// 异步延迟帧
		/// </summary>
		public static async TreeTask AsyncYield(this INode self, int delayFrame = 0)
		{
			TreeTask asyncTask = self.AddTemp(out TreeTask _);
			// 捕获令牌
			var token = await self.TreeTaskTokenCatch();
			// 添加定帧器
			self.Core.GameTimeManager.AddFramerDelay<TreeTaskSetResuIt>(delayFrame, asyncTask, token);
			// 等待异步执行
			await asyncTask;
		}


		/// <summary>
		/// 异步延迟秒
		/// </summary>
		public static async TreeTask AsyncDelay(this INode self, TimeSpan time)
		{
			TreeTask asyncTask = self.AddTemp(out TreeTask _);
			// 捕获令牌
			var token = await self.TreeTaskTokenCatch();
			// 添加定时器
			self.Core.RealTimeManager.AddTimerDelay<TreeTaskSetResuIt>(time.Ticks, asyncTask, token);
			// 等待异步执行
			await asyncTask;
		}
	}
}
