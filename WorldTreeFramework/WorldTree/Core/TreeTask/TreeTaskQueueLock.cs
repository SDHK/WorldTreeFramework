/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 14:06

* 描述： 

*/

namespace WorldTree
{
    /// <summary>
    /// 异步任务队列锁
    /// </summary>
    public class TreeTaskQueueLock : Node
    {
        public DynamicNodeQueue nodeQueue;
    }


    class TreeTaskQueueLockRemoveRule : RemoveRule<TreeTaskQueueLock>
    {
        public override void OnEvent(TreeTaskQueueLock self)
        {
            //当前任务完成，此时锁正在被移除
            var task = self.ParentTo<TreeTask<TreeTaskQueueLock>>();

           

            //设置任务完成，返回下一个任务的锁
            World.Log("nodeQueue:"+self.nodeQueue.Count);
            task.SetResult(self.nodeQueue.Peek().GetComponent<TreeTaskQueueLock>());

            //节点队列移除任务
            self.nodeQueue.Remove(task);

            self.nodeQueue = null;
        }
    }

}
