
/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/27 9:47

* 描述： 异步任务
* 
*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WorldTree.Internal;

namespace WorldTree
{
    /// <summary>
    /// 异步任务
    /// </summary>
    [AsyncMethodBuilder(typeof(Internal.AsyncTaskMethodBuilder))]
    public class AsyncTask : Entity, IAsyncTask
    {
        public AsyncTask GetAwaiter() => this;

        public Action continuation;
        public Exception Exception { get; private set; }

        public bool IsCompleted { get; set; }

        [DebuggerHidden]
        private async AsyncTaskVoid InnerCoroutine()
        {
            await this;
        }

        /// <summary>
        /// 协程启动
        /// </summary>
        [DebuggerHidden]
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }
        public void SetException(Exception exception)
        {
            this.Exception = exception;
        }

        public void GetResult()
        {
        }
        public void SetResult()
        {
            continuation?.Invoke();
            Dispose();
        }

    }

    /// <summary>
    /// 泛型异步任务
    /// </summary>
    [AsyncMethodBuilder(typeof(Internal.AsyncTaskMethodBuilder<>))]
    public class AsyncTask<T> : Entity, IAsyncTask<T>
    {
        public AsyncTask<T> GetAwaiter() => this;
        public Action continuation;
        public bool IsCompleted { get; set; }

        public T Result;

        [DebuggerHidden]
        private async AsyncTaskVoid InnerCoroutine()
        {
            await this;
        }

        /// <summary>
        /// 协程启动
        /// </summary>
        [DebuggerHidden]
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }


        public T GetResult()
        {
            return Result;
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);

        }
        public void UnsafeOnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }
        public void SetResult(T result)
        {
            Result = result;
            continuation?.Invoke();
            Dispose();
        }

        public void SetException(Exception exception)
        {
        }


    }






}
