
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 异步任务完成类
* 
* 因为没有使用单例对象池，所以必须让构建器执行6号步骤。
* 
* 因此没法进行跳过处理，只能通过 Update 调用，延迟到下一帧执行。

*/

using System;
using System.Runtime.CompilerServices;

namespace WorldTree.Internal
{
    /// <summary>
    /// 异步任务完成类
    /// </summary>
    [AsyncMethodBuilder(typeof(AsyncTaskCompletedMethodBuilder))]
    public class AsyncTaskCompleted : AsyncTaskBase
    {
        public AsyncTaskCompleted GetAwaiter() => this;
        public override bool IsCompleted { get; set; }
        public Action SetResult { get; set; }

        public AsyncTaskCompleted() : base()
        {
            Type = typeof(AsyncTaskCompleted);
            SetResult = SetCompleted;
        }
        public void GetResult() { }
    }



    class AsyncTaskCompletedUpdateSystem : UpdateSystem<AsyncTaskCompleted>
    {
        public override void Update(AsyncTaskCompleted self, float deltaTime)
        {
            self.SetResult();
        }
    }

    class AsyncTaskCompletedEnableSystem : EnableSystem<AsyncTaskCompleted>
    {
        public override void OnEnable(AsyncTaskCompleted self)
        {
            self.Continue();
        }
    }

    class AsyncTaskCompletedRemoveSystem : RemoveSystem<AsyncTaskCompleted>
    {
        public override void OnRemove(AsyncTaskCompleted self)
        {
            self.IsCompleted = false;
            self.continuation = null;
        }
    }
}
