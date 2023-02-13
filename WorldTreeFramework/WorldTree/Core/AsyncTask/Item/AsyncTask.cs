
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
    public class AsyncTask : AsyncTaskBase
    {
        public AsyncTask GetAwaiter() => this;
        public override bool IsCompleted { get; set; }

        public Action SetResult { get; set; }


        public AsyncTask() : base()
        {
            SetResult = SetCompleted;
        }

        public void GetResult() { }

        [DebuggerHidden]
        private async AsyncTaskVoid InnerCoroutine()
        {
            await this;
        }

        /// <summary>
        /// 协程启动
        /// </summary>
        public AsyncTask Coroutine()
        {
            InnerCoroutine().Coroutine();
            return this;
        }
    }

  

    /// <summary>
    /// 泛型异步任务
    /// </summary>
    [AsyncMethodBuilder(typeof(Internal.AsyncTaskMethodBuilder<>))]
    public class AsyncTask<T> : AsyncTaskBase
    {
        public AsyncTask<T> GetAwaiter() => this;
        public override bool IsCompleted { get; set; }
        public Action<T> SetResult { get; set; }

        public T Result;

        public AsyncTask():base()
        {
            SetResult = SetResultFunc;
        }

        public T GetResult()
        {
            return Result;
        }

        private void SetResultFunc(T result)
        {
            Result = result;
            SetCompleted();
        }



        [DebuggerHidden]
        private async AsyncTaskVoid InnerCoroutine()
        {
            await this;
        }

        /// <summary>
        /// 协程启动
        /// </summary>
        public AsyncTask<T> Coroutine()
        {
            InnerCoroutine().Coroutine();
            return this;
        }
    }

}
