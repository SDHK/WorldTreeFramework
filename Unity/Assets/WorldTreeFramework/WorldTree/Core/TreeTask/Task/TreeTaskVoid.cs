
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 空异步任务
*  
* 用于代替异步方法返回的 void，如果直接使用 void 会导致程序调用C#原生的Task。
* 
* 
* 主要作用是捕获异常。
* 
* IsCompleted 是 true，表示任务已经完成，将会直接跳过这个异步。
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
