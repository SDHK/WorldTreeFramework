
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
		private ITreeTaskStateMachine treeTaskStateMachine;

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
			task.LogError(exception);
			//TreeTaskBase.ExceptionHandler?.Invoke(exception);
		}

        // 设置结果
        public void SetResult()
        {
            task.SetCompleted();
			if (this.treeTaskStateMachine != null)
			{
				this.treeTaskStateMachine.Dispose();
				this.treeTaskStateMachine = null;
			}

		}

        // 5. 等待完成
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
			AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
		}

		// 6. 等待不安全完成
		[DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : TreeTaskBase, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
			if (treeTaskStateMachine == null)
			{
				this.treeTaskStateMachine = awaiter.PoolGetUnit(out TreeTaskStateMachine<TStateMachine> taskStateMachine);
				taskStateMachine.SetStateMachine(ref stateMachine);
			}
			if (task == null)
            {
                task = awaiter.Parent.AddTemp(out task);

                if (awaiter.m_TreeTaskToken is null)
                {
                    task.m_RelevanceTask = awaiter;
                }
                else
                {
                    task.m_TreeTaskToken = awaiter.m_TreeTaskToken;
                    task.m_TreeTaskToken.tokenEvent.Add(task, TypeInfo<TreeTaskTokenEvent>.Default);
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
