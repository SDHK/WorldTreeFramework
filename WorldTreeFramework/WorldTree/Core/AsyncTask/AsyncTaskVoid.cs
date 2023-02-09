
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 空异步任务
*  
* 用于启动异步方法

*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WorldTree.Internal
{
    /// <summary>
    /// 空异步任务
    /// </summary>
    [AsyncMethodBuilder(typeof(AsyncTaskVoidMethodBuilder))]
    public struct AsyncTaskVoid : ICriticalNotifyCompletion
    {
        [DebuggerHidden]
        public void Coroutine() {  }
        [DebuggerHidden]
        public bool IsCompleted => true;
        [DebuggerHidden]
        public void OnCompleted(Action continuation)  { }
        [DebuggerHidden]
        public void UnsafeOnCompleted(Action continuation) {  }
    }

}
