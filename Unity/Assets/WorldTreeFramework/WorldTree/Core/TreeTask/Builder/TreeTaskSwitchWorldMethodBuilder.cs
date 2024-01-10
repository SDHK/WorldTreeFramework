/****************************************

* 作者： 闪电黑客
* 日期： 2024/01/03 11:33:27

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
	public struct TreeTaskSwitchWorldMethodBuilder
	{
		private ITreeTaskStateMachine treeTaskStateMachine;

		private TreeTaskSwitchWorld task;


		[DebuggerHidden]
		public static TreeTaskSwitchWorldMethodBuilder Create()
		{
			return new TreeTaskSwitchWorldMethodBuilder();
		}

		// 2. TaskLike Task property.
		[DebuggerHidden]
		public TreeTaskSwitchWorld Task
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
			task.SetCompleted();
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
			task = null;
			if (task != null)
			{
				if (task.m_TreeTaskToken != null)
				{
					awaiter.SetToken(task.m_TreeTaskToken);
				}
				awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
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
