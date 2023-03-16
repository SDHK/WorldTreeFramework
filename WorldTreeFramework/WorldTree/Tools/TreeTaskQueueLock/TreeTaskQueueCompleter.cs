/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 14:06

* 描述： 异步任务队列锁的解锁器

*/

namespace WorldTree
{
    /// <summary>
    /// 异步任务队列锁的解锁器
    /// </summary>
    public class TreeTaskQueueCompleter : Node,ChildOfNode
    {
        public TreeTaskQueueLock queueLock;
        public long key;
    }

    class TreeTaskQueueCompleterRemoveRule : RemoveRule<TreeTaskQueueCompleter>
    {
        public override void OnEvent(TreeTaskQueueCompleter self)
        {
            self.queueLock.RunNext(self.key);
        }
    }

}
