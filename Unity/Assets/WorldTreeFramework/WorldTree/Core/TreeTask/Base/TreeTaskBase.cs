
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
    /// 同步任务标记接口
    /// </summary>
    /// <remarks>任务将会在构建器中以同步形式直接执行</remarks>
    public interface ISyncTask { }

    /// <summary>
    /// 树异步任务基类
    /// </summary>
    public abstract class TreeTaskBase : Node, ICriticalNotifyCompletion, ChildOf<INode>
        , AsRule<ITreeTaskTokenEventRule>
    {
        /// <summary>
        /// 树任务令牌
        /// </summary>
        public TreeTaskToken m_TreeTaskToken;

        /// <summary>
        /// 关联令牌的任务
        /// </summary>
        public TreeTaskBase m_RelevanceTask;



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
        public Action m_Continuation;

        /// <summary>
        /// 设置完成
        /// </summary>
        public void SetCompleted()
        {
            IsCompleted = true;
            if (m_TreeTaskToken is null)
            {
                m_Continuation?.Invoke();
                Dispose();
            }
            else
            {
                switch (m_TreeTaskToken.State)
                {
                    case TaskState.Running:
                        m_Continuation?.Invoke();
                        Dispose();
                        break;
                    case TaskState.Stop:
                        m_TreeTaskToken.stopTask = this;
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
            this.m_Continuation = continuation;
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
                m_Continuation?.Invoke();
                Dispose();
            }
        }

        /// <summary>
        /// 设置并传递令牌
        /// </summary>
        public TreeTaskBase SetToken(TreeTaskToken treeTaskToken)
        {
            TreeTaskBase NowAwaiter = this;
            while (NowAwaiter != null)
            {
                NowAwaiter.m_TreeTaskToken = treeTaskToken;
                treeTaskToken.tokenEvent.Add(NowAwaiter, default(ITreeTaskTokenEventRule));
                NowAwaiter = NowAwaiter.m_RelevanceTask;
            }
            return this;
        }

        /// <summary>
        /// 同步任务完成
        /// </summary>
        public void TrySyncTaskSetCompleted()
        {
            TreeTaskBase NowAwaiter = this;
            while (NowAwaiter != null)
            {
                if (NowAwaiter is ISyncTask)
                {
                    NowAwaiter.SetCompleted();
                }
                NowAwaiter = NowAwaiter.m_RelevanceTask;
            }

        }

        public override void OnDispose()
        {
            IsCompleted = false;
            m_TreeTaskToken = null;
            m_RelevanceTask = null;
            m_Continuation = null;

            base.OnDispose();
        }
    }

    class TreeTaskBaseTaskTokenEventRule : TreeTaskTokenEventRule<TreeTaskBase, TaskState>
    {
        protected override void OnEvent(TreeTaskBase self, TaskState state)
        {
            if (state == TaskState.Cancel)
            {
                self.Dispose();
            }
        }
    }
}
