
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

        public Action SetResult { get; set; }

        public AsyncTask():base()
        {
            SetResult = SetResultMethod;
        }

        public void GetResult() {  }
        private void SetResultMethod()
        {
            continuation?.Invoke();
            Dispose();
        }

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
        public Action<T> SetResult { get; set; }

        public T Result;

        public AsyncTask()
        {
            SetResult = SetResultFunc;
        }

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

        private void SetResultFunc(T result)
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
