/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/27 9:47

* 描述： 异步任务
* 
*/

using System.Diagnostics;
using System.Runtime.CompilerServices;
using WorldTree.Internal;

namespace WorldTree
{
    /// <summary>
    /// 异步任务
    /// </summary>
    /// <remarks>因为继承Node所以可以挂在树上</remarks>
    [AsyncMethodBuilder(typeof(TreeTaskMethodBuilder))]
    public class TreeTask : TreeTaskBase
        , AsRule<IAwakeRule>
        , AsRule<ITreeTaskSetResuItRule>
    {
        public TreeTask GetAwaiter() => this;
        public override bool IsCompleted { get; set; }

        public void GetResult() { }

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
			this.TrySyncTaskSetCompleted();
		}
		/// <summary>
		/// 协程启动
		/// </summary>
		public void Coroutine(TreeTaskToken treeTaskToken)
        {
            this.SetToken(treeTaskToken);
            InnerCoroutine().Coroutine();
            this.TrySyncTaskSetCompleted();

        }
    }




    /// <summary>
    /// 泛型异步任务
    /// </summary>
    [AsyncMethodBuilder(typeof(TreeTaskMethodBuilder<>))]
    public class TreeTask<T> : TreeTaskBase
        , AsRule<IAwakeRule>
        , AsRule<ITreeTaskSetResuItRule<T>>
    {
        public TreeTask<T> GetAwaiter() => this;
        public override bool IsCompleted { get; set; }

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
			this.TrySyncTaskSetCompleted();
		}
		/// <summary>
		/// 协程启动
		/// </summary>
		public void Coroutine(TreeTaskToken treeTaskToken)
		{
			this.SetToken(treeTaskToken);
			InnerCoroutine().Coroutine();
			this.TrySyncTaskSetCompleted();

		}
	}

    public static class TreeTaskRule
    {
        public static void SetResult(this TreeTask self)
        {
            self.SetCompleted();
        }
        class SetResultSendRule : TreeTaskSetResuItRule<TreeTask>
        {
            protected override void OnEvent(TreeTask self)
            {
                self.SetResult();
            }
        }

        public static void SetResult<T>(this TreeTask<T> self, T result)
        {
            self.Result = result;
            self.SetCompleted();
        }
        class SetResultSendRule<T> : TreeTaskSetResuItRule<TreeTask<T>, T>
        {
            protected override void OnEvent(TreeTask<T> self, T result)
            {
                self.SetResult(result);
            }
        }
    }
}