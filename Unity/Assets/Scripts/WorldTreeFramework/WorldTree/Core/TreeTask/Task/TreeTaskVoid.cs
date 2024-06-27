
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 空异步任务
*  
* 用于代替异步方法返回的 void，如果直接使用 void 会导致程序调用C#原生的Task。
* 
* 它主要作用是让异常可以被捕获。
* 
* IsCompleted 是 true，表示任务已经完成，将会直接跳过这个异步。
* 

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
