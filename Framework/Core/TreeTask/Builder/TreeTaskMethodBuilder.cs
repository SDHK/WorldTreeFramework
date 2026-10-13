/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

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
		/// <summary>
		/// 状态机
		/// </summary>
		private TreeTaskStateMachine treeTaskStateMachine;
		/// <summary>
		/// 任务
		/// </summary>
		private TreeTask task;

		/// <summary>
		/// 创建一个新的构建器
		/// </summary>
		[DebuggerHidden]
		public static TreeTaskMethodBuilder Create()
		{
			return new TreeTaskMethodBuilder();
		}

		/// <summary>
		/// 任务
		/// </summary>
		[DebuggerHidden]
		public TreeTask Task
		{
			get
			{
				return task;
			}
		}

		/// <summary>
		/// 设置异常
		/// </summary>
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

		/// <summary>
		/// 设置结果
		/// </summary>
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

		/// <summary>
		/// 等待完成
		/// </summary>
		[DebuggerHidden]

		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
		}

		/// <summary>
		/// 等待完成
		/// </summary>
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			if (task == null)//发生在异步方法等待的第一个 await TreeTask 时。
			{
				//新建一个TreeTask
				awaiter.Parent.AddTemp(out task);
				if (treeTaskStateMachine == null) awaiter.Parent.AddTemp(out treeTaskStateMachine).SetStateMachine(ref stateMachine);
				//任务关联
				//task.Log($"新建任务[{task.Id}] 关联=> Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");
				task.RelevanceTask = awaiter;
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
			}
			else

			{
				if (task.TreeTaskToken.Value != null)
				{
					//task.Log($"当前任务[{task.Id}] 有令牌，设置令牌[{task.m_TreeTaskToken.Value.Id}]给 Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");
					//如果当前任务有令牌，那么设置给传入的awaiter，同时会设置给所有 没有令牌 的关联任务。
					awaiter.SetToken(task.TreeTaskToken);
				}
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
				awaiter.FindSyncTaskSetCompleted();
			}

		}

		/// <summary>
		/// 开始
		/// </summary>
		[DebuggerHidden]
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			stateMachine.MoveNext();
		}

		/// <summary>
		/// 设置状态机
		/// </summary>
		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}
	}

	/// <summary>
	/// 泛型异步任务构建器
	/// </summary>
	public struct TreeTaskMethodBuilder<T>
	{
		/// <summary>
		/// 状态机
		/// </summary>
		private TreeTaskStateMachine treeTaskStateMachine;
		/// <summary>
		/// 任务
		/// </summary>
		private TreeTask<T> task;

		/// <summary>
		/// 创建一个新的构建器
		/// </summary>
		[DebuggerHidden]
		public static TreeTaskMethodBuilder<T> Create()
		{
			TreeTaskMethodBuilder<T> builder = new TreeTaskMethodBuilder<T>();
			return builder;
		}

		/// <summary>
		/// 任务
		/// </summary>
		[DebuggerHidden]
		public TreeTask<T> Task
		{
			get
			{
				return task;
			}
		}

		/// <summary>
		/// 设置异常
		/// </summary>
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

		/// <summary>
		/// 设置结果
		/// </summary>
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

		/// <summary>
		/// 等待完成
		/// </summary>
		[DebuggerHidden]
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
		}

		/// <summary>
		/// 等待完成
		/// </summary>
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
				task.RelevanceTask = awaiter;
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);

			}
			else
			{
				if (task.TreeTaskToken.Value != null)
				{
					//task.Log($"当前任务[{task.Id}] 有令牌，设置令牌[{task.m_TreeTaskToken.Value.Id}]给 Awaiter[{awaiter.Id}]({awaiter.GetType().Name})，当前状态机：{stateMachine}");
					//如果当前任务有令牌，那么设置给传入的awaiter，同时会设置给所有 没有令牌 的关联任务。
					awaiter.SetToken(task.TreeTaskToken);
				}
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
				awaiter.FindSyncTaskSetCompleted();
			}
		}

		/// <summary>
		/// 开始
		/// </summary>
		[DebuggerHidden]
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			stateMachine.MoveNext();
		}

		/// <summary>
		/// 设置状态机
		/// </summary>
		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}
	}


}
