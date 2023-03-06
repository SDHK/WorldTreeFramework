
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:21

* 描述： 
* 
* 队列在最高层或属于实体
* 
* 异步属于实体，锁属于异步
* 
* 锁存队列
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 异步任务队列
    /// </summary>
    public class TreeTaskQueue : Node
    {
        public EntityDictionary<long, DynamicNodeQueue> nodeQueueDictitonary;

        public async TreeTask<TreeTaskQueueLock> Add(Node entity, long key)
        {
            TreeTask<TreeTaskQueueLock> treeTask = entity.AddChildren<TreeTask<TreeTaskQueueLock>>();
            treeTask.AddChildren<TreeTaskQueueLock, long>(key);

            //给任务挂上队列锁
            TreeTaskQueueLock taskQueueLock = treeTask.AddChildren<TreeTaskQueueLock>();

            //判断如果没有这个键值
            if (!nodeQueueDictitonary.Value.TryGetValue(key, out DynamicNodeQueue TaskQueue))
            {
                //新建动态节点队列
                TaskQueue = nodeQueueDictitonary.AddChildren<DynamicNodeQueue>();

                //动态节点队列添加进字典
                nodeQueueDictitonary.Value.Add(key, TaskQueue);

                TaskQueue.Enqueue(treeTask);
                taskQueueLock.nodeQueue = TaskQueue;

                await entity.TreeTaskCompleted();

                return taskQueueLock;
            }
            else
            {
                TaskQueue.Enqueue(treeTask);
                taskQueueLock.nodeQueue = TaskQueue;
            }

            return await treeTask;//返回一个锁任务
        }

    }

    class AsyncTaskQueueAddRule : AddRule<TreeTaskQueue>
    {
        public override void OnEvent(TreeTaskQueue self)
        {
            self.AddChildren(out self.nodeQueueDictitonary);

        }
    }



}
