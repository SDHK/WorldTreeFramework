/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:51

* 描述： 世界环境

*/
using System;
using System.Threading;

namespace WorldTree
{
	/// <summary>
	/// 世界环境的工作请求数据
	/// </summary>
	public struct WorldContextWorkRequest
	{
		/// <summary>
		/// 委托回调
		/// </summary>
		private readonly SendOrPostCallback delagateCallback;

		/// <summary>
		/// 委托状态
		/// </summary>
		private readonly object delagateState;

		public WorldContextWorkRequest(SendOrPostCallback callback, object state)
		{
			delagateCallback = callback;
			delagateState = state;
		}

		/// <summary>
		/// 执行委托
		/// </summary>
		public void Invoke()
		{
			delagateCallback?.Invoke(delagateState);
		}
	}

	/// <summary>
	/// 世界环境
	/// </summary>
	/// <remarks>线程的上下文</remarks>
	public partial class WorldContext : SynchronizationContext, INode, IListenerIgnorer
		, AsChildBranch
		, CoreManagerOf<WorldLine>
		, AsAwake
	{
		/// <summary>
		/// 请求队列
		/// </summary>
		public TreeConcurrentQueue<WorldContextWorkRequest> queue;

		public override void Post(SendOrPostCallback callback, object state)
		{
			this.queue.Enqueue(new(callback, state));
		}

		/// <summary>
		/// 往线程中注入委托
		/// </summary>
		public void Post(Action action)
		{
			this.queue.Enqueue(new WorldContextWorkRequest((x) => action(), null));
		}
	}

	public static class WorldContextRule
	{
		class AddRule : AddRule<WorldContext>
		{
			protected override void Execute(WorldContext self)
			{
				self.AddChild(out self.queue);
			}
		}

		class UpdateRule : UpdateRule<WorldContext>
		{
			protected override void Execute(WorldContext self)
			{
				while (true)
				{
					if (!self.queue.TryDequeue(out WorldContextWorkRequest workRequest)) return;
					try
					{
						workRequest.Invoke();
					}
					catch (Exception e)
					{
						self.LogError(e);
					}
				}
			}
		}
	}
}
