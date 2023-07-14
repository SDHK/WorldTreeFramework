
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 异步任务构建器

*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using SharpCompress.Writers;
using Unity.VisualScripting;

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
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public TreeTask Task
        {
            get
            {
                World.Log($"[{task.Id}]TreeTask Get");
                return task;
            }
        }

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            task.SetException(exception);
        }

        // 4. SetResult
        public void SetResult()
        {
            World.Log($"[{task.Id}]TreeTask SetResult");
            task.SetResult();
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                awaiter.Parent.AddChild(out task);

                if (awaiter.m_TreeTaskToken is null)
                {
                    task.m_RelevanceTask = awaiter;
                }
                else
                {
                    task.m_TreeTaskToken = awaiter.m_TreeTaskToken;
                    task.m_TreeTaskToken.tokenEvent.Add(task, default(ITreeTaskTokenEventRule));

                }
            }
            else
            {
                if (task.m_TreeTaskToken != null)
                {
                    awaiter.SetToken(task.m_TreeTaskToken);
                }
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

                if (awaiter.m_TreeTaskToken is null)
                {
                    task.m_RelevanceTask = awaiter; 
                    World.Log($"[{task.Id}]TreeTask 等待 1 null [{awaiter.Id}]{awaiter.Type}");

                }
                else
                {
                    task.m_TreeTaskToken = awaiter.m_TreeTaskToken;
                    task.m_TreeTaskToken.tokenEvent.Add(task, default(ITreeTaskTokenEventRule));
                    World.Log($"[{task.Id}]TreeTask 等待2 awaiter[{awaiter.Id}]{awaiter.Type}");

                }
                awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
                World.Log($"[{task.Id}]TreeTask 等待!! awaiter[{awaiter.Id}]{awaiter.Type}");

            }
            else
            {
                if (task.m_TreeTaskToken != null)
                {
                    awaiter.SetToken(task.m_TreeTaskToken);
                    World.Log($"[{task.Id}]TreeTask 等待3 awaiter[{awaiter.Id}]{awaiter.Type}");

                    awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
                    World.Log($"[{task.Id}]TreeTask 等待!! awaiter[{awaiter.Id}]{awaiter.Type}");

                    if (awaiter is TreeTaskCompleted)
                    {
                        awaiter.SetCompleted();
                    }
                    else if (awaiter.m_RelevanceTask is TreeTaskCompleted)
                    { 
                        awaiter.m_RelevanceTask.SetCompleted();
                    }
                }
            }
          
        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                World.Log($"TreeTask stateMachine.MoveNext");
            }
            else
            {
                World.Log($"[{task.Id}]TreeTask stateMachine.MoveNext");
            }
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
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
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public TreeTask<T> Task
        {
            get
            {
                World.Log($"[{task.Id}]TreeTask<{typeof(T)}> Get");
                return task;
            }
        }

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            task.SetException(exception);
        }

        // 4. SetResult
        [DebuggerHidden]

        public void SetResult(T ret)
        {
            World.Log($"[{task.Id}]TreeTask<{typeof(T)}> SetResult");

            task.SetResult(ret);
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                awaiter.Parent.AddChild(out task);
                if (awaiter.m_TreeTaskToken is null)
                {
                    task.m_RelevanceTask = awaiter;
                }
                else
                {
                    task.m_TreeTaskToken = awaiter.m_TreeTaskToken;
                    task.m_TreeTaskToken.tokenEvent.Add(task, default(ITreeTaskTokenEventRule));

                }
            }
            else
            {
                if (task.m_TreeTaskToken != null)
                {
                    awaiter.SetToken(task.m_TreeTaskToken);
                }
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
                if (awaiter.m_TreeTaskToken is null)
                {
                    task.m_RelevanceTask = awaiter;
                }
                else
                {
                    task.m_TreeTaskToken = awaiter.m_TreeTaskToken;
                    task.m_TreeTaskToken.tokenEvent.Add(task, default(ITreeTaskTokenEventRule));
                }
            }
            else
            {
                if (task.m_TreeTaskToken != null)
                {
                    awaiter.SetToken(task.m_TreeTaskToken);
                }
            }
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
            if (awaiter is TreeTaskCompleted)
            {
                //awaiter.SetCompleted();
            }
            World.Log($"[{task.Id}]TreeTask<{typeof(T)}> 等待 awaiter[{awaiter.Id}]{awaiter.Type}");
        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                World.Log($"TreeTask<{typeof(T)}> stateMachine.MoveNext");
            }
            else
            {
                World.Log($"[{task.Id}]TreeTask<{typeof(T)}> stateMachine.MoveNext");
            }

            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }


}
