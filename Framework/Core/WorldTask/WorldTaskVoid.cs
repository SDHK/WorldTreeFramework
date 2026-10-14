/****************************************

* 作者：闪电黑客
* 日期：2026/10/14

* 描述：

*/
using System;
using System.Runtime.CompilerServices;

namespace WorldTree.Internal
{
	/// <summary>
	/// 无返回的异步任务，只给顶层 Coroutine 启动用
	/// </summary>
	[AsyncMethodBuilder(typeof(WorldTaskVoidMethodBuilder))]
	public struct WorldTaskVoid
	{
		/// <summary>
		/// 启动标记。真正的执行在构建器的 Start 里。
		/// </summary>
		public void Coroutine() { }
	}

	/// <summary>
	/// 无返回异步任务构建器
	/// </summary>
	public struct WorldTaskVoidMethodBuilder
	{
		/// <summary>
		/// 创建构建器
		/// </summary>
		public static WorldTaskVoidMethodBuilder Create() => default;

		/// <summary>
		/// 任务
		/// </summary>
		public WorldTaskVoid Task => default;

		/// <summary>
		/// 启动状态机
		/// </summary>
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
			=> stateMachine.MoveNext();

		/// <summary>
		/// 设置状态机
		/// </summary>
		public void SetStateMachine(IAsyncStateMachine stateMachine) { }

		/// <summary>
		/// 顶层 Coroutine 没有调用方，异常只输出，不打断心跳
		/// </summary>
		public void SetException(Exception exception) => System.Diagnostics.Debug.WriteLine(exception);

		/// <summary>
		/// 完成
		/// </summary>
		public void SetResult() { }

		/// <summary>
		/// 等待完成
		/// </summary>
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
			where TAwaiter : WorldTaskBase, INotifyCompletion
			where TStateMachine : IAsyncStateMachine
			=> AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);

		/// <summary>
		/// 等待完成
		/// </summary>
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
			where TAwaiter : WorldTaskBase, ICriticalNotifyCompletion
			where TStateMachine : IAsyncStateMachine
			=> awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
	}
}
