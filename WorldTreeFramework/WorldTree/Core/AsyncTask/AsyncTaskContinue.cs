/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/13 20:35

* 描述： 异步任务继续执行器
* 
* 默认父节点是已停止的异步任务，当被激活时则让异步任务继续运行

*/

namespace WorldTree
{
    /// <summary>
    /// 异步任务继续执行器
    /// </summary>
    public class AsyncTaskContinue : Entity { }

    class AsyncTaskContinueEnableSystem : EnableSystem<AsyncTaskContinue>
    {
        public override void OnEnable(AsyncTaskContinue self)
        {
            if (self.TryGetParent(out AsyncTaskBase Parent))
            {
                Parent.Continue();
            }
        }
    }

}
