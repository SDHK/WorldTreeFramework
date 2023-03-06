
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
        public EntityDictionary<long, DynamicNodeQueue> taskDictitonary;

        public async TreeTask<TreeTaskQueueLock> Add(Node entity, long key)
        {
            TreeTask<TreeTaskQueueLock> asyncTask = entity.AddChildren<TreeTask<TreeTaskQueueLock>>();
            asyncTask.AddChildren<TreeTaskQueueLock, long>(key);


            if (!taskDictitonary.Value.TryGetValue(key, out var TaskQueue))
            {
                var nodeQueue = taskDictitonary.AddChildren<DynamicNodeQueue>();
                taskDictitonary.Value.Add(key, nodeQueue);

                var taskQueueLock = asyncTask.AddChildren<TreeTaskQueueLock, long>(key);
                nodeQueue.Enqueue(asyncTask);
                return taskQueueLock;
            }
            taskDictitonary.GetValueEntity(key).Enqueue(asyncTask);
            return await asyncTask;//返回一个锁
        }

    }

    class AsyncTaskQueueAddRule : AddRule<TreeTaskQueue>
    {
        public override void OnEvent(TreeTaskQueue self)
        {
            self.AddChildren(out self.taskDictitonary);

        }
    }



}
