using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WorldTree
{
    public static class AsyncTaskCompletedExtension
    {
        /// <summary>
        /// 立即完成，不可用于死循环
        /// </summary>
        public static AsyncTaskCompleted AsyncTaskCompleted(this Entity self)
        {
            return self.AddChildren<AsyncTaskCompleted>();
        }
    }

    [AsyncMethodBuilder(typeof(AsyncTaskCompletedMethodBuilder))]
    public class AsyncTaskCompleted : Entity, ICriticalNotifyCompletion
    {
        [DebuggerHidden]
        public AsyncTaskCompleted GetAwaiter()
        {
            return this;
        }

        [DebuggerHidden]
        public bool IsCompleted => true;

        [DebuggerHidden]
        public void GetResult()
        {
            RemoveSelf();
        }

        [DebuggerHidden]
        public void OnCompleted(Action continuation)
        {
        }

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action continuation)
        {
        }
    }
}
