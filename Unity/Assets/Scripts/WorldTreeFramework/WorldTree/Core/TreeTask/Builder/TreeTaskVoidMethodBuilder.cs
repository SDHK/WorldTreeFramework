/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace WorldTree.Internal
{
	/// <summary>
	/// 空异步任务构建器
	/// </summary>
	public struct TreeTaskVoidMethodBuilder
	{
		/// <summary>
		/// 等待器
		/// </summary>
		private TreeTaskBase awaiter;

		/// <summary>
		/// 创建一个新的构建器
		/// </summary>
		[DebuggerHidden]
		public static TreeTaskVoidMethodBuilder Create()
		{
			TreeTaskVoidMethodBuilder builder = new TreeTaskVoidMethodBuilder();
			return builder;
		}

		/// <summary>
		/// 任务
		/// </summary>
		[DebuggerHidden]
		public TreeTaskVoid Task => default;

		/// <summary>
		/// 设置异常
		/// </summary>
		[DebuggerHidden]
		public void SetException(Exception exception)
		{
			awaiter.LogError(exception);
		}

		/// <summary>
		/// 设置结果
		/// </summary>
		public void SetResult()
		{
		}

		/// <summary>
		/// 等待完成
		/// </summary>
		[DebuggerHidden]
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			this.awaiter = awaiter;
			awaiter.OnCompleted(stateMachine.MoveNext);
		}

		/// <summary>
		/// 等待完成
		/// </summary>
		[DebuggerHidden]
		[SecuritySafeCritical]
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			this.awaiter = awaiter;
			awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
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
