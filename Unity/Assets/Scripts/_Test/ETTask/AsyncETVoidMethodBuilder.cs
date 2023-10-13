using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace ET
{
    internal struct AsyncETVoidMethodBuilder
    {
        private IStateMachineWrap iStateMachineWarp;

        // 1. Static Create method.
        [DebuggerHidden]
        public static AsyncETVoidMethodBuilder Create()
        {
            AsyncETVoidMethodBuilder builder = new();
            return builder;
        }

        // 2. TaskLike Task property(void)
        [DebuggerHidden]
        public ETVoid Task => default;

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception e)
        {
            if (this.iStateMachineWarp != null)
            {
                this.iStateMachineWarp.Recycle();
                this.iStateMachineWarp = null;
            }
            ETTask.ExceptionHandler.Invoke(e);
        }

        // 4. SetResult
        [DebuggerHidden]
        public void SetResult()
        {
            if (this.iStateMachineWarp != null)
            {
                this.iStateMachineWarp.Recycle();
                this.iStateMachineWarp = null;
            }
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            this.iStateMachineWarp ??= StateMachineWrap<TStateMachine>.Fetch(ref stateMachine);
            awaiter.OnCompleted(this.iStateMachineWarp.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            this.iStateMachineWarp ??= StateMachineWrap<TStateMachine>.Fetch(ref stateMachine);
            awaiter.UnsafeOnCompleted(this.iStateMachineWarp.MoveNext);
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