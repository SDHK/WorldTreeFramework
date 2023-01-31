/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 系统广播器
* 
* 用于广播拥有指定系统的实体

*/

namespace WorldTree
{
    /// <summary>
    /// 系统广播
    /// </summary>
    public partial class SystemBroadcast : Entity
    {
        /// <summary>
        /// 系统组
        /// </summary>
        public SystemGroup systems;

        public UnitQueue<long> updateQueue;
        public UnitDictionary<long, Entity> update1;
        public UnitDictionary<long, Entity> update2;

        public override string ToString()
        {
            return $"SystemBroadcast : {systems?.systemType}";
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        public void AddEntity(Entity entity)
        {
            if (update1.ContainsKey(entity.id) || update2.ContainsKey(entity.id)) return;

            updateQueue.Enqueue(entity.id);
            update2.Add(entity.id, entity);

        }

        /// <summary>
        /// 移除实体
        /// </summary>
        public void RemoveEntity(Entity entity)
        {
            update1.Remove(entity.id);
            update2.Remove(entity.id);
        }
        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            update1.Clear();
            update2.Clear();
            updateQueue.Clear();
        }
    }

    class SystemBroadcastAddSystem : AddSystem<SystemBroadcast>
    {
        public override void OnAdd(SystemBroadcast self)
        {
            self.updateQueue = self.PoolGet<UnitQueue<long>>();
            self.update1 = self.PoolGet<UnitDictionary<long, Entity>>();
            self.update2 = self.PoolGet<UnitDictionary<long, Entity>>();
        }
    }

    class SystemBroadcastRemoveSystem : RemoveSystem<SystemBroadcast>
    {
        public override void OnRemove(SystemBroadcast self)
        {
            self.updateQueue.Dispose();
            self.update1.Dispose();
            self.update2.Dispose();
        }
    }
}
