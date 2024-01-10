﻿
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
			TreeTaskBase.ExceptionHandler?.Invoke(exception);
		}

		// 4. SetResult
		public void SetResult()
		{
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
			if (treeTaskStateMachine == null)
			{
				this.treeTaskStateMachine = awaiter.PoolGetUnit(out TreeTaskStateMachine<TStateMachine> taskStateMachine);
				taskStateMachine.SetStateMachine(ref stateMachine);
			}
			if (task == null)
			{

				awaiter.Parent.AddChild(out task);

				if (awaiter.m_TreeTaskToken is null)
				{
					task.m_RelevanceTask = awaiter;
				}
				else
				{
					task.m_TreeTaskToken = awaiter.m_TreeTaskToken;
					task.m_TreeTaskToken.tokenEvent.Add(task, default(ITreeTaskTokenEventRule));
				}
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
			}
			else
			{
				if (task.m_TreeTaskToken != null)
				{
					awaiter.SetToken(task.m_TreeTaskToken);
				}
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
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
			TreeTaskBase.ExceptionHandler?.Invoke(exception);
		}

		// 4. SetResult
		[DebuggerHidden]

		public void SetResult(T ret)
		{

			task.SetResult(ret);
			
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
			if (task == null)
			{
				awaiter.Parent.AddChild(out task);
				if (awaiter.m_TreeTaskToken is null)
				{
					task.m_RelevanceTask = awaiter;
				}
				else
				{
					task.m_TreeTaskToken = awaiter.m_TreeTaskToken;
					task.m_TreeTaskToken.tokenEvent.Add(task, TypeInfo<ITreeTaskTokenEventRule>.Default);
				}
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);

			}
			else
			{
				if (task.m_TreeTaskToken != null)
				{
					awaiter.SetToken(task.m_TreeTaskToken);
				}
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
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
