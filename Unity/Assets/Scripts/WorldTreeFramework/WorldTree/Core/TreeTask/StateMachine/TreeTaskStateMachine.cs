/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：树任务方法状态机

*/
using System.Runtime.CompilerServices;

namespace WorldTree.Internal
{

	public interface ITreeTaskStateMachine : INode
	{
		public void MoveNext();
	}

	/// <summary>
	/// 树任务方法状态机
	/// </summary>
	public class TreeTaskStateMachine : Node, ITreeTaskStateMachine
		, TempOf<INode>
		, AsAwake
		, AsTreeTaskTokenEvent
	{
		/// <summary>
		/// 当前状态机任务
		/// </summary>
		public IAsyncStateMachine m_StateMachine;

		/// <summary>
		/// 设置状态机
		/// </summary>
		public void SetStateMachine<T>(ref T stateMachine)
			where T : IAsyncStateMachine
		{
			m_StateMachine = stateMachine;
		}

		/// <summary>
		/// 状态机继续下一步
		/// </summary>
		public void MoveNext()
		{
			//this.Log($"状态机MoveNext !!!  {!this.IsRecycle}");
			//假如当前状态机的任务丢失了，那么可能是令牌取消了，将Tsak释放了
			//那么任务等于断了，状态机也就没有继续的必要了。
			 m_StateMachine?.MoveNext();
		}
	}

	public static class TreeTaskStateMachineRule
	{

		class Remove : RemoveRule<TreeTaskStateMachine>
		{
			protected override void Execute(TreeTaskStateMachine self)
			{
				self.m_StateMachine = null;
			}
		}

		class TreeTaskTokenEvent : TreeTaskTokenEventRule<TreeTaskStateMachine>
		{
			protected override void Execute(TreeTaskStateMachine self, TaskState state)
			{
				//假如令牌取消了，直接释放状态机。
				if (state == TaskState.Cancel)
				{
					self.Dispose();
				}
			}
		}
	}


}
