using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;


namespace WorldTree
{
    public struct AsyncTaskMethodBuilder
    {
        private AsyncTask task;
        // 1. Static Create method.

        [DebuggerHidden]
        public static AsyncTaskMethodBuilder Create()
        {
            AsyncTaskMethodBuilder builder = new AsyncTaskMethodBuilder();
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public AsyncTask Task
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
        [DebuggerHidden]

        public void SetResult()
        {
            task.SetResult();
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : Entity, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                task = awaiter.Parent.AddChildren<AsyncTask>();
            }
            awaiter.OnCompleted(stateMachine.MoveNext);
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



    public struct AsyncTaskMethodBuilder<T>
    {
        private AsyncTask<T> task;
        // 1. Static Create method.

        [DebuggerHidden]
        public static AsyncTaskMethodBuilder<T> Create()
        {
            AsyncTaskMethodBuilder<T> builder = new AsyncTaskMethodBuilder<T>();
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public AsyncTask<T> Task
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
        [DebuggerHidden]

        public void SetResult(T ret)
        {
            task.SetResult(ret);
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : Entity, ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (task == null)
            {
                task = awaiter.Parent.AddChildren<AsyncTask<T>>();
            }
            awaiter.OnCompleted(stateMachine.MoveNext);
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
