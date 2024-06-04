/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/5 16:01

* 描述： 树任务令牌

*/


using System.Threading.Tasks;

namespace WorldTree
{
	/// <summary>
	/// 任务状态
	/// </summary>
	public enum TaskState
	{
		/// <summary>
		/// 任务运行中
		/// </summary>
		Running,
		/// <summary>
		/// 任务暂停
		/// </summary>
		Stop,
		/// <summary>
		/// 任务取消
		/// </summary>
		Cancel
	}

	/// <summary>
	/// 树任务令牌
	/// </summary>
	public class TreeTaskToken : Node, ChildOf<INode>, ComponentOf<INode>
		, AsChildBranch
		, AsAwake
	{
		/// <summary>
		/// 任务状态
		/// </summary>
		public TaskState taskState = TaskState.Running;

		/// <summary>
		/// 暂停的任务
		/// </summary>
		public TreeTaskBase stopTask;

		/// <summary>
		/// 任务令牌事件
		/// </summary>
		public RuleActuator<TreeTaskTokenEvent> tokenEvent;


		/// <summary>
		/// 任务状态
		/// </summary>
		public TaskState State { get => taskState; }

		public override string ToString()
		{
			return $"TreeTaskToken({stopTask?.Id})";
		}

		/// <summary>
		/// 继续执行
		/// </summary>
		public void Continue()
		{
			taskState = TaskState.Running;
			tokenEvent?.Send(taskState);
			stopTask?.Continue();
			stopTask = null;
		}

		/// <summary>
		/// 暂停
		/// </summary>
		public void Stop()
		{
			taskState = TaskState.Stop;
			tokenEvent?.Send(taskState);
		}

		/// <summary>
		/// 取消
		/// </summary>
		public void Cancel()
		{
			taskState = TaskState.Cancel;
			tokenEvent?.Send(taskState);
			tokenEvent.Clear();
			stopTask = null;
		}

	}

	public static class TreeTaskTokenRule
	{
		class AddRule : AddRule<TreeTaskToken>
		{
			protected override void Execute(TreeTaskToken self)
			{
				self.taskState = TaskState.Running;
				self.AddChild(out self.tokenEvent);
			}
		}

		class Remove : BeforeRemoveRule<TreeTaskToken>
		{
			protected override void Execute(TreeTaskToken self)
			{
				//令牌释放回收前，让挂起任务继续执行
				if (self.taskState == TaskState.Stop) self.Continue();
			}
		}

		class RemoveRule : RemoveRule<TreeTaskToken>
		{
			protected override void Execute(TreeTaskToken self)
			{
				self.stopTask = null;
				self.tokenEvent = null;
			}
		}
	}
}
