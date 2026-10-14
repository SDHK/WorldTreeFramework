/****************************************

* 作者：闪电黑客
* 日期：2026/10/14

* 描述：

*/
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace WorldTree.Internal
{
	/// <summary>
	/// 异步任务构建器
	/// </summary>
	public struct WorldTaskMethodBuilder
	{
		/// <summary>
		/// 任务
		/// </summary>
		private WorldTask task;

		/// <summary>
		/// 状态机
		/// </summary>
		private WorldTaskStateMachine stateMachine;

		/// <summary>
		/// 创建构建器
		/// </summary>
		public static WorldTaskMethodBuilder Create() => default;

		/// <summary>
		/// 任务。第一次 await 之前是空。
		/// </summary>
		public WorldTask Task => task;

		/// <summary>
		/// 启动状态机
		/// </summary>
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
			=> stateMachine.MoveNext();

		/// <summary>
		/// 设置状态机
		/// </summary>
		public void SetStateMachine(IAsyncStateMachine stateMachine) { }

		/// <summary>
		/// 设置异常
		/// </summary>
		public void SetException(Exception exception)
		{
			if (task == null) ExceptionDispatchInfo.Capture(exception).Throw();
			task.SetException(exception);
			RecycleStateMachine();
		}

		/// <summary>
		/// 设置结果
		/// </summary>
		public void SetResult()
		{
			if (task == null) throw new InvalidOperationException("async WorldTask 至少要有一次 await");
			task.SetResult();
			RecycleStateMachine();
		}

		/// <summary>
		/// 等待完成
		/// </summary>
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
			where TAwaiter : WorldTaskBase, INotifyCompletion
			where TStateMachine : IAsyncStateMachine
			=> AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);

		/// <summary>
		/// 等待完成
		/// </summary>
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
			where TAwaiter : WorldTaskBase, ICriticalNotifyCompletion
			where TStateMachine : IAsyncStateMachine
		{
			if (task == null)
			{
				awaiter.World.PoolGetUnit(out task);
				awaiter.World.PoolGetUnit(out this.stateMachine);
				task.Relevance = awaiter;
				this.stateMachine.SetStateMachine(ref stateMachine);
				awaiter.UnsafeOnCompleted(this.stateMachine.MoveNext);
			}
			else
			{
				if (task.Token != null) awaiter.SetToken(task.Token);
				awaiter.UnsafeOnCompleted(this.stateMachine.MoveNext);
				awaiter.FindSyncTaskSetCompleted();
			}
		}

		/// <summary>
		/// 方法结束后归还状态机
		/// </summary>
		private void RecycleStateMachine()
		{
			if (stateMachine == null) return;
			stateMachine.Dispose();
			stateMachine = null;
		}
	}

	/// <summary>
	/// 带返回值的异步任务构建器
	/// </summary>
	public struct WorldTaskMethodBuilder<T>
	{
		/// <summary>
		/// 任务
		/// </summary>
		private WorldTask<T> task;

		/// <summary>
		/// 状态机
		/// </summary>
		private WorldTaskStateMachine stateMachine;

		/// <summary>
		/// 创建构建器
		/// </summary>
		public static WorldTaskMethodBuilder<T> Create() => default;

		/// <summary>
		/// 任务。第一次 await 之前是空。
		/// </summary>
		public WorldTask<T> Task => task;

		/// <summary>
		/// 启动状态机
		/// </summary>
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
			=> stateMachine.MoveNext();

		/// <summary>
		/// 设置状态机
		/// </summary>
		public void SetStateMachine(IAsyncStateMachine stateMachine) { }

		/// <summary>
		/// 设置异常
		/// </summary>
		public void SetException(Exception exception)
		{
			if (task == null) ExceptionDispatchInfo.Capture(exception).Throw();
			task.SetException(exception);
			RecycleStateMachine();
		}

		/// <summary>
		/// 设置结果
		/// </summary>
		public void SetResult(T result)
		{
			if (task == null) throw new InvalidOperationException("async WorldTask 至少要有一次 await");
			task.SetResult(result);
			RecycleStateMachine();
		}

		/// <summary>
		/// 等待完成
		/// </summary>
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
			where TAwaiter : WorldTaskBase, INotifyCompletion
			where TStateMachine : IAsyncStateMachine
			=> AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);

		/// <summary>
		/// 等待完成
		/// </summary>
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
			where TAwaiter : WorldTaskBase, ICriticalNotifyCompletion
			where TStateMachine : IAsyncStateMachine
		{
			if (task == null)
			{
				awaiter.World.PoolGetUnit(out task);
				awaiter.World.PoolGetUnit(out this.stateMachine);
				task.Relevance = awaiter;
				this.stateMachine.SetStateMachine(ref stateMachine);
				awaiter.UnsafeOnCompleted(this.stateMachine.MoveNext);
			}
			else
			{
				if (task.Token != null) awaiter.SetToken(task.Token);
				awaiter.UnsafeOnCompleted(this.stateMachine.MoveNext);
				awaiter.FindSyncTaskSetCompleted();
			}
		}

		/// <summary>
		/// 方法结束后归还状态机
		/// </summary>
		private void RecycleStateMachine()
		{
			if (stateMachine == null) return;
			stateMachine.Dispose();
			stateMachine = null;
		}
	}
}
