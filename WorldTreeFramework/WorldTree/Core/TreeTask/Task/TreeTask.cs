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
    /// <remarks>因为继承Node所以可以挂在树上</remarks>
    [AsyncMethodBuilder(typeof(Internal.TreeTaskMethodBuilder))]
    public class TreeTask : TreeTaskBase
        ,AsRule<IAwakeRule>
    {
        public TreeTask GetAwaiter() => this;
        public override bool IsCompleted { get; set; }

        public Action SetResult { get; set; }


        public TreeTask()
        {
            SetResult = SetCompleted;
        }

        public void GetResult()
        {
        }

        [DebuggerHidden]
        private async TreeTaskVoid InnerCoroutine()
        {
            await this;
        }

        /// <summary>
        /// 协程启动
        /// </summary>
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }
    }


    /// <summary>
    /// 泛型异步任务
    /// </summary>
    [AsyncMethodBuilder(typeof(Internal.AsyncTaskMethodBuilder<>))]
    public class TreeTask<T> : TreeTaskBase
        ,AsRule<IAwakeRule>
    {
        public TreeTask<T> GetAwaiter() => this;
        public override bool IsCompleted { get; set; }
        public Action<T> SetResult { get; set; }

        public T Result;

        public TreeTask() : base()
        {
            SetResult = SetResultMethod;
        }

        public T GetResult()
        {
            return Result;
        }

        private void SetResultMethod(T result)
        {
            Result = result;
            SetCompleted();
        }


        [DebuggerHidden]
        private async TreeTaskVoid InnerCoroutine()
        {
            await this;
        }

        /// <summary>
        /// 协程启动
        /// </summary>
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }
    }
}