
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 异步任务构建器
* 
* 实现了内联传递令牌功能
* 
* 将会在 AwaitUnsafeOnCompleted 方法中将Task进行互相关联，并设置令牌。
* 
* 实现通过 await TreeTaskTokenCatch 在异步方法中凭空获取到令牌。
* 

*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace WorldTree.Internal
{
	/// <summary>
	/// 异步任务构建器
	/// </summary>
	public struct TreeTaskMethodBuilder
	{
		private TreeTaskStateMachine treeTaskStateMachine;
		private TreeTask task;
		// 1. Static Create method.

		[DebuggerHidden]
		public static TreeTaskMethodBuilder Create()
		{
			return new TreeTaskMethodBuilder();
		}

		// 2. TaskLike Task property.
		[DebuggerHidden]
		public TreeTask Task
		{
			get
			{
				return task;
			}
		}

		// 3. SetException
		[DebuggerHidden]
		public void SetException(Exception exception)
		{
			task.SetException(exception);
			if (this.treeTaskStateMachine != null)
			{
				this.treeTaskStateMachine.Dispose();
				this.treeTaskStateMachine = null;
			}
		}

		// 4. SetResult
		public void SetResult()
		{
			
			//task.Log($"[{task.Id}] 状态机任务完成!!!");
			task.SetResult();
			if (this.treeTaskStateMachine != null)
			{
				this.treeTaskStateMachine.Dispose();
				this.treeTaskStateMachine = null;
			}
		}

		// 5. AwaitOnCompleted
		[DebuggerHidden]

		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
		}

		// 6. AwaitUnsafeOnCompleted
		[SecuritySafeCritical]
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{

			//if (task == null)//发生在异步方法等待的第一个 await TreeTask 时。
			//{
			//	//新建一个TreeTask
			//	awaiter.Parent.AddTemp(out task);
			//	if (treeTaskStateMachine == null) awaiter.Parent.AddTemp(out treeTaskStateMachine).SetStateMachine(ref stateMachine);
			//	//任务关联
			//	task.Log($"新建任务[{task.Id}] ，当前状态机：{stateMachine}");

			//}
			//awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);

			//if (task.m_TreeTaskToken.Value != null)
			//{
			//	task.Log($"当前任务[{task.Id}] 有令牌，设置令牌[{task.m_TreeTaskToken.Value.Id}]给 Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");
			//	//如果当前任务有令牌，那么设置给传入的awaiter，同时会设置给所有 没有令牌 的关联任务。
			//	awaiter.SetToken(task.m_TreeTaskToken);
			//	awaiter.FindSyncTaskSetCompleted();
			//	return;
			//}

			//task.m_RelevanceTask = awaiter;
			//task.Log($"当前任务[{task.Id}] 关联=> Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");

			if (task == null)//发生在异步方法等待的第一个 await TreeTask 时。
			{
				//新建一个TreeTask
				awaiter.Parent.AddTemp(out task);
				if (treeTaskStateMachine == null) awaiter.Parent.AddTemp(out treeTaskStateMachine).SetStateMachine(ref stateMachine);
				//任务关联
				//task.Log($"新建任务[{task.Id}] 关联=> Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");
				task.m_RelevanceTask = awaiter;
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);

			}
			else
			{
				if (task.m_TreeTaskToken.Value != null)
				{
					//task.Log($"当前任务[{task.Id}] 有令牌，设置令牌[{task.m_TreeTaskToken.Value.Id}]给 Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");
					//如果当前任务有令牌，那么设置给传入的awaiter，同时会设置给所有 没有令牌 的关联任务。
					awaiter.SetToken(task.m_TreeTaskToken);
				}
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
				awaiter.FindSyncTaskSetCompleted();
			}

		}

		// 7. Start
		[DebuggerHidden]
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			stateMachine.MoveNext();
		}

		// 8. SetStateMachine
		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}
	}



	public struct TreeTaskMethodBuilder<T>
	{
		private TreeTaskStateMachine treeTaskStateMachine;

		private TreeTask<T> task;
		// 1. Static Create method.

		[DebuggerHidden]
		public static TreeTaskMethodBuilder<T> Create()
		{
			TreeTaskMethodBuilder<T> builder = new TreeTaskMethodBuilder<T>();
			return builder;
		}

		// 2. TaskLike Task property.
		[DebuggerHidden]
		public TreeTask<T> Task
		{
			get
			{
				return task;
			}
		}

		// 3. SetException
		[DebuggerHidden]
		public void SetException(Exception exception)
		{
			task.SetException(exception);

			if (this.treeTaskStateMachine != null)
			{
				this.treeTaskStateMachine.Dispose();
				this.treeTaskStateMachine = null;
			}
		}

		// 4. SetResult
		[DebuggerHidden]

		public void SetResult(T ret)
		{
			//task.Log($"[{task.Id}] 状态机任务完成!!!");
			task.SetResult(ret);
			if (this.treeTaskStateMachine != null)
			{
				this.treeTaskStateMachine.Dispose();
				this.treeTaskStateMachine = null;
			}
		}

		// 5. AwaitOnCompleted
		[DebuggerHidden]

		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
		}

		// 6. AwaitUnsafeOnCompleted
		[SecuritySafeCritical]
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			//if (task == null)//发生在异步方法等待的第一个 await TreeTask 时。
			//{
			//	//新建一个TreeTask
			//	awaiter.Parent.AddTemp(out task);
			//	if (treeTaskStateMachine == null) awaiter.Parent.AddTemp(out treeTaskStateMachine).SetStateMachine(ref stateMachine);
			//	//任务关联
			//	task.Log($"新建任务[{task.Id}] ，当前状态机：{stateMachine}");

			//}
			//awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);

			//if (task.m_TreeTaskToken.Value != null)
			//{
			//	task.Log($"当前任务[{task.Id}] 有令牌，设置令牌[{task.m_TreeTaskToken.Value.Id}]给 Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");
			//	//如果当前任务有令牌，那么设置给传入的awaiter，同时会设置给所有 没有令牌 的关联任务。
			//	awaiter.SetToken(task.m_TreeTaskToken);
			//	awaiter.FindSyncTaskSetCompleted();
			//	return;
			//}

			//task.m_RelevanceTask = awaiter;
			//task.Log($"当前任务[{task.Id}] 关联=> Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");

			if (task == null)//发生在异步方法等待的第一个 await TreeTask 时。
			{
				//新建一个TreeTask
				awaiter.Parent.AddTemp(out task);
				if (treeTaskStateMachine == null) awaiter.Parent.AddTemp(out treeTaskStateMachine).SetStateMachine(ref stateMachine);
				//任务关联
				//task.Log($"新建任务[{task.Id}] 关联=> Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");
				task.m_RelevanceTask = awaiter;
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);

			}
			else
			{
				if (task.m_TreeTaskToken.Value != null)
				{
					//task.Log($"当前任务[{task.Id}] 有令牌，设置令牌[{task.m_TreeTaskToken.Value.Id}]给 Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");
					//如果当前任务有令牌，那么设置给传入的awaiter，同时会设置给所有 没有令牌 的关联任务。
					awaiter.SetToken(task.m_TreeTaskToken);
				}
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
				awaiter.FindSyncTaskSetCompleted();
			}
		}

		// 7. Start
		[DebuggerHidden]
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			stateMachine.MoveNext();
		}

		// 8. SetStateMachine
		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}
	}


}
