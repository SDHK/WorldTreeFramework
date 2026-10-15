/****************************************

* 作者：闪电黑客
* 日期：2026/10/14

* 描述：

*/
using System.Runtime.CompilerServices;
using WorldTree.Internal;

namespace WorldTree
{
	/// <summary>
	/// 定时到期。任务上是完成，令牌上是取消。
	/// </summary>
	public interface WorldTaskTimeout : ISendRule { }

	/// <summary>
	/// 异步任务
	/// </summary>
	[AsyncMethodBuilder(typeof(WorldTaskMethodBuilder))]
	public class WorldTask : WorldTaskBase, AsRule<WorldTaskTimeout>
	{
		/// <summary>
		/// 设置成功
		/// </summary>
		public void SetResult() => SetCompleted();
	}

	/// <summary>
	/// 任务到期完成
	/// </summary>
	public static class WorldTaskTimeoutRule
	{
		class Timeout : SendRule<WorldTask, WorldTaskTimeout>
		{
			/// <summary>
			/// 到期完成。已经结束的任务不再设置。
			/// </summary>
			protected override void Execute(WorldTask self)
			{
				if (self.IsDisposed || self.IsCompleted) return;
				self.SetResult();
			}
		}
	}

	/// <summary>
	/// 带返回值的异步任务
	/// </summary>
	[AsyncMethodBuilder(typeof(WorldTaskMethodBuilder<>))]
	public class WorldTask<T> : WorldTaskBase
	{
		/// <summary>
		/// 结果
		/// </summary>
		public T Result;

		/// <summary>
		/// 获取等待器
		/// </summary>
		public new WorldTask<T> GetAwaiter() => this;

		/// <summary>
		/// 获取结果
		/// </summary>
		public new virtual T GetResult()
		{
			T value = Result;
			OnGetResult();
			return value;
		}

		/// <summary>
		/// 设置成功
		/// </summary>
		public void SetResult(T value)
		{
			Result = value;
			SetCompleted();
		}

		public override void OnCreate()
		{
			Result = default;
			base.OnCreate();
		}

		public override void OnDispose()
		{
			Result = default;
			base.OnDispose();
		}
	}
}
