
/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/27 9:47

* 描述：异步任务
* 
*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;



namespace WorldTree
{

    public interface IAsyncTask : ICriticalNotifyCompletion
    {
        bool IsCompleted { get; set; }
        IAsyncTask GetResult();
        void SetResult();
        void SetException(Exception exception);
    }


    public interface IAsyncTask<T> : ICriticalNotifyCompletion
    {
        bool IsCompleted { get; set; }
        void SetResult(T result);
        T GetResult();
        void SetException(Exception exception);

    }

    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder))]
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

        public IAsyncTask GetResult()
        {
            return this;
        }
        public void SetResult()
        {
            continuation?.Invoke();
            RemoveSelf();
        }

    }


    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder<>))]
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
            RemoveSelf();
        }

        public void SetException(Exception exception)
        {
        }


    }






}
