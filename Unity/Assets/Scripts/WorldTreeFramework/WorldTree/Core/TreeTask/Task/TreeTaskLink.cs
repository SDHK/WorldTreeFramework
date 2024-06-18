/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/13 19:30

* 描述： 树任务令牌捕获任务
*
* 这是一个和TreeTaskCompleted一样的同步任务，
*
* 它可以从异步流中捕获到令牌
*

*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace WorldTree.Internal
{
	/// <summary>
	/// 树任务连接器
	/// </summary>
	public class TreeTaskLink : AwaiterBase, ISyncTask

		, ChildOf<INode>
		, AsAwake
	{
		public Task task;
		private CancellationToken cancellationToken; // 添加CancellationToken字段

		private bool isStart = false;

		public override void SetCompleted()
		{
			this.Log($"[{this.Id}]({this.GetType().Name}) : SetCompleted Task启动！！！！!!!");
			if (IsRecycle || IsCompleted) throw new InvalidOperationException("当前任务早已完成");
			if (isStart) throw new InvalidOperationException("当前任务已经启动");
			RunTask();
		}

		public async void RunTask()
		{
			isStart = true;
			try
			{
				if (!task.IsCompleted && !task.IsFaulted)
				{
					await task;
				}

				this.Core.worldContext.Post((self) =>
				{
					var Self = (TreeTaskLink)self;
					base.SetCompleted();
				}, this);
			}
			catch (Exception e)
			{
				this.Core.worldContext.Post(this.LogError, e);
			}
		}

		public override void OnDispose()
		{
			this.isStart = false;
			task = null;
			base.OnDispose();
		}
	}

	/// <summary>
	/// 树任务连接器
	/// </summary>
	public class TreeTaskLink<T> : AwaiterBase<T>, ISyncTask
		, ChildOf<INode>
		, AsAwake
	{
		public Task<T> task;

		private bool isStart = false;

		public override T GetResult()
		{ Result = task.Result; return base.GetResult(); }

		public override void SetCompleted()
		{
			if (IsRecycle || IsCompleted) throw new InvalidOperationException("当前任务早已完成");
			if (isStart) throw new InvalidOperationException("当前任务已经启动");
			RunTask();
		}

		public override void OnDispose()
		{
			this.isStart = false;
			task = null;
			base.OnDispose();
		}

		public async void RunTask()
		{
			isStart = true;
			try
			{
				if (!task.IsCompleted && !task.IsFaulted)
				{
					await task;
				}
				this.Core.worldContext.Post((self) =>
				{
					var Self = (TreeTaskLink<T>)self;
					base.SetCompleted();
				}, this);
			}
			catch (Exception e)
			{
				this.Core.worldContext.Post(this.LogError, e);
			}
		}
	}
}