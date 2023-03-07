/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 14:06

* 描述： 异步任务队列锁解锁器

*/

namespace WorldTree
{
    /// <summary>
    /// 异步任务队列锁解锁器
    /// </summary>
    public class TreeTaskQueueCompleter : Node
    {
        public TreeTaskQueueLock treeTaskQueue;
        public long key;
    }

    class TreeTaskQueueCompleterRemoveRule : RemoveRule<TreeTaskQueueCompleter>
    {
        public override void OnEvent(TreeTaskQueueCompleter self)
        {
            self.treeTaskQueue.RunNext(self.key);
        }
    }

}
