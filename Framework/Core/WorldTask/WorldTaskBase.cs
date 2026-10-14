/****************************************

* 作者：闪电黑客
* 日期：2026/10/14

* 描述：无线程归属的异步任务。继承 Unit，不挂树，完成与取消走委托。

*/
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using WorldTree.Internal;

namespace WorldTree
{
	/// <summary>
	/// 同步任务标记。构建器会在当前调用里直接把它完成。
	/// </summary>
	public interface IWorldSyncTask { }

	/// <summary>
	/// 任务状态
	/// </summary>
	public enum WorldTaskState
	{
		/// <summary>
		/// 等待
		/// </summary>
		Pending,

		/// <summary>
		/// 成功
		/// </summary>
		Succeeded,

		/// <summary>
		/// 失败
		/// </summary>
		Faulted,

		/// <summary>
		/// 取消。await 不抛异常，调用方自己看状态或令牌。
		/// </summary>
		Canceled,
	}

	/// <summary>
	/// 异步任务基类
	/// </summary>
	public abstract class WorldTaskBase : Unit, ICriticalNotifyCompletion
	{
		/// <summary>
		/// 任务状态
		/// </summary>
		public WorldTaskState State;

		/// <summary>
		/// 令牌。空表示不受令牌控制。
		/// </summary>
		public WorldTaskToken Token;

		/// <summary>
		/// 更深层的等待者。令牌沿这条链传播，直接持有实例。
		/// </summary>
		public WorldTaskBase Relevance;

		/// <summary>
		/// 完成后执行的延续
		/// </summary>
		private Action continuation;

		/// <summary>
		/// 失败时抛出的异常
		/// </summary>
		private ExceptionDispatchInfo exceptionInfo;

		/// <summary>
		/// 是否完成
		/// </summary>
		public bool IsCompleted => State != WorldTaskState.Pending;

		/// <summary>
		/// 获取等待器
		/// </summary>
		public WorldTaskBase GetAwaiter() => this;

		/// <summary>
		/// 获取结果
		/// </summary>
		public void GetResult() => OnGetResult();

		/// <summary>
		/// 注册延续
		/// </summary>
		public virtual void OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);

		/// <summary>
		/// 注册延续
		/// </summary>
		public virtual void UnsafeOnCompleted(Action continuation) => this.continuation = continuation;

		/// <summary>
		/// 设置完成
		/// </summary>
		public void SetCompleted()
		{
			if (IsDisposed || IsCompleted) throw new InvalidOperationException($"WorldTask 已经完成");

			if (Token != null)
			{
				switch (Token.State)
				{
					case WorldTokenState.Stop:
						Token.PausedTask = this;
						return;
					case WorldTokenState.Cancel:
						State = WorldTaskState.Canceled;
						break;
					default:
						State = WorldTaskState.Succeeded;
						break;
				}
			}
			else
			{
				State = WorldTaskState.Succeeded;
			}

			Action next = continuation;
			continuation = null;
			next?.Invoke();
		}

		/// <summary>
		/// 设置异常
		/// </summary>
		public void SetException(Exception exception)
		{
			if (IsDisposed || IsCompleted) throw new InvalidOperationException($"WorldTask 已经完成，但出了异常 {exception}");

			State = WorldTaskState.Faulted;
			exceptionInfo = ExceptionDispatchInfo.Capture(exception);
			Action next = continuation;
			continuation = null;
			next?.Invoke();
		}

		/// <summary>
		/// 成功时取结果。同步任务在这里还池，普通任务不还池。
		/// </summary>
		protected virtual void OnGetResult()
		{
			switch (State)
			{
				case WorldTaskState.Canceled:
				case WorldTaskState.Succeeded:
					if (this is IWorldSyncTask) Dispose();
					break;
				case WorldTaskState.Faulted:
					ExceptionDispatchInfo captured = exceptionInfo;
					exceptionInfo = null;
					if (this is IWorldSyncTask) Dispose();
					captured?.Throw();
					break;
				default:
					throw new NotSupportedException("任务未完成时不能调用 GetResult");
			}
		}

		/// <summary>
		/// 沿关联链设置令牌。已经有令牌的任务不再往下传。
		/// </summary>
		public WorldTaskBase SetToken(WorldTaskToken token)
		{
			if (token == null) return this;
			WorldTaskBase now = this;
			while (now != null)
			{
				if (now.Token != null) return this;
				now.Token = token;
				now = now.Relevance;
			}
			return this;
		}

		/// <summary>
		/// 最深层如果是同步任务，就地完成。
		/// </summary>
		public void FindSyncTaskSetCompleted()
		{
			WorldTaskBase now = this;
			while (now.Relevance != null) now = now.Relevance;
			if (now is IWorldSyncTask && !now.IsCompleted) now.SetCompleted();
		}

		/// <summary>
		/// 顶层入口。异步方法返回后调用，同步任务才会接着跑完。
		/// </summary>
		public void Coroutine()
		{
			InnerCoroutine().Coroutine();
			FindSyncTaskSetCompleted();
		}

		/// <summary>
		/// 顶层入口，并指定令牌
		/// </summary>
		public void Coroutine(WorldTaskToken token)
		{
			SetToken(token);
			InnerCoroutine().Coroutine();
			FindSyncTaskSetCompleted();
		}

		/// <summary>
		/// 把顶层任务挂进一次空的异步方法，Coroutine 才能启动延续
		/// </summary>
		private async WorldTaskVoid InnerCoroutine()
		{
			await this;
		}

		public override void OnCreate()
		{
			State = WorldTaskState.Pending;
			Token = null;
			Relevance = null;
			continuation = null;
			exceptionInfo = null;
		}

		public override void OnDispose()
		{
			State = WorldTaskState.Pending;
			Token = null;
			Relevance = null;
			continuation = null;
			exceptionInfo = null;
			base.OnDispose();
		}
	}
}
