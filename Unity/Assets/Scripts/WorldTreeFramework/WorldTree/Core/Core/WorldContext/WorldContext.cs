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
	[INodeProxy]
	public partial class WorldContext : SynchronizationContext, INode
		, AsChildBranch
		, CoreManagerOf<WorldLine>
		, AsRule<Awake>
	{
		/// <summary>
		/// 请求队列
		/// </summary>
		public TreeQueue<WorldContextRequest> contextQueue;

		/// <summary>
		/// 往线程中注入委托 
		/// </summary>
		public void Post(Action<WorldContextData> callback, WorldContextData data)
		{
			contextQueue.Enqueue(new(callback, data));
		}
		/// <summary>
		/// 往线程中注入委托
		/// </summary>
		public void Post(Action action)
		{
			contextQueue.Enqueue(new((obj) => ((Action)obj.CallBack)(), new() { CallBack = action }));
		}
		public override void Post(SendOrPostCallback callback, object state)
		{
			contextQueue.Enqueue(new((obj) => ((SendOrPostCallback)obj.CallBack)(obj.Object), new(state) { CallBack = callback }));
		}
	}
	public static class WorldContextRule
	{
		class AddRule : AddRule<WorldContext>
		{
			protected override void Execute(WorldContext self)
			{
				self.AddChild(out self.contextQueue);
			}
		}
		class UpdateRule : UpdateRule<WorldContext>
		{
			protected override void Execute(WorldContext self)
			{
				while (self.contextQueue.TryDequeue(out WorldContextRequest workRequest))
				{
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
