
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 异步任务完成类

*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WorldTree.Internal
{
    /// <summary>
    /// 异步任务完成类
    /// </summary>
    [AsyncMethodBuilder(typeof(AsyncTaskCompletedMethodBuilder))]
    public class AsyncTaskCompleted : Entity,ICriticalNotifyCompletion
    {
        [DebuggerHidden]
        public AsyncTaskCompleted GetAwaiter() => this;
        [DebuggerHidden]
        public bool IsCompleted => true;
        [DebuggerHidden]
        public void GetResult() { Dispose(); }
        [DebuggerHidden]
        public void OnCompleted(Action continuation) { }
        [DebuggerHidden]
        public void UnsafeOnCompleted(Action continuation) { }
    }
}
