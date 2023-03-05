
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 异步任务完成方法构建器

*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace WorldTree.Internal
{
    /// <summary>
    /// 异步任务完成方法构建器
    /// </summary>
    public struct TreeTaskCompletedMethodBuilder
    {
        private TreeTaskCompleted task;

        // 静态构建方法
        [DebuggerHidden]
        public static TreeTaskCompletedMethodBuilder Create()
        {
            TreeTaskCompletedMethodBuilder builder = new TreeTaskCompletedMethodBuilder();
            return builder;
        }

        public TreeTaskCompleted Task
        {
            get
            {
                return task;
            }
        }
        // 设置异常
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            task.SetException(exception);
        }

        // 设置结果
        [DebuggerHidden]
        public void SetResult()
        {
            task.SetResult();
        }

        // 5. 等待完成
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                task = awaiter.Parent.AddChildren<TreeTaskCompleted>();
            }
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. 等待不安全完成
        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                task = awaiter.Parent.AddChildren<TreeTaskCompleted>();
            }
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
        }

        // 7. 开始
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        // 8. 设置状态机
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }
}
