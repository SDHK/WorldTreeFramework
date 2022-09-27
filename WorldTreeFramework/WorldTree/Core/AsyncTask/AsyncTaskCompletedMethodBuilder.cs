using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace WorldTree
{
    public struct AsyncTaskCompletedMethodBuilder
    {

        private AsyncTaskCompleted task;

        // 1. Static Create method.
        [DebuggerHidden]
        public static AsyncTaskCompletedMethodBuilder Create()
        {
            AsyncTaskCompletedMethodBuilder builder = new AsyncTaskCompletedMethodBuilder();
            return builder;
        }

        public AsyncTaskCompleted Task
        {
            get
            {
                return task;
            }
        }
        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception e)
        {
        }

        // 4. SetResult
        [DebuggerHidden]
        public void SetResult()
        {
            // do nothing
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : Entity, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                task = awaiter.Parent.AddChildren<AsyncTaskCompleted>();
            }
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
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
