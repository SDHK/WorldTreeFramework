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
	/// 世界环境
	/// </summary>
	/// <remarks>线程的上下文</remarks>
	public partial class WorldContext : SynchronizationContext, INode
		,ComponentOf<WorldTreeRoot>
		,AsRule<IAwakeRule>
	{

		public TreeConcurrentQueue<Action> m_Queue;

		public override void Post(SendOrPostCallback callback, object state)
		{
			this.Post(() => callback(state));
		}

		public void Post(Action action)
		{
			this.m_Queue.Enqueue(action);
		}
	}

	public static class WorldContextRule
	{
		class AddRule : AddRule<WorldContext>
		{
			protected override void Execute(WorldContext self)
			{
				self.AddComponent(out self.m_Queue);
			}
		}

		class UpdateRule : UpdateRule<WorldContext>
		{
			protected override void Execute(WorldContext self)
			{
				while (true)
				{
					if (!self.m_Queue.TryDequeue(out Action action)) return;
					try
					{
						action();
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
