/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 16:21

* 描述： 动态实体队列
* 
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 动态实体队列
    /// </summary>
    public class DynamicEntityQueue : Entity
    {
        /// <summary>
        /// 实体id队列
        /// </summary>
        public UnitQueue<long> idQueue;

        /// <summary>
        /// 实体id被回收的次数
        /// </summary>
        public UnitDictionary<long, int> recycleId;

        /// <summary>
        /// 实体名单
        /// </summary>
        public UnitDictionary<long, Entity> entitys;


        /// <summary>
        /// 当前队列数量
        /// </summary>
        public int Conunt => idQueue.Count;

        /// <summary>
        /// 实体入列
        /// </summary>
        public void Enqueue(Entity entity)
        {
            if (entitys.ContainsKey(entity.id)) return;

            idQueue.Enqueue(entity.id);
            entitys.Add(entity.id, entity);
        }

        /// <summary>
        /// 实体移除
        /// </summary>
        public void Remove(Entity entity)
        {
            if (entitys.ContainsKey(entity.id))
            {
                entitys.Remove(entity.id);
                if (!recycleId.ContainsKey(entity.id))
                {
                    recycleId.Add(entity.id, 0);
                }
                else
                {
                    recycleId[entity.id] = recycleId[entity.id] + 1;
                }
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            entitys.Clear();
            recycleId.Clear();
            idQueue.Clear();
        }


        /// <summary>
        /// 实体出列
        /// </summary>
        public Entity Dequeue()
        {
            if (TryDequeue(out Entity entity))
            {
                return entity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 尝试出列
        /// </summary>
        public bool TryDequeue(out Entity entity)
        {
            //尝试获取一个id
            if (idQueue.TryDequeue(out long id))//假如队列不为0
            {
                //假如id被回收了
                while (recycleId.TryGetValue(id, out int count))
                {
                    //回收次数抵消
                    recycleId[id] = --count;

                    //次数为0时删除id
                    if (count == 0) recycleId.Remove(id);

                    //获取下一个id
                    id = idQueue.Dequeue();
                }
                //此时的id是正常id
                if (entitys.TryGetValue(id, out entity))
                {
                    entitys.Remove(entity.id);
                    return true;
                }
            }

            entity = null;
            return false;
        }
    }

    class DynamicEntityQueueAddSystem : AddSystem<DynamicEntityQueue>
    {
        public override void OnEvent(DynamicEntityQueue self)
        {
            self.PoolGet(out self.idQueue);
            self.PoolGet(out self.recycleId);
            self.PoolGet(out self.entitys);
        }
    }

    class DynamicEntityQueueRemoveSystem : RemoveSystem<DynamicEntityQueue>
    {
        public override void OnEvent(DynamicEntityQueue self)
        {
            self.idQueue.Dispose();
            self.recycleId.Dispose();
            self.entitys.Dispose();
        }
    }
}
