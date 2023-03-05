
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
        public EntityDictionary<long, EntityQueue<TreeTask<TreeTaskQueueLock>>> taskDictitonary;


        //self.AddChildren<TreeTask>()
        public async TreeTask<TreeTaskQueueLock> Add(Node entity, long key)
        {
            TreeTask<TreeTaskQueueLock> asyncTask = entity.AddChildren<TreeTask<TreeTaskQueueLock>>();
            asyncTask.AddChildren<TreeTaskQueueLock, long>(key);


            if (!taskDictitonary.Value.TryGetValue(key, out var TaskQueue))
            {
                return asyncTask.AddChildren<TreeTaskQueueLock, long>(key);
            }
            taskDictitonary.GetValueEntity(key).Value.Enqueue(asyncTask);
            return await asyncTask;//返回一个锁
        }

    }

    class AsyncTaskQueueAddSystem : AddSystem<TreeTaskQueue>
    {
        public override void OnEvent(TreeTaskQueue self)
        {
            self.AddChildren(out self.taskDictitonary);

        }
    }



}
