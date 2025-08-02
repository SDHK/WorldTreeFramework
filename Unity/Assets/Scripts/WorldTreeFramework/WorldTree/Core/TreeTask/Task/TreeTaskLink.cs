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
		/// 运行任务
		/// </summary>
		public async void RunTask()
		{
			isStart = true;
			try
			{
				if (!Task.IsCompleted && !Task.IsFaulted)
				{
					await Task;
				}

				this.Core.WorldContext.Post((selfObj) =>
				{
					var self = (TreeTaskLink)selfObj;
					base.SetCompleted();
				}, this);
			}
			catch (Exception e)
			{
				this.Core.WorldContext.Post((e) => this.LogError((Exception)e), e);
			}
		}

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
		{ Result = Task.Result; return base.GetResult(); }

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
		/// 运行任务
		/// </summary>
		public async void RunTask()
		{
			isStart = true;
			try
			{
				if (!Task.IsCompleted && !Task.IsFaulted)
				{
					await Task;
				}
				this.Core.WorldContext.Post((selfObj) =>
				{
					var self = (TreeTaskLink<T>)selfObj;
					base.SetCompleted();
				}, this);
			}
			catch (Exception e)
			{
				this.Core.WorldContext.Post((o) => this.LogError((Exception)o), e);
			}
		}
	}
}