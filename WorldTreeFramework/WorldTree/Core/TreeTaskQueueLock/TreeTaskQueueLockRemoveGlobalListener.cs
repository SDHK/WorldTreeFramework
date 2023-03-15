/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/7 15:05

* 描述： 任务队列锁 任务移除 的 全局监听器

*/

namespace WorldTree
{

    /// <summary>
    /// 任务队列锁 任务移除 的 全局静态监听器
    /// </summary>
    public class TreeTaskQueueLockRemoveGlobalListener : Node, ComponentOf<DynamicNodeQueue>
    {
        public DynamicNodeQueue nodeQueue;

    }

    class TreeTaskQueueLockRemoveGlobalListenerAddRule : AddRule<TreeTaskQueueLockRemoveGlobalListener>
    {
        public override void OnEvent(TreeTaskQueueLockRemoveGlobalListener self)
        {
            self.TryParentTo(out self.nodeQueue);
        }
    }

    class TreeTaskQueueLockRemoveGlobalListenerRemoveRule : RemoveRule<TreeTaskQueueLockRemoveGlobalListener>
    {
        public override void OnEvent(TreeTaskQueueLockRemoveGlobalListener self)
        {
            self.nodeQueue = null;
        }
    }

    class TreeTaskQueueLockRemoveGlobalListenerListenerRemoveRule : ListenerRemoveRule<TreeTaskQueueLockRemoveGlobalListener, TreeTask<TreeTaskQueueCompleter>, IRule>
    {
        public override void OnEvent(TreeTaskQueueLockRemoveGlobalListener self, TreeTask<TreeTaskQueueCompleter> node)
        {
            self.nodeQueue?.Remove(node);
        }
    }
}
