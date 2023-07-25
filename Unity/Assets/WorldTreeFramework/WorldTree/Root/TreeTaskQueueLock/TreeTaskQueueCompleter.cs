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

    public static partial class TreeTaskQueueCompleterRule
    {
        class AwakeRule : AwakeRule<TreeTaskQueueCompleter, long, TreeTaskQueueLock>
        {
            public override void OnEvent(TreeTaskQueueCompleter self, long key, TreeTaskQueueLock queueLock)
            {
                self.key = key;
                self.m_QueueLock = queueLock;
            }
        }

        class RemoveRule : RemoveRule<TreeTaskQueueCompleter>
        {
            public override void OnEvent(TreeTaskQueueCompleter self)
            {
                self.m_QueueLock.RunNext(self.key);
                self.m_QueueLock = default;
                self.key = default;
            }
        }
    }


}
