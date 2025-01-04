/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 任务状态
	/// </summary>
	public enum TokenState
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
		, AsTreeTaskTokenEvent
	{
		/// <summary>
		/// 任务状态
		/// </summary>
		public TokenState taskState = TokenState.Running;

		/// <summary>
		/// 暂停的任务
		/// </summary>
		public TreeTaskBase StopTask;

		/// <summary>
		/// 任务令牌事件
		/// </summary>
		public RuleActuator<TreeTaskTokenEvent> TokenEvent;

		/// <summary>
		/// 任务状态
		/// </summary>
		public TokenState State { get => taskState; }

		public override string ToString()
		{
			return $"TreeTaskToken({StopTask?.Id})";
		}

		/// <summary>
		/// 继续执行
		/// </summary>
		public void Continue()
		{
			taskState = TokenState.Running;
			TokenEvent?.Send(taskState);
			StopTask?.SetCompleted();
			StopTask = null;
		}

		/// <summary>
		/// 暂停
		/// </summary>
		public void Stop()
		{
			taskState = TokenState.Stop;
			TokenEvent?.Send(taskState);
		}

		/// <summary>
		/// 取消
		/// </summary>
		public void Cancel()
		{
			taskState = TokenState.Cancel;
			TokenEvent?.Send(taskState);
			TokenEvent.Clear();
			StopTask = null;
		}
	}

	public static class TreeTaskTokenRule
	{
		private class AddRule : AddRule<TreeTaskToken>
		{
			protected override void Execute(TreeTaskToken self)
			{
				self.taskState = TokenState.Running;
				self.AddChild(out self.TokenEvent);
			}
		}

		private class Remove : BeforeRemoveRule<TreeTaskToken>
		{
			protected override void Execute(TreeTaskToken self)
			{
				//令牌释放回收前，让挂起任务继续执行
				if (self.taskState == TokenState.Stop) self.Continue();
			}
		}

		private class RemoveRule : RemoveRule<TreeTaskToken>
		{
			protected override void Execute(TreeTaskToken self)
			{
				self.StopTask = null;
				self.TokenEvent = null;
			}
		}

		private class TreeTaskTokenEventRule : TreeTaskTokenEventRule<TreeTaskToken>
		{
			protected override void Execute(TreeTaskToken self, TokenState state)
			{
				if (state == TokenState.Cancel)
				{
					self.Cancel();
				}
			}
		}
	}
}