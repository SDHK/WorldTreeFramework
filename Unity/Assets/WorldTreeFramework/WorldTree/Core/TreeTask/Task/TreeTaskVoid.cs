
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 空异步任务
*  
* 用于代替异步方法返回的 void
* 并且告诉调用者，这是个不可等待的 "协程"。
* 
* 如果直接使用 void 会导致程序调用C#原生的Task。
* 
* 

*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WorldTree.Internal;

namespace WorldTree
{
    /// <summary>
    /// 空异步任务
    /// </summary>
    [AsyncMethodBuilder(typeof(TreeTaskVoidMethodBuilder))]
    public struct TreeTaskVoid : ICriticalNotifyCompletion
    {
        [DebuggerHidden]
        public void Coroutine() { }
        [DebuggerHidden]
        public bool IsCompleted => true;
        [DebuggerHidden]
        public void OnCompleted(Action continuation) { }
        [DebuggerHidden]
        public void UnsafeOnCompleted(Action continuation) { }

    }
}
