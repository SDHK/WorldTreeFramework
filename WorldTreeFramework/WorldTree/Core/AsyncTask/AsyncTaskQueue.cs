
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
* 
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 异步任务队列
    /// </summary>
    public class AsyncTaskQueue : Entity
    {
        public EntityDictionary<long, EntityQueue<AsyncTask<AsyncTaskQueueLock>>> taskDictitonary;


        //self.AddChildren<AsyncTask>()
        public async AsyncTask<AsyncTaskQueueLock> Add(Entity entity, long key)
        {
            AsyncTask<AsyncTaskQueueLock> asyncTask = entity.AddChildren<AsyncTask<AsyncTaskQueueLock>>();
            asyncTask.AddChildren<AsyncTaskQueueLock, long>(key);


            if (!taskDictitonary.Value.TryGetValue(key, out var TaskQueue))
            {
                return asyncTask.AddChildren<AsyncTaskQueueLock, long>(key);
            }
            taskDictitonary.GetValueEntity(key).Value.Enqueue(asyncTask);
            return await asyncTask;//返回一个锁
        }

    }

    class AsyncTaskQueueAddSystem : AddSystem<AsyncTaskQueue>
    {
        public override void OnEvent(AsyncTaskQueue self)
        {
            self.AddChildren(out self.taskDictitonary);

        }
    }



}
