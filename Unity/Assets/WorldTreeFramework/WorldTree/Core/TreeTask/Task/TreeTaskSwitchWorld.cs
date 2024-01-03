/****************************************

* 作者： 闪电黑客
* 日期： 2024/01/02 09:31:36

* 描述： 切换世界线任务
* 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WorldTree.Internal;

namespace WorldTree
{
	[AsyncMethodBuilder(typeof(TreeTaskSwitchWorldMethodBuilder))]
	public class TreeTaskSwitchWorld : TreeTaskBase
		, ChildOf<INode>
		, AsRule<IAwakeRule<WorldContext>>
		, AsRule<ITreeTaskSetResuItRule>
	{
		public TreeTaskSwitchWorld GetAwaiter() => this;
		public override bool IsCompleted { get; set; }

		/// <summary>
		/// 世界上下文
		/// </summary>
		public WorldContext worldContext;

		public void GetResult() { }

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
			this.TrySyncTaskSetCompleted();
		}
		/// <summary>
		/// 协程启动
		/// </summary>
		public void Coroutine(TreeTaskToken treeTaskToken)
		{
			this.SetToken(treeTaskToken);
			InnerCoroutine().Coroutine();
			this.TrySyncTaskSetCompleted();

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
			protected override void OnEvent(TreeTaskSwitchWorld self, WorldContext worldContext)
			{
				self.worldContext = worldContext;
			}
		}

		class RemoveRule : RemoveRule<TreeTaskSwitchWorld>
		{
			protected override void OnEvent(TreeTaskSwitchWorld self)
			{
				self.worldContext = null;
			}
		}

	}
}
