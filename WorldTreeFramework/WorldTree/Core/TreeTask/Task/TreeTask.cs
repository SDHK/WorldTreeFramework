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
    {
        public TreeTask GetAwaiter() => this;
        public override bool IsCompleted { get; set; }

        public class SetResult : RuleNode<TreeTask>, AsRule<ISendRule> { }

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


    public static class TreeTaskRule
    {
        class SetResultSendRule : SendRule<TreeTask.SetResult>
        {
            public override void OnEvent(TreeTask.SetResult self)
            {
                self.Node.SetCompleted();
            }
        }

        class SetResultSendRule<T> : SendRule<TreeTask<T>.SetResult, T>
        {
            public override void OnEvent(TreeTask<T>.SetResult self, T result)
            {
                self.Node.Result = result;
                self.Node.SetCompleted();
            }
        }
    }

    /// <summary>
    /// 泛型异步任务
    /// </summary>
    [AsyncMethodBuilder(typeof(Internal.AsyncTaskMethodBuilder<>))]
    public class TreeTask<T> : TreeTaskBase
        , AsRule<IAwakeRule>
    {
        public TreeTask<T> GetAwaiter() => this;
        public override bool IsCompleted { get; set; }

        public class SetResult : RuleNode<TreeTask<T>>, AsRule<ISendRule<T>> { }

        public T Result;

        public T GetResult()
        {
            return Result;
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