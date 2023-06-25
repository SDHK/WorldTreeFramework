
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/9 14:34

* 描述： 世界树异步任务基类

*/

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
    /// <summary>
    /// 树异步任务基类
    /// </summary>
    public abstract class TreeTaskBase : Node, ICriticalNotifyCompletion, ChildOf<INode>
    {
        /// <summary>
        /// 树任务令牌
        /// </summary>
        public TreeTaskToken treeTaskToken;

        /// <summary>
        /// 关联令牌的任务
        /// </summary>
        public TreeTaskBase relevanceTask;



        /// <summary>
        /// 是否完成
        /// </summary>
        public abstract bool IsCompleted { get; set; }

        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// 延续
        /// </summary>
        public Action continuation;

        /// <summary>
        /// 设置完成
        /// </summary>
        public void SetCompleted()
        {
            World.Log($"[{Id}] 任务完成 : {IsActive}");

            IsCompleted = true;
            if (treeTaskToken is null)
            {
                continuation?.Invoke();
                Dispose();
            }
            else
            {
                switch (treeTaskToken.State)
                {
                    case TaskState.Running:
                        continuation?.Invoke();
                        Dispose();
                        break;
                    case TaskState.Stop:
                        treeTaskToken.stopTask = this;
                        break;
                    case TaskState.Cancel:
                        Dispose();
                        break;
                }
            }
        }

        /// <summary>
        /// 设置异常
        /// </summary>
        public void SetException(Exception exception)
        {
            this.Exception = exception;
        }

        /// <summary>
        /// 不安全完成时
        /// </summary>
        public void UnsafeOnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }
        /// <summary>
        /// 完成时
        /// </summary>
        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        /// <summary>
        /// 继续
        /// </summary>
        public void Continue()
        {
            if (IsCompleted)
            {
                continuation?.Invoke();
                Dispose();
            }
        }

        /// <summary>
        /// 设置并传递令牌
        /// </summary>
        public TreeTaskBase SetToken(TreeTaskToken treeTaskToken)
        {
            TreeTaskBase NowAwaiter = this;
            while (NowAwaiter is not null && NowAwaiter.treeTaskToken is null)
            {
                NowAwaiter.treeTaskToken = treeTaskToken;
                NowAwaiter = NowAwaiter.relevanceTask;
            }
            return this;
        }

        public override void OnDispose()
        {
            IsCompleted = false;
            continuation = null;

            base.OnDispose();
        }
    }
}
