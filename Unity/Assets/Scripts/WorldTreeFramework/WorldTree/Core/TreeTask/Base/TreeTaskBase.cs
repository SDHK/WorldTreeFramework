﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/9 14:34

* 描述： 世界树异步任务基类
* 
* 内联令牌的异步Task基类

*/

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WorldTree
{
	/// <summary>
	/// 同步任务标记接口
	/// </summary>
	/// <remarks>任务将会在构建器中以同步形式直接执行</remarks>
	public interface ISyncTask
	{


	}


	public abstract class TreeTaskBase : Node, ICriticalNotifyCompletion
	{
		/// <summary>
		/// 内联传递的令牌
		/// </summary>
		public object m_Context;

		/// <summary>
		/// 是否完成
		/// </summary>
		public abstract bool IsCompleted { get; set; }

		/// <summary>
		/// 延续
		/// </summary>
		public Action m_Continuation;

		public virtual void OnCompleted(Action continuation)
		{
			UnsafeOnCompleted(continuation);
		}

		public virtual void UnsafeOnCompleted(Action continuation)
		{
			this.m_Continuation = continuation;
		}

		/// <summary>
		/// 设置完成
		/// </summary>
		public void SetCompleted()
		{
			if (IsRecycle || IsCompleted) return;

			this.Log($"[{this.Id}]({this.GetType().Name}) :  任务完成!!!");
			IsCompleted = true;
			m_Continuation?.Invoke();
			Dispose();
		}

		public override void OnDispose()
		{
			IsCompleted = false;
			m_Context = default;
			m_Continuation = null;
			base.OnDispose();
		}

		/// <summary>
		/// 设置令牌
		/// </summary>
		/// <param name="treeTaskToken"></param>
		/// <returns></returns>
		public TreeTaskBase SetToken(TreeTaskToken treeTaskToken)
		{
			TreeTaskBase nowTask = this;
			while (nowTask != null)
			{
				//如果是同步任务就直接执行
				if (nowTask is ISyncTask)
				{
					this.Log($"[{nowTask.Id}]({nowTask.GetType().Name}) :  同步任务执行!!!");

					nowTask.m_Context = treeTaskToken;
					nowTask.SetCompleted();
					break;
				}

				//如果没有设置过令牌就设置
				if (nowTask.m_Context is not TreeTaskToken)
				{
					//假如当前任务有关联任务，那么就将关联任务设置为下次的当前任务
					TreeTaskBase nextTask = nowTask.m_Context as TreeTaskBase;
					//当前任务设置令牌
					nowTask.m_Context = treeTaskToken;
					this.Log($"[{nowTask.Id}]({nowTask.GetType().Name}) :  令牌传递!!!");

					//如果有关联任务就继续往下传递
					nowTask = nextTask;
				}
				else//如果设置过就不再往下传播
				{
					this.Log($"[{nowTask.Id}]({nowTask.GetType().Name}) :  已有令牌存在XXXX");
					break;
				}
			}
			return this;
		}
	}


	/// <summary>
	/// 树异步任务基类
	/// </summary>
	public abstract class TreeTaskBase1 : Node, ICriticalNotifyCompletion
		, AsTreeTaskTokenEvent
	{
		/// <summary>
		/// 树任务令牌
		/// </summary>
		/// <remarks>如果令牌被释放，异步链将变成无令牌控制状态</remarks>
		public NodeRef<TreeTaskToken> m_TreeTaskToken;

		/// <summary>
		/// 关联令牌的任务
		/// </summary>
		/// <remarks>如果关联任务被意外释放，令牌传播可能断掉</remarks>
		public NodeRef<TreeTaskBase1> m_RelevanceTask;

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
			if (IsRecycle || IsCompleted) return;
			//this.Log($"[{this.Id}]({this.GetType().Name} X: {m_TreeTaskToken.Value?.State} 任务完成!!!");
			IsCompleted = true;

			if (m_TreeTaskToken.Value is null)//没有令牌就直接完成
			{
				m_Continuation?.Invoke();
				Dispose();
			}
			else
			{
				switch (m_TreeTaskToken.Value.State)
				{
					case TaskState.Running:
						m_Continuation?.Invoke();
						Dispose();
						break;
					case TaskState.Stop:
						//m_TreeTaskToken.Value.stopTask = this;
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
		/// <remarks>由于令牌通过关联任务进行内部传播，所以设置后不可更改，也不能传空</remarks>
		public TreeTaskBase1 SetToken(TreeTaskToken treeTaskToken)
		{
			if (treeTaskToken == null)
			{
				this.LogError($"{this.Id}任务设置令牌为null");
				return this;
			}
			TreeTaskBase1 nowTask = this;
			while (nowTask != null)
			{
				if (nowTask.m_TreeTaskToken.Value == null)
				{
					//nowTask.Log($"{nowTask.Id}({nowTask.GetType().Name})设置令牌：{treeTaskToken?.Id}!!!!!!!!");
					//将令牌传递给更深层的关联任务
					nowTask.m_TreeTaskToken = treeTaskToken;
				}
				else
				{
					//nowTask.Log($"{nowTask.Id}({nowTask.GetType().Name})已有令牌：{nowTask.m_TreeTaskToken.Value?.Id}XXXXX");
					//已有令牌的情况，发生在异步方法内部新建了令牌，所以当前传递的令牌不再往下传播，直接退出。
					return this;
				}
				treeTaskToken.tokenEvent.Add(nowTask, default(TreeTaskTokenEvent));
				nowTask = nowTask.m_RelevanceTask;
			}
			return this;
		}

		/// <summary>
		/// 尝试找到同步任务执行
		/// </summary>
		public void FindSyncTaskSetCompleted()
		{
			TreeTaskBase1 nowTask = this;
			//找到最深层的关联任务
			while (nowTask.m_RelevanceTask.Value != null) nowTask = nowTask.m_RelevanceTask;
			//如果是同步任务就直接执行
			if (nowTask is ISyncTask)
			{
				//this.Log($"同步任务执行[{nowTask.Id}]({nowTask.GetType().Name})完成");
				nowTask.SetCompleted();
			}
		}


		public override void OnDispose()
		{
			IsCompleted = false;
			m_TreeTaskToken = default;
			m_RelevanceTask = default;
			m_Continuation = null;
			base.OnDispose();
		}
	}
	
}
