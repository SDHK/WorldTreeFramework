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
    public class TreeTaskQueueCompleter : Node, ChildOf<INode>
        , AsRule<IAwakeRule<long, TreeTaskQueueLock>>
    {
        public TreeTaskQueueLock m_QueueLock;
        public long key;
    }

    public class TreeTaskQueueCompleterAwakeRule : AwakeRule<TreeTaskQueueCompleter, long, TreeTaskQueueLock>
    {
        public override void OnEvent(TreeTaskQueueCompleter self, long arg1, TreeTaskQueueLock arg2)
        {
            self.key = arg1;
            self.m_QueueLock = arg2;
        }
    }

    class TreeTaskQueueCompleterRemoveRule : RemoveRule<TreeTaskQueueCompleter>
    {
        public override void OnEvent(TreeTaskQueueCompleter self)
        {
            self.m_QueueLock.RunNext(self.key);
            self.m_QueueLock = default;
            self.key = default;
        }
    }

}
