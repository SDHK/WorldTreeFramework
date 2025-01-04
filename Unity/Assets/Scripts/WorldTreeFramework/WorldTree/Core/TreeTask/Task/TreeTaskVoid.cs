/****************************************

* 作者：闪电黑客
* 日期：2024/6/3 10:29

* 描述：

*/
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WorldTree.Internal
{
	/// <summary>
	/// 空异步任务
	/// </summary>
	[AsyncMethodBuilder(typeof(TreeTaskVoidMethodBuilder))]
	public struct TreeTaskVoid : ICriticalNotifyCompletion
	{
		/// <summary>
		/// 协程启动
		/// </summary>
		[DebuggerHidden]
		public void Coroutine() { }
		/// <summary>
		/// 默认完成
		/// </summary>
		[DebuggerHidden]
		public bool IsCompleted => true;
		[DebuggerHidden]
		public void OnCompleted(Action continuation) { }
		[DebuggerHidden]
		public void UnsafeOnCompleted(Action continuation) { }

	}
}
