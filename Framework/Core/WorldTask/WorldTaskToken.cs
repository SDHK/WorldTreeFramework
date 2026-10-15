/****************************************

* 作者：闪电黑客
* 日期：2026/10/14

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 令牌状态
	/// </summary>
	public enum WorldTokenState
	{
		/// <summary>
		/// 运行
		/// </summary>
		Running,

		/// <summary>
		/// 暂停
		/// </summary>
		Stop,

		/// <summary>
		/// 取消
		/// </summary>
		Cancel,
	}

	/// <summary>
	/// 任务令牌。监听者是委托，不是法则。
	/// </summary>
	public class WorldTaskToken : Unit, AsRule<WorldTaskTimeout>
	{
		/// <summary>
		/// 当前状态
		/// </summary>
		public WorldTokenState State;

		/// <summary>
		/// 暂停时拦住的任务。继续或取消时放行。
		/// </summary>
		public WorldTaskBase PausedTask;

		/// <summary>
		/// 状态变化监听
		/// </summary>
		private Action<WorldTokenState> listeners;

		/// <summary>
		/// 订阅状态变化。已取消的令牌不再接收。
		/// </summary>
		public void Add(Action<WorldTokenState> listener)
		{
			if (listener == null || State == WorldTokenState.Cancel) return;
			listeners += listener;
		}

		/// <summary>
		/// 取消订阅
		/// </summary>
		public void Remove(Action<WorldTokenState> listener) => listeners -= listener;

		/// <summary>
		/// 继续
		/// </summary>
		public void Continue()
		{
			State = WorldTokenState.Running;
			listeners?.Invoke(State);
			WorldTaskBase paused = PausedTask;
			PausedTask = null;
			paused?.SetCompleted();
		}

		/// <summary>
		/// 暂停
		/// </summary>
		public void Stop()
		{
			State = WorldTokenState.Stop;
			listeners?.Invoke(State);
		}

		/// <summary>
		/// 取消。监听者和被暂停的任务都会结束。
		/// </summary>
		public void Cancel()
		{
			if (State == WorldTokenState.Cancel) return;
			State = WorldTokenState.Cancel;
			Action<WorldTokenState> notify = listeners;
			listeners = null;
			WorldTaskBase paused = PausedTask;
			PausedTask = null;
			notify?.Invoke(State);
			if (paused != null && !paused.IsCompleted) paused.SetCompleted();
		}

		public override void OnCreate()
		{
			State = WorldTokenState.Running;
			PausedTask = null;
			listeners = null;
		}

		public override void OnDispose()
		{
			PausedTask = null;
			listeners = null;
			base.OnDispose();
		}
	}

	/// <summary>
	/// 令牌到期取消
	/// </summary>
	public static class WorldTaskTokenTimeoutRule
	{
		class Timeout : SendRule<WorldTaskToken, WorldTaskTimeout>
		{
			/// <summary>
			/// 到期取消。已经取消的令牌不再处理。
			/// </summary>
			protected override void Execute(WorldTaskToken self)
			{
				if (self.IsDisposed || self.State == WorldTokenState.Cancel) return;
				self.Cancel();
			}
		}
	}
}
