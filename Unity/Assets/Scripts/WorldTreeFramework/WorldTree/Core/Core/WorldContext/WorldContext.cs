/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:51

* 描述： 世界环境

*/
using System;
using System.Threading;

namespace WorldTree
{
	public struct WorldContextWorkRequest
	{
		private readonly SendOrPostCallback m_DelagateCallback;
		private readonly object m_DelagateState;

		public WorldContextWorkRequest(SendOrPostCallback callback, object state)
		{
			m_DelagateCallback = callback;
			m_DelagateState = state;
		}

		public void Invoke()
		{
			m_DelagateCallback?.Invoke(m_DelagateState);
		}

	}

	/// <summary>
	/// 世界环境
	/// </summary>
	/// <remarks>线程的上下文</remarks>
	public partial class WorldContext : SynchronizationContext, INode
		, AsChildBranch
		, ComponentOf<WorldTreeRoot>
		, AsAwake
	{

		public TreeConcurrentQueue<WorldContextWorkRequest> m_Queue;

		public override void Post(SendOrPostCallback callback, object state)
		{
			this.m_Queue.Enqueue(new(callback, state));
		}

		public void Post(Action action)
		{
			this.m_Queue.Enqueue(new WorldContextWorkRequest((x) => action(), null));
		}
	}

	public static class WorldContextRule
	{
		class AddRule : AddRule<WorldContext>
		{
			protected override void Execute(WorldContext self)
			{
				self.AddChild(out self.m_Queue);
			}
		}

		class UpdateRule : UpdateRule<WorldContext>
		{
			protected override void Execute(WorldContext self)
			{
				while (true)
				{
					if (!self.m_Queue.TryDequeue(out WorldContextWorkRequest workRequest)) return;
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
