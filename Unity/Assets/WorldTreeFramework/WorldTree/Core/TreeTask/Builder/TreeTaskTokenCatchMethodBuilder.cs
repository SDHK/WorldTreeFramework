
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
		private ITreeTaskStateMachine treeTaskStateMachine;

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
            task.SetCompleted();
			if (this.treeTaskStateMachine != null)
			{
				this.treeTaskStateMachine.Dispose();
				this.treeTaskStateMachine = null;
			}
		}

        // 5. AwaitOnCompleted
        [DebuggerHidden]

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
        }

        // 6. AwaitUnsafeOnCompleted
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
			if (treeTaskStateMachine == null)
			{
				this.treeTaskStateMachine = awaiter.PoolGet(out TreeTaskStateMachine<TStateMachine> taskStateMachine);
				taskStateMachine.SetStateMachine(ref stateMachine);
			}
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
                    task.m_TreeTaskToken.tokenEvent.Add(task, DefaultType<ITreeTaskTokenEventRule>.Default);
                }
                awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
            }
            else
            {
                if (task.m_TreeTaskToken != null)
                {
                    awaiter.SetToken(task.m_TreeTaskToken);
                }
                awaiter.UnsafeOnCompleted(treeTaskStateMachine.MoveNext);
            }
        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }
}
