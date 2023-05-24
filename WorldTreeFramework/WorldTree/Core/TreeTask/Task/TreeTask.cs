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

        public class SetResultRuleNode : RuleNode<TreeTask>, AsRule<ISendRuleBase> { }

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
        , AsRule<IAwakeRule>
    {
        public TreeTask<T> GetAwaiter() => this;
        public override bool IsCompleted { get; set; }

        public class SetResultRuleNode : RuleNode<TreeTask<T>>, AsRule<ISendRuleBase<T>> { }

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
    public static class TreeTaskRule
    {
        class SetResultSendRule : SendRule<TreeTask.SetResultRuleNode>
        {
            public override void OnEvent(TreeTask.SetResultRuleNode self)
            {
                self.Node.SetResult();
            }
        }

        class SetResultSendRule<T> : SendRule<TreeTask<T>.SetResultRuleNode, T>
        {
            public override void OnEvent(TreeTask<T>.SetResultRuleNode self, T result)
            {
                self.Node.SetResult(result);
            }
        }

        public static void SetResult(this TreeTask self)
        {
            self.SetCompleted();
        }
        public static void SetResult<T>(this TreeTask<T> self, T result)
        {
            self.Result = result;
            self.SetCompleted();
        }

    }

}