
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 异步任务构建器

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
		private ITreeTaskStateMachine treeTaskStateMachine;
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
			task.LogError(exception);

			//TreeTaskBase.ExceptionHandler?.Invoke(exception);
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


		//在调用异步方法时，方法本身不是第一个Task,第一个Task是方法内部生成的一个Task。

		// 6. AwaitUnsafeOnCompleted
		[SecuritySafeCritical]
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			if (treeTaskStateMachine == null)
			{
				this.treeTaskStateMachine = awaiter.PoolGetUnit(out TreeTaskStateMachine<TStateMachine> taskStateMachine);
				taskStateMachine.SetStateMachine(ref stateMachine);
			}

			//如果传入的awaiter是一个同步任务，那么将自己记录起来。
			if (awaiter is ISyncTask syncTask) awaiter.syncTask = syncTask;

			if (task == null)//发生在异步方法等待的第一个 await TreeTask 时。
			{
				//新建一个TreeTask
				awaiter.Parent.AddTemp(out task);

				//尝试拿到上一个Task(也就是更深层方法)记录的同步任务。
				//以便在后续的同步执行时，可以直接获取这个同步任务。
				task.syncTask = awaiter.syncTask;


				//任务关联
				//传入的 awaiter 就是方法内的第一个 TreeTask
				//因为此时 awaiter 和 新建的 task 都没有令牌，所以需要关联起来。

				//假设：是A异步方法,调用B异步方法（B是第一个await）

				//那么B方法的状态机内部生成的TreeTask，也会被当做awaiter，传入到A的方法状态机内部。
				//于是所有的异步方法的第一个await任务，都会被关联起来。

				//另一种情况：传入的 awaiter 有令牌 ，这意味着在A方法里面，新建了个令牌2，给了B方法。
				//那么也只关联任务，不设置令牌2。因为这个令牌2是属于B方法的，不属于当前的A方法。
				task.m_RelevanceTask = awaiter;

				//task.Log($"新建任务[{task.Id}] 关联任务=> Awaiter[{awaiter.Id}]({awaiter.GetType().Name}) ，当前状态机：{stateMachine}");
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
			}
			else //发生在异步方法等待的第一个以后的 await TreeTask 时。
			{
				if (task.m_TreeTaskToken != null)
				{
					//如果当前任务有令牌，那么设置给传入的awaiter,同时会设置给所有 没有令牌 的关联任务。
					//task.Log($"当前任务[{task.Id}] 有令牌，设置令牌给 Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");
					awaiter.SetToken(task.m_TreeTaskToken);
				}
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
				//task.Log($"Awaiter[{awaiter.Id}]({awaiter.GetType().Name}) 尝试完成同步任务： if在当前task[{task.Id}] ，当前状态机：{stateMachine}");
				//尝试当前状态机找到同步任务执行
				awaiter.TrySyncTaskSetCompleted();
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
		private ITreeTaskStateMachine treeTaskStateMachine;

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
			task.LogError(exception);
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

		}

		// 6. AwaitUnsafeOnCompleted
		[SecuritySafeCritical]
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			if (treeTaskStateMachine == null)
			{
				this.treeTaskStateMachine = awaiter.PoolGetUnit(out TreeTaskStateMachine<TStateMachine> taskStateMachine);
				taskStateMachine.SetStateMachine(ref stateMachine);
			}

			if (awaiter is ISyncTask syncTask) awaiter.syncTask = syncTask;

			if (task == null)
			{
				awaiter.Parent.AddTemp(out task);
				task.syncTask = awaiter.syncTask;

				task.m_RelevanceTask = awaiter;

				//task.Log($"新建任务[{task.Id}] 关联任务=> Awaiter[{awaiter.Id}]({awaiter.GetType().Name}) ，当前状态机：{stateMachine}");
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
			}
			else
			{
				if (task.m_TreeTaskToken != null)
				{
					//task.Log($"当前任务[{task.Id}] 有令牌，设置令牌给 Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");
					awaiter.SetToken(task.m_TreeTaskToken);
				}
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
				//task.Log($"Awaiter[{awaiter.Id}]({awaiter.GetType().Name}) 尝试完成同步任务： if在当前task[{task.Id}] ，当前状态机：{stateMachine}");
				awaiter.TrySyncTaskSetCompleted();
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
