/****************************************

* 作者：闪电黑客
* 日期：2024/6/18 14:44

* 描述：

*/
using System;
using System.Threading.Tasks;

namespace WorldTree.Internal
{
	/// <summary>
	/// 树任务连接器
	/// </summary>
	public class TreeTaskLink : AwaiterBase, ISyncTask

		, ChildOf<INode>
		, AsRule<Awake>
	{
		/// <summary>
		/// 任务
		/// </summary>
		public Task Task;
		/// <summary>
		/// 任务取消令牌
		/// </summary>
		//private CancellationToken cancellationToken; // 添加CancellationToken字段

		/// <summary>
		/// 是否启动
		/// </summary>
		private bool isStart = false;

		public override void SetCompleted()
		{
			this.Log($"[{this.Id}]({this.GetType().Name}) : SetCompleted Task启动！！！！!!!");
			if (IsDisposed || IsCompleted) throw new InvalidOperationException("当前任务早已完成");
			if (isStart) throw new InvalidOperationException("当前任务已经启动");
			RunTask();
		}

		/// <summary>
		/// 运行任务。失败和取消在世界线上 SetException，不再只打日志。
		/// </summary>
		public async void RunTask()
		{
			isStart = true;
			Exception caught = null;
			try
			{
				if (!Task.IsCompleted) await Task;
			}
			catch (Exception e)
			{
				caught = e;
			}

			var bclTask = Task;
			this.World.Line.WorldContext.Post((selfObj) =>
			{
				TreeTaskLink self = (TreeTaskLink)selfObj.Object;
				if (self.IsDisposed || self.IsCompleted) return;
				Exception error = TreeTaskLinkTask.TakeException(bclTask) ?? caught;
				if (error != null) self.SetException(error);
				else self.Finish();
			}, new(this));
		}

		/// <summary>
		/// 成功结束
		/// </summary>
		private void Finish() => base.SetCompleted();

		public override void OnDispose()
		{
			this.isStart = false;
			Task = null;
			base.OnDispose();
		}
	}

	/// <summary>
	/// 树任务连接器
	/// </summary>
	public class TreeTaskLink<T> : AwaiterBase<T>, ISyncTask
		, ChildOf<INode>
		, AsRule<Awake>
	{
		/// <summary>
		/// 任务
		/// </summary>
		public Task<T> Task;
		/// <summary>
		/// 任务取消令牌
		/// </summary>
		private bool isStart = false;

		public override T GetResult()
		{
			if (Task != null && Task.Status == TaskStatus.RanToCompletion) Result = Task.Result;
			return base.GetResult();
		}

		public override void SetCompleted()
		{
			if (IsDisposed || IsCompleted) throw new InvalidOperationException("当前任务早已完成");
			if (isStart) throw new InvalidOperationException("当前任务已经启动");
			RunTask();
		}

		public override void OnDispose()
		{
			this.isStart = false;
			Task = null;
			base.OnDispose();
		}

		/// <summary>
		/// 运行任务。失败和取消在世界线上 SetException，不再只打日志。
		/// </summary>
		public async void RunTask()
		{
			isStart = true;
			Exception caught = null;
			try
			{
				if (!Task.IsCompleted) await Task;
			}
			catch (Exception e)
			{
				caught = e;
			}

			var bclTask = Task;
			this.World.Line.WorldContext.Post((selfObj) =>
			{
				TreeTaskLink<T> self = (TreeTaskLink<T>)selfObj.Object;
				if (self.IsDisposed || self.IsCompleted) return;
				Exception error = TreeTaskLinkTask.TakeException(bclTask) ?? caught;
				if (error != null) self.SetException(error);
				else self.Finish();
			}, new(this));
		}

		/// <summary>
		/// 成功结束
		/// </summary>
		private void Finish() => base.SetCompleted();
	}

	/// <summary>
	/// 从 BCL 任务上取出失败或取消
	/// </summary>
	internal static class TreeTaskLinkTask
	{
		/// <summary>
		/// 成功返回空。失败拆开一层 AggregateException，取消变成 TaskCanceledException。
		/// </summary>
		public static Exception TakeException(Task task)
		{
			if (task == null) return null;
			if (task.IsFaulted)
			{
				Exception exception = task.Exception;
				if (exception is AggregateException aggregate && aggregate.InnerExceptions.Count == 1)
					return aggregate.InnerExceptions[0];
				return exception;
			}
			if (task.IsCanceled) return new TaskCanceledException(task);
			return null;
		}
	}
}