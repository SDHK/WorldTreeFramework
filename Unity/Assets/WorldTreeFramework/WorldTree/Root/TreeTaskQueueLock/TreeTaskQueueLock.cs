
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
            return self.Root.AddComponent(out TreeTaskQueueLock _).Lock(self, key);
        }
    }


    public class TreeTaskQueueLockRootAddRule : RootAddRule<TreeTaskQueueLock> { }

    /// <summary>
    /// 异步任务队列锁
    /// </summary>
    public class TreeTaskQueueLock : Node, ComponentOf<WorldTreeRoot>
        , AsRule<IAwakeRule>
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
                nodeQueueDictitonary.AddChild(out TaskQueue);

                //动态节点队列添加进字典
                nodeQueueDictitonary.Add(key, TaskQueue);

                //节点添加解锁器
                node.AddChild(out TreeTaskQueueCompleter taskQueueCompleter, key, this);

                //防止异常
                await node.TreeTaskCompleted();

                //返回解锁器
                return taskQueueCompleter;
            }

            //节点添加任务锁
            _ = node.AddChild(out TreeTask<TreeTaskQueueCompleter> treeTask);
            TaskQueue.EnqueueReferenced(treeTask);

            //返回一个任务锁
            return await treeTask;
        }

        /// <summary>
        /// 启动下一个队列任务
        /// </summary>
        public void RunNext(long key)
        {
            if (nodeQueueDictitonary != null)
                if (nodeQueueDictitonary.TryGetValue(key, out DynamicNodeQueue nodeQueue))
                {
                    //获取第一个任务
                    if (nodeQueue.TryDequeue(out var task))
                    {
                        TreeTaskQueueCompleter taskQueueCompleter;
                        if (nodeQueue.TryPeek(out var nextTask))
                        {
                            //给下一个任务父节点挂上任务解锁器
                            nextTask.Parent.AddChild(out taskQueueCompleter, key, this);
                        }
                        else
                        {
                            //下一个任务不存在，意味着已是最后一个，解锁器挂最后节点身上
                            task.Parent.AddChild(out taskQueueCompleter, key, this);
                        }

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
