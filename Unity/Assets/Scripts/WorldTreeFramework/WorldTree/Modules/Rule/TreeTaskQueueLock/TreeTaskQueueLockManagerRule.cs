/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/25 21:09

* 描述： 

*/

namespace WorldTree
{
    public static partial class NodeRule
    {
        /// <summary>
        /// 异步队列锁
        /// </summary>
        /// <remarks>按照队列顺序执行异步任务</remarks>
        public static TreeTask<TreeTaskQueueCompleter> AsyncLock(this INode self, long key)
        {
            return self.Root.AddComponent(out TreeTaskQueueLockManager _).Lock(self, key);
        }
    }

    public static partial class TreeTaskQueueLockManagerRule
    {
        class RootAddRule : RootAddRule<TreeTaskQueueLockManager> { }


        class AddRule : AddRule<TreeTaskQueueLockManager>
        {
            protected override void OnEvent(TreeTaskQueueLockManager self)
            {
                self.AddComponent(out self.nodeQueueDictitonary);
            }
        }

        class RemoveRule : RemoveRule<TreeTaskQueueLockManager>
        {
            protected override void OnEvent(TreeTaskQueueLockManager self)
            {
                self.nodeQueueDictitonary = null;
            }
        }



        /// <summary>
        /// 队列任务锁
        /// </summary>
        public static async TreeTask<TreeTaskQueueCompleter> Lock(this TreeTaskQueueLockManager self, INode node, long key)
        {
            //判断如果没有这个键值
            if (!self.nodeQueueDictitonary.TryGetValue(key, out DynamicNodeQueue TaskQueue))
            {
                //新建动态节点队列
                self.nodeQueueDictitonary.AddChild(out TaskQueue);

                //动态节点队列添加进字典
                self.nodeQueueDictitonary.Add(key, TaskQueue);

                //节点添加解锁器
                node.AddChild(out TreeTaskQueueCompleter taskQueueCompleter, key, self);

                //防止异常
                await node.TreeTaskCompleted();

                //返回解锁器
                return taskQueueCompleter;
            }

            //节点添加任务锁
            _ = node.AddChild(out TreeTask<TreeTaskQueueCompleter> treeTask);
            TaskQueue.TryEnqueue(treeTask);

            //返回一个任务锁
            return await treeTask;
        }

        /// <summary>
        /// 启动下一个队列任务
        /// </summary>
        public static void RunNext(this TreeTaskQueueLockManager self, long key)
        {
            if (self.nodeQueueDictitonary != null)
                if (self.nodeQueueDictitonary.TryGetValue(key, out DynamicNodeQueue nodeQueue))
                {
                    //获取第一个任务
                    if (nodeQueue.TryDequeue(out var task))
                    {
                        TreeTaskQueueCompleter taskQueueCompleter;
                        if (nodeQueue.TryPeek(out var nextTask))
                        {
                            //给下一个任务父节点挂上任务解锁器
                            nextTask.Parent.AddChild(out taskQueueCompleter, key, self);
                        }
                        else
                        {
                            //下一个任务不存在，意味着已是最后一个，解锁器挂最后节点身上
                            task.Parent.AddChild(out taskQueueCompleter, key, self);
                        }

                        //启动下一个任务，塞入任务解锁器
                        task.To<TreeTask<TreeTaskQueueCompleter>>().SetResult(taskQueueCompleter);
                    }
                    //队列空了则删掉键值
                    if (nodeQueue.Count == 0)
                    {
                        self.nodeQueueDictitonary.Remove(key);
                        nodeQueue.Dispose();
                    }
                }
        }

    }
}
