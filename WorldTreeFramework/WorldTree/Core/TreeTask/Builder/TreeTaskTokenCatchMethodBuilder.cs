
/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/13 19:46

* 描述： 

*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace WorldTree.Internal
{
    public struct TreeTaskTokenCatchMethodBuilder
    {
        private TreeTaskTokenCatch task;

        [DebuggerHidden]
        public static TreeTaskTokenCatchMethodBuilder Create()
        {
            TreeTaskTokenCatchMethodBuilder builder = new TreeTaskTokenCatchMethodBuilder();
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public TreeTaskTokenCatch Task
        {
            get
            {
                World.Log($"[{task.Id}]TreeTaskTokenCatch Get");
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
            World.Log($"[{task.Id}]TreeTaskTokenCatch SetResult");
            task.SetCompleted();
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
            World.Log($"[{task.Id}]TreeTaskToKenCatch 等待 awaiter[{awaiter.Id}]{awaiter.Type}");
        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                World.Log($"TreeTaskToKenCatch stateMachine.MoveNext");
            }
            else
            {
                World.Log($"[{task.Id}]TreeTaskToKenCatch stateMachine.MoveNext");
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
