/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

*/
using System;
using WorldTree.Internal;

namespace WorldTree
{
	/// <summary>
	/// 切换世界线任务
	/// </summary>
	public class TreeTaskSwitchWorld : AwaiterBase
		, ChildOf<WorldContext>
		, AsRule<Awake<WorldContext>>
		, AsRule<TreeTaskSetResuIt>
	{
		/// <summary>
		/// 世界上下文
		/// </summary>
		public WorldContext worldContext;

		/// <summary>
		/// 内部协程
		/// </summary>
		private async TreeTaskVoid InnerCoroutine()
		{
			await this;
		}

		/// <summary>
		/// 协程启动
		/// </summary>
		public void Coroutine()
		{
			InnerCoroutine().Coroutine();
			this.FindSyncTaskSetCompleted();
		}
		/// <summary>
		/// 协程启动
		/// </summary>
		public void Coroutine(TreeTaskToken treeTaskToken)
		{
			this.SetToken(treeTaskToken);
			InnerCoroutine().Coroutine();
			this.FindSyncTaskSetCompleted();

		}

		public override void OnCompleted(Action continuation)
		{
			UnsafeOnCompleted(continuation);
			worldContext.Post(SetCompleted);
		}
	}

	public static class TreeTaskSwitchWorldRule
	{
		class AwakeRule : AwakeRule<TreeTaskSwitchWorld, WorldContext>
		{
			protected override void Execute(TreeTaskSwitchWorld self, WorldContext worldContext)
			{
				self.worldContext = worldContext;
			}
		}

		class RemoveRule : RemoveRule<TreeTaskSwitchWorld>
		{
			protected override void Execute(TreeTaskSwitchWorld self)
			{
				self.worldContext = null;
			}
		}

	}
}
