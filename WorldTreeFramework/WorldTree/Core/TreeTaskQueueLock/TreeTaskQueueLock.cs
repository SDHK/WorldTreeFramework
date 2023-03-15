
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:21

* 描述： 异步任务队列锁
* 
*/

namespace WorldTree
{
    public static partial class NodeStaticRule
    {
        /// <summary>
        /// 异步队列锁
        /// </summary>
        /// <remarks>按照队列顺序执行异步任务</remarks>
        public static TreeTask<TreeTaskQueueCompleter> AsyncLock(this INode self, long key)
        {
            return self.Root.AddComponent<TreeTaskQueueLock>().Lock(self, key);
        }
    }

    /// <summary>
    /// 异步任务队列锁
    /// </summary>
    public class TreeTaskQueueLock : Node
    {
        public TreeDictionary<long, DynamicNodeQueue> nodeQueueDictitonary;

        /// <summary>
        /// 队列任务锁
        /// </summary>
        public async TreeTask<TreeTaskQueueCompleter> Lock(INode node, long key)
        {
            //判断如果没有这个键值
            if (!nodeQueueDictitonary.TryGetValue(key, out DynamicNodeQueue TaskQueue))
            {
                //新建动态节点队列
                TaskQueue = nodeQueueDictitonary.AddChildren<DynamicNodeQueue>();

                //添加 任务队列锁 任务移除 的 全局监听器。
                TaskQueue.AddComponent<TreeTaskQueueLockRemoveGlobalListener>();

                //动态节点队列添加进字典
                nodeQueueDictitonary.Add(key, TaskQueue);

                //节点添加解锁器
                TreeTaskQueueCompleter taskQueueCompleter = node.AddChildren<TreeTaskQueueCompleter>();

                //解锁器配上队列
                taskQueueCompleter.queueLock = this;
                taskQueueCompleter.key = key;

                //防止异常
                await node.TreeTaskCompleted();

                //返回解锁器
                return taskQueueCompleter;
            }

            //节点添加任务锁
            TreeTask<TreeTaskQueueCompleter> treeTask = node.AddChildren<TreeTask<TreeTaskQueueCompleter>>();
            TaskQueue.Enqueue(treeTask);

            //返回一个任务锁
            return await treeTask;
        }

        /// <summary>
        /// 启动下一个队列任务
        /// </summary>
        public void RunNext(long key)
        {
            if (nodeQueueDictitonary.TryGetValue(key, out DynamicNodeQueue nodeQueue))
            {
                //获取第一个任务
                if (nodeQueue.TryDequeue(out var task))
                {
                    TreeTaskQueueCompleter taskQueueCompleter;
                    if (nodeQueue.TryPeek(out var nextTask))
                    {
                        //给下一个任务父节点挂上任务解锁器
                        taskQueueCompleter = nextTask.Parent.AddChildren<TreeTaskQueueCompleter>();
                    }
                    else
                    {
                        //下一个任务不存在，意味着已是最后一个，解锁器挂最后节点身上
                        taskQueueCompleter = task.Parent.AddChildren<TreeTaskQueueCompleter>();
                    }
                    //解锁器参数传递
                    taskQueueCompleter.queueLock = this;
                    taskQueueCompleter.key = key;

                    //启动下一个任务，塞入任务解锁器
                    task.To<TreeTask<TreeTaskQueueCompleter>>().SetResult(taskQueueCompleter);
                }
                //队列空了则删掉键值
                if (nodeQueue.Count == 0)
                {
                    nodeQueueDictitonary.Remove(key);
                    nodeQueue.Dispose();
                }
            }
        }

    }

    class TreeTaskQueueLockAddRule : AddRule<TreeTaskQueueLock>
    {
        public override void OnEvent(TreeTaskQueueLock self)
        {
            self.AddComponent(out self.nodeQueueDictitonary);
        }
    }

    class TreeTaskQueueLockRemoveRule : RemoveRule<TreeTaskQueueLock>
    {
        public override void OnEvent(TreeTaskQueueLock self)
        {
            self.nodeQueueDictitonary = null;
        }
    }
}
