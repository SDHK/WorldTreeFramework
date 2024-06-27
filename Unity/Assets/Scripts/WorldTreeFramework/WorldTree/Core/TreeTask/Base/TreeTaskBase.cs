
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/9 14:34

* 描述： 世界树异步任务基类
* 
* 内联令牌的异步Task基类

*/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace WorldTree
{
	public interface ITreeTaskFlow
	{
		/// <summary>
		/// 开始时
		/// </summary>
		void OnStart();

	}


	/// <summary>
	/// 同步任务标记接口
	/// </summary>
	/// <remarks>任务将会在构建器中以同步形式直接执行</remarks>
	public interface ISyncTask { }

	/// <summary>
	/// 等待器状态
	/// </summary>
	public enum AwaiterState
	{
		/// <summary>
		/// 等待
		/// </summary>
		Pending,

		/// <summary>
		/// 成功完成
		/// </summary>
		Succeeded,

		/// <summary>
		/// 失败完成
		/// </summary>
		Faulted,

		/// <summary>
		/// 取消完成
		/// </summary>
		Canceled,

	}

	/// <summary>
	/// 异步任务基类
	/// </summary>
	public abstract class AwaiterBase<T> : TreeTaskBase
	{
		/// <summary>
		/// 获取等待器
		/// </summary>
		public AwaiterBase<T> GetAwaiter() => this;
		/// <summary>
		/// 结果
		/// </summary>
		public T Result;
		/// <summary>
		/// 获取结果
		/// </summary>
		public virtual T GetResult()
		{
			OnGetResult();
			return Result;
		}
		/// <summary>
		/// 设置结果
		/// </summary>
		public void SetResult(T result)
		{
			this.Result = result;
			this.SetCompleted();
		}
	}

	/// <summary>
	/// 异步任务基类
	/// </summary>
	public abstract class AwaiterBase : TreeTaskBase
	{
		/// <summary>
		/// 获取等待器
		/// </summary>
		public AwaiterBase GetAwaiter() => this;

		/// <summary>
		/// 获取结果
		/// </summary>
		public void GetResult()
		{
			OnGetResult();
		}

		/// <summary>
		/// 设置结果
		/// </summary>
		public void SetResult()
		{
			this.SetCompleted();
		}
	}

	/// <summary>
	/// 树任务基类
	/// </summary>
	public abstract class TreeTaskBase : Node, ICriticalNotifyCompletion
	{
		/// <summary>
		/// 树任务令牌
		/// </summary>
		/// <remarks>如果令牌被释放，异步链将变成无令牌控制状态</remarks>
		public NodeRef<TreeTaskToken> TreeTaskToken;

		/// <summary>
		/// 关联令牌的任务
		/// </summary>
		/// <remarks>如果关联任务被意外释放，令牌传播可能断掉</remarks>
		public NodeRef<TreeTaskBase> RelevanceTask;

		/// <summary>
		/// 任务状态
		/// </summary>
		public AwaiterState state;

		/// <summary>
		/// 是否完成
		/// </summary>
		public virtual bool IsCompleted => state != AwaiterState.Pending;

		/// <summary>
		/// 延续执行内容：委托 或 异常
		/// </summary>
		public object m_Continuation;

		public virtual void OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);
		public virtual void UnsafeOnCompleted(Action continuation) => this.m_Continuation = continuation;


		/// <summary>
		/// 设置完成
		/// </summary>
		public virtual void SetCompleted()
		{
			if (IsRecycle || IsCompleted) throw new InvalidOperationException($"[{Id}]({this.GetType().Name})当前任务早已完成");

			//判断任务暂停状态
			if (TreeTaskToken.Value is TreeTaskToken token)
			{
				switch (token.State)
				{
					case TokenState.Stop:
						token.stopTask = this;
						return;
					case TokenState.Cancel:
						state = AwaiterState.Canceled;
						break;
					case TokenState.Running:
						state = AwaiterState.Succeeded;
						break;
				}
			}
			else
			{
				state = AwaiterState.Succeeded;
			}

			//设置为成功完成状态
			Action c = this.m_Continuation as Action;
			this.m_Continuation = null;
			c?.Invoke();
		}

		/// <summary>
		/// 设置异常
		/// </summary>
		public virtual void SetException(Exception e)
		{
			if (IsRecycle || IsCompleted) throw new InvalidOperationException($"[{Id}]({this.GetType().Name})当前任务早已完成，但出了异常{e}");

			//设置为失败完成状态
			this.state = AwaiterState.Faulted;
			Action c = this.m_Continuation as Action;
			this.m_Continuation = ExceptionDispatchInfo.Capture(e);
			c?.Invoke();
		}

		/// <summary>
		/// 完成之后
		/// </summary>
		protected virtual void OnGetResult()
		{
			switch (this.state)
			{
				case AwaiterState.Canceled:
				//取消的任务也是完成的任务，取消逻辑交给业务判断

				case AwaiterState.Succeeded:
					Dispose();
					break;
				case AwaiterState.Faulted:
					ExceptionDispatchInfo c = this.m_Continuation as ExceptionDispatchInfo;
					this.m_Continuation = null;
					Dispose();
					c?.Throw();
					break;
				default:
					throw new NotSupportedException("当任务未完成时，不允许直接调用GetResult。请使用await");
			}
		}

		public override void OnDispose()
		{
			state = AwaiterState.Pending;
			RelevanceTask = default;
			TreeTaskToken = default;
			m_Continuation = null;
			base.OnDispose();
		}


		/// <summary>
		/// 设置并传递令牌
		/// </summary>
		/// <remarks>由于令牌通过关联任务进行内部传播，所以设置后不可更改，也不能传空</remarks>
		public TreeTaskBase SetToken(TreeTaskToken treeTaskToken)
		{
			if (treeTaskToken == null)
			{
				this.LogError($"{this.Id}任务设置令牌为null");
				return this;
			}
			TreeTaskBase nowTask = this;
			while (nowTask != null)
			{
				if (nowTask.TreeTaskToken.Value == null)
				{
					//nowTask.Log($"{nowTask.Id}({nowTask.GetType().Name})设置令牌：{treeTaskToken?.Id}!!!!!!!!");
					//将令牌传递给更深层的关联任务
					nowTask.TreeTaskToken = treeTaskToken;
				}
				else
				{
					//nowTask.Log($"{nowTask.Id}({nowTask.GetType().Name})已有令牌：{nowTask.m_TreeTaskToken.Value?.Id}XXXXX");
					//已有令牌的情况，发生在异步方法内部新建了令牌，所以当前传递的令牌不再往下传播，直接退出。
					return this;
				}
				nowTask = nowTask.RelevanceTask;
			}
			return this;
		}

		/// <summary>
		/// 尝试找到同步任务执行
		/// </summary>
		public void FindSyncTaskSetCompleted()
		{
			TreeTaskBase nowTask = this;
			//找到最深层的关联任务
			while (nowTask.RelevanceTask.Value != null) nowTask = nowTask.RelevanceTask;
			//如果是同步任务就直接执行
			if (nowTask is ISyncTask)
			{
				//this.Log($"同步任务执行[{nowTask.Id}]({nowTask.GetType().Name})完成");
				nowTask.SetCompleted();
			}
		}
	}

}
