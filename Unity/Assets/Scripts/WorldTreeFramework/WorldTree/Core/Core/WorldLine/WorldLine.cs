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
	public struct WorldLineWorkRequest
	{
		/// <summary>
		/// 委托回调
		/// </summary>
		private readonly SendOrPostCallback delagateCallback;

		/// <summary>
		/// 委托状态
		/// </summary>
		private readonly object delagateState;

		public WorldLineWorkRequest(SendOrPostCallback callback, object state)
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
	/// 世界线
	/// </summary>
	/// <remarks>线程的上下文</remarks>
	public partial class WorldLine : SynchronizationContext, INode, IListenerIgnorer
		, AsChildBranch
		, CoreManagerOf<WorldTreeCore>
		, AsAwake
	{
		/// <summary>
		/// 请求队列
		/// </summary>
		public TreeConcurrentQueue<WorldLineWorkRequest> queue;

		public override void Post(SendOrPostCallback callback, object state)
		{
			this.queue.Enqueue(new(callback, state));
		}

		/// <summary>
		/// 往线程中注入委托
		/// </summary>
		public void Post(Action action)
		{
			this.queue.Enqueue(new WorldLineWorkRequest((x) => action(), null));
		}
	}

	public static class WorldContextRule
	{
		class AddRule : AddRule<WorldLine>
		{
			protected override void Execute(WorldLine self)
			{
				self.AddChild(out self.queue);
			}
		}

		class UpdateRule : UpdateRule<WorldLine>
		{
			protected override void Execute(WorldLine self)
			{
				while (true)
				{
					if (!self.queue.TryDequeue(out WorldLineWorkRequest workRequest)) return;
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
