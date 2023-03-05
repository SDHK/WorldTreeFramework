/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 16:21

* 描述： 动态实体队列

*/

namespace WorldTree
{
    /// <summary>
    /// 动态实体队列
    /// </summary>
    public class DynamicEntityQueue : Node
    {
        /// <summary>
        /// 实体id队列
        /// </summary>
        public UnitQueue<long> idQueue;

        /// <summary>
        /// 实体id被移除的次数
        /// </summary>
        public UnitDictionary<long, int> removeId;

        /// <summary>
        /// 实体名单
        /// </summary>
        public UnitDictionary<long, Node> entitys;


        /// <summary>
        /// 当前队列数量
        /// </summary>
        public int Count => idQueue.Count;

        /// <summary>
        /// 实体入列
        /// </summary>
        public void Enqueue(Node entity)
        {
            if (entitys.ContainsKey(entity.id)) return;

            idQueue.Enqueue(entity.id);
            entitys.Add(entity.id, entity);
        }

        /// <summary>
        /// 实体移除
        /// </summary>
        public void Remove(Node entity)
        {
            if (entitys.ContainsKey(entity.id))
            {
                entitys.Remove(entity.id);

                //累计强制移除的实体id
                if (removeId.TryGetValue(entity.id, out var count))
                {
                    removeId[entity.id] = count + 1;
                }
                else
                {
                    removeId.Add(entity.id, 1);
                }
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            entitys.Clear();
            removeId.Clear();
            idQueue.Clear();
        }


        /// <summary>
        /// 实体出列
        /// </summary>
        public Node Dequeue()
        {
            if (TryDequeue(out Node entity))
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
        public bool TryDequeue(out Node entity)
        {
            //尝试获取一个id
            if (idQueue.TryDequeue(out long id))
            {
                //假如id被回收了
                while (removeId.TryGetValue(id, out int count))
                {
                    //回收次数抵消
                    removeId[id] = --count;

                    //次数为0时删除id
                    if (count == 0) removeId.Remove(id);

                    //获取下一个id
                    if (!idQueue.TryDequeue(out id))
                    {
                        //假如队列空了,则直接返回退出
                        entity = null;
                        return false;
                    }
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
            self.PoolGet(out self.removeId);
            self.PoolGet(out self.entitys);
        }
    }

    class DynamicEntityQueueRemoveSystem : RemoveSystem<DynamicEntityQueue>
    {
        public override void OnEvent(DynamicEntityQueue self)
        {
            self.idQueue.Dispose();
            self.removeId.Dispose();
            self.entitys.Dispose();
        }
    }
}
