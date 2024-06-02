
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/9 14:34

* 描述： 世界树异步任务基类
* 
* 内联令牌的异步Task基类

*/

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 同步任务标记接口
	/// </summary>
	/// <remarks>任务将会在构建器中以同步形式直接执行</remarks>
	public interface ISyncTask
	{
		/// <summary>
		/// 同步任务设置完成
		/// </summary>
		void SetCompleted();
	}

	/// <summary>
	/// 树异步任务基类
	/// </summary>
	public abstract class TreeTaskBase : Node, ICriticalNotifyCompletion
		, AsTreeTaskTokenEvent
	{
		/// <summary>
		/// 树任务令牌
		/// </summary>
		public TreeTaskToken m_TreeTaskToken;

		/// <summary>
		/// 关联令牌的任务
		/// </summary>
		public TreeTaskBase m_RelevanceTask;

		/// <summary>
		/// 同步任务记录
		/// </summary>
		/// <remarks>
		/// <para>记录异步调用流 await 的第一个 Task 是否为同步任务</para>
		/// <para>如果是同步任务，将会在构建器中直接执行</para>
		/// </remarks>
		public ISyncTask syncTask;

		/// <summary>
		/// 是否完成
		/// </summary>
		public abstract bool IsCompleted { get; set; }

		/// <summary>
		/// 延续
		/// </summary>
		public Action m_Continuation;

		/// <summary>
		/// 设置完成
		/// </summary>
		public void SetCompleted()
		{
			if (IsCompleted) return;
			//this.Log($"[{this.Id}]({this.GetType().Name}) 任务完成!!!");
			IsCompleted = true;
			if (m_TreeTaskToken is null)
			{
				m_Continuation?.Invoke();
				Dispose();
			}
			else
			{
				switch (m_TreeTaskToken.State)
				{
					case TaskState.Running:
						m_Continuation?.Invoke();
						Dispose();
						break;
					case TaskState.Stop:
						m_TreeTaskToken.stopTask = this;
						break;
					case TaskState.Cancel:
						Dispose();
						break;
				}
			}
		}

		/// <summary>
		/// 不安全完成时
		/// </summary>
		public virtual void UnsafeOnCompleted(Action continuation)
		{
			this.m_Continuation = continuation;
		}
		/// <summary>
		/// 完成时
		/// </summary>
		public virtual void OnCompleted(Action continuation)
		{
			UnsafeOnCompleted(continuation);
		}

		/// <summary>
		/// 继续
		/// </summary>
		public void Continue()
		{
			if (IsCompleted)
			{
				m_Continuation?.Invoke();
				Dispose();
			}
		}

		/// <summary>
		/// 设置并传递令牌
		/// </summary>
		public TreeTaskBase SetToken(TreeTaskToken treeTaskToken)
		{
			TreeTaskBase NowAwaiter = this;
			while (NowAwaiter != null)
			{
				if (NowAwaiter.m_TreeTaskToken == null)
				{
					//NowAwaiter.Log($"{NowAwaiter.Id}({NowAwaiter.GetType().Name})设置令牌：{treeTaskToken?.Id}!!!!!!!!");
					NowAwaiter.m_TreeTaskToken = treeTaskToken;
				}
				else
				{
					//NowAwaiter.Log($"{NowAwaiter.Id}({NowAwaiter.GetType().Name})已有令牌：{NowAwaiter.m_TreeTaskToken?.Id}XXXXX");
					return this;
				}

				treeTaskToken.tokenEvent.Add(NowAwaiter, default(TreeTaskTokenEvent));
				NowAwaiter = NowAwaiter.m_RelevanceTask;
			}
			return this;
		}

		/// <summary>
		/// 尝试执行记录的同步任务
		/// </summary>
		public void TrySyncTaskSetCompleted()
		{
			//this.Log($"同步任务执行[{(this.syncTask as TreeTaskBase)?.Id}]({this.syncTask?.GetType().Name})完成");
			this.syncTask?.SetCompleted();
		}

		public override void OnDispose()
		{
			IsCompleted = false;
			m_TreeTaskToken = null;
			m_RelevanceTask = null;
			m_Continuation = null;
			syncTask = null;
			base.OnDispose();
		}
	}

	class TreeTaskBaseTaskTokenEventRule : TreeTaskTokenEventRule<TreeTaskBase>
	{
		protected override void Execute(TreeTaskBase self, TaskState state)
		{
			if (state == TaskState.Cancel)
			{
				self.Dispose();
			
			}
		}
	}
}
