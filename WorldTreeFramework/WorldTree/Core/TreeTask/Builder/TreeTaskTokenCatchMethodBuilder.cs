
/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/13 19:46

* 描述： 

*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree.Internal
{
    public struct TreeTaskTokenCatchMethodBuilder
    {
        private TreeTaskTokenCatch task;

        [DebuggerHidden]
        public static TreeTaskTokenCatchMethodBuilder Create()
        {
            TreeTaskTokenCatchMethodBuilder builder = new TreeTaskTokenCatchMethodBuilder();
            World.Log($"TreeTaskTokenCatch ！！！！！！静态构建方法！！！！！！！");
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public TreeTaskTokenCatch Task
        {
            get
            {
                World.Log($"TreeTaskTokenCatch 获取Task");
                return task;
            }
        }

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            World.Log($"[{task.Id}]TreeTaskTokenCatch 设置异常 {exception}");
            task.SetException(exception);
        }

        // 4. SetResult
        public void SetResult()
        {
            World.Log($"[{task.Id}]TreeTaskTokenCatch 设置结果");
            task.SetCompleted();
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
                World.Log($"({awaiter.treeTaskToken != null})（{stateMachine.GetType()}）传入 awaiter [{awaiter.Id}] =>  新建 Task [{task.Id}] 6. 等待不安全完成");

                if (awaiter.treeTaskToken is null)
                {
                    awaiter.relevanceTask = task;
                }
                else
                {
                    task.treeTaskToken = awaiter.treeTaskToken;
                }
            }
            else
            {
                World.Log($"({task.treeTaskToken != null})（{stateMachine.GetType()}）已经存在 Task [{task.Id}] 移动到 => ({awaiter.treeTaskToken != null}) awaiter [{awaiter.Id}] 6. 等待不安全完成！！！！");
                awaiter.treeTaskToken = task.treeTaskToken;

            }
            awaiter.OnCompleted(stateMachine.MoveNext);



            World.Log($"TreeTaskTokenCatch 6. 等待不安全完成后");

        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            World.Log($"TreeTaskTokenCatch 7. 开始前");
            stateMachine.MoveNext();
            World.Log($"TreeTaskTokenCatch 7. 开始后");
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            World.Log($"TreeTaskTokenCatch 8. 设置状态机");
        }
    }
}
