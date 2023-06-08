
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
            World.Log($"Completed ！！！！！！静态构建方法！！！！！！！");
            return builder;
        }

        public TreeTaskCompleted Task
        {
            get
            {
                World.Log($"Completed 获取Task");
                return task;
            }
        }
        // 设置异常
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            World.Log($"[{task.Id}]Completed 设置异常");
            task.SetException(exception);
        }

        // 设置结果
        public void SetResult()
        {
            World.Log($"[{task.Id}]Completed 设置结果");
            task.SetCompleted();
        }

        // 5. 等待完成
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                awaiter.Parent.AddChild(out task);
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
                task = awaiter.Parent.AddChild(out task);
                World.Log($"（{awaiter.Parent}）（{stateMachine.GetType()}） 新建 Completed [{task.Id}] => awaiter [{awaiter.Id}] 6. 等待不安全完成");
                task.treeTaskController = awaiter.treeTaskController;
            }
            else
            {
                World.Log($"（{awaiter.Parent}）（{stateMachine.GetType()}） 已经存在 Completed [{task.Id}] => awaiter [{awaiter.Id}] 6. 等待不安全完成！！！！");
                awaiter.treeTaskController = task.treeTaskController;
            }
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
            World.Log($"Completed 6. 等待不安全完成2");

        }

        // 7. 开始
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            World.Log($"Completed 7. 开始");
            stateMachine.MoveNext();
            World.Log($"Completed 7. 开始2");

        }

        // 8. 设置状态机
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            World.Log($"Completed 8. 设置状态机");
        }
    }
}
