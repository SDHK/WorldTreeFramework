
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 异步任务构建器

*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;


namespace WorldTree.Internal
{
    /// <summary>
    /// 异步任务构建器
    /// </summary>
    public struct TreeTaskMethodBuilder
    {
        private TreeTask task;
        // 1. Static Create method.

        [DebuggerHidden]
        public static TreeTaskMethodBuilder Create()
        {
            TreeTaskMethodBuilder builder = new TreeTaskMethodBuilder();
            World.Log($"Task ！！！！！！静态构建方法！！！！！！！");
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public TreeTask Task
        {
            get
            {
                World.Log($"Task 获取Task");
                return task;
            }
        }

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            World.Log($"[{task.Id}]Task 设置异常 {exception}");
            task.SetException(exception);
        }

        // 4. SetResult
        public void SetResult()
        {
            World.Log($"[{task.Id}]Task 设置结果");
            task.SetResult();
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                awaiter.Parent.AddChild(out task);
            }

            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {

            if (task == null)
            {
                awaiter.Parent.AddChild(out task);

                if (awaiter.treeTaskToken is null)
                {
                    task.relevanceTask = awaiter;
                }
                else
                {
                    task.treeTaskToken = awaiter.treeTaskToken;
                }
                World.Log($"({awaiter.treeTaskToken != null})（{stateMachine.GetType()}）传入 awaiter [{awaiter.Id}] =>  新建 Task [{task.Id}] 6. 等待不安全完成");
            }
            else
            {

                if (task.treeTaskToken != null)
                {
                    if (awaiter.treeTaskToken is null)
                    {
                        awaiter.treeTaskToken = task.treeTaskToken;
                        if (awaiter.relevanceTask != null && awaiter.relevanceTask.treeTaskToken is null)
                        {
                            awaiter.relevanceTask.treeTaskToken = task.treeTaskToken;
                        }
                    }
                }
                World.Log($"({task.treeTaskToken != null})（{stateMachine.GetType()}）已经存在 Task [{task.Id}] 移动到 => ({awaiter.treeTaskToken != null}) awaiter [{awaiter.Id}] 6. 等待不安全完成！！！！");

            }
            awaiter.OnCompleted(stateMachine.MoveNext);



            World.Log($"Task 6. 等待不安全完成后");

        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            World.Log($"Task 7. 开始前");
            stateMachine.MoveNext();
            World.Log($"Task 7. 开始后");
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            World.Log($"Task 8. 设置状态机");
        }
    }



    public struct TreeTaskMethodBuilder<T>
    {
        private TreeTask<T> task;
        // 1. Static Create method.

        [DebuggerHidden]
        public static TreeTaskMethodBuilder<T> Create()
        {
            TreeTaskMethodBuilder<T> builder = new TreeTaskMethodBuilder<T>();
            World.Log($"Task<{typeof(T)}> ！！！！！！静态构建方法！！！！！！！");
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public TreeTask<T> Task
        {
            get
            {
                World.Log($"Task<{typeof(T)}> 获取Task");
                return task;
            }
        }

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            World.Log($"[{task.Id}]Task<{typeof(T)}> 设置异常");
            task.SetException(exception);
        }

        // 4. SetResult
        [DebuggerHidden]

        public void SetResult(T ret)
        {
            World.Log($"[{task.Id}]Task<{typeof(T)}> 设置结果");
            task.SetResult(ret);
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                awaiter.Parent.AddChild(out task);
            }
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                awaiter.Parent.AddChild(out task);
                if (awaiter.treeTaskToken is null)
                {
                    task.relevanceTask = awaiter;
                }
                else
                {
                    task.treeTaskToken = awaiter.treeTaskToken;
                }
                World.Log($"({awaiter.treeTaskToken != null})（{stateMachine.GetType()}）传入 awaiter [{awaiter.Id}] => 新建 Task<{typeof(T)}> [{task.Id}]  6. 等待不安全完成");
            }
            else
            {
                if (task.treeTaskToken != null)
                {
                    if (awaiter.treeTaskToken is null)
                    {
                        awaiter.treeTaskToken = task.treeTaskToken;
                        if (awaiter.relevanceTask != null && awaiter.relevanceTask.treeTaskToken is null)
                        {
                            awaiter.relevanceTask.treeTaskToken = task.treeTaskToken;
                        }
                    }
                }
                World.Log($"({task.treeTaskToken != null})（{stateMachine.GetType()}）已经存在 Task<{typeof(T)}> [{task.Id}] 移动到 => awaiter [{awaiter.Id}] 6. 等待不安全完成！！！！");
            }
            awaiter.OnCompleted(stateMachine.MoveNext);
            World.Log($"Task<{typeof(T)}> 6. 等待不安全完成2");
        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            World.Log($"Task<{typeof(T)}> 7. 开始前");
            stateMachine.MoveNext();
            World.Log($"Task<{typeof(T)}> 7. 开始后");
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            World.Log($"Task<{typeof(T)}> 8. 设置状态机");
        }
    }


}
