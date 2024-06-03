/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/27 9:47

* 描述： 异步任务
* 
*/

using System.Diagnostics;
using System.Runtime.CompilerServices;
using WorldTree.Internal;

namespace WorldTree
{
	/// <summary>
	/// 异步任务
	/// </summary>
	/// <remarks>因为继承Node所以可以挂在树上</remarks>
	[AsyncMethodBuilder(typeof(TreeTaskMethodBuilder))]
	public class TreeTask : TreeTaskBase
		, ChildOf<INode>
		, AsAwake
		, AsTreeTaskSetResuIt
	{
		public TreeTask GetAwaiter() => this;
		public override bool IsCompleted { get; set; }

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
			//this.Log($"this任务[{this.Id}]");
			InnerCoroutine().Coroutine();
			//this.Log($"this任务[{this.Id}]尝试完成同步任务！！！");
			this.FindSyncTaskSetCompleted();
		}
		/// <summary>
		/// 协程启动
		/// </summary>
		public void Coroutine(TreeTaskToken treeTaskToken)
		{
			//this.Log($"this任务[{this.Id}]");
			this.SetToken(treeTaskToken);
			InnerCoroutine().Coroutine();
			//this.Log($"this任务[{this.Id}]尝试完成同步任务！！！");
			this.FindSyncTaskSetCompleted();

		}

		public void SetResult()
		{
			this.SetCompleted();
		}
	}




	/// <summary>
	/// 泛型异步任务
	/// </summary>
	[AsyncMethodBuilder(typeof(TreeTaskMethodBuilder<>))]
	public class TreeTask<T> : TreeTaskBase
		, ChildOf<INode>
		, AsAwake
		, AsTreeTaskSetResuIt<T>
	{
		public TreeTask<T> GetAwaiter() => this;
		public override bool IsCompleted { get; set; }

		public T Result;

		public T GetResult()
		{
			return Result;
		}

		[DebuggerHidden]
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

		public void SetResult(T result)
		{
			this.Result = result;
			this.SetCompleted();
		}
	}

	public static class TreeTaskRule
	{
		class SetResultSendRule : TreeTaskSetResuItRule<TreeTask>
		{
			protected override void Execute(TreeTask self)
			{
				self.SetResult();
			}
		}

		class SetResultSendRule<T> : TreeTaskSetResuItRule<TreeTask<T>, T>
		{
			protected override void Execute(TreeTask<T> self, T result)
			{
				self.SetResult(result);
			}
		}
	}
}