/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/25 21:08

* 描述： 

*/

namespace WorldTree
{
    public static partial class TreeTaskQueueCompleterRule
    {
        class AwakeRule : AwakeRule<TreeTaskQueueCompleter, long, TreeTaskQueueLockManager>
        {
            public override void OnEvent(TreeTaskQueueCompleter self, long key, TreeTaskQueueLockManager queueLock)
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
