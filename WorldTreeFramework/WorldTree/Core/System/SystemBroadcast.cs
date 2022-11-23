/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 系统广播器
* 
* 用于广播拥有指定系统的实体

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 系统广播
    /// </summary>
    public partial class SystemBroadcast : Entity
    {

        public SystemGroup systems;

        public UnitDictionary<long, Entity> update1;
        public UnitDictionary<long, Entity> update2;

        public void AddEntity(Entity entity)
        {
            if (systems != null)
                if (systems.ContainsKey(entity.Type))
                {
                    update2.Add(entity.id, entity);
                }
        }

        public void RemoveEntity(Entity entity)
        {
            if (systems != null)
                if (systems.ContainsKey(entity.Type))
                {
                    update1.Remove(entity.id);
                    update2.Remove(entity.id);
                }
        }

    }

    class SystemBroadcastAwakeSystem : AwakeSystem<SystemBroadcast,Type>
    {
        public override void OnAwake(SystemBroadcast self,Type type)
        {
            self.Root.SystemManager.TryGetGroup(type, out self.systems);
        }
    }

    class SystemBroadcastEntityRemoveSystem : EntityRemoveSystem<SystemBroadcast>
    {
        public override void OnEntityRemove(SystemBroadcast self, Entity entity)
        {
            self.RemoveEntity(entity);
        }
    }


    class SystemBroadcastAddSystem : AddSystem<SystemBroadcast>
    {
        public override void OnAdd(SystemBroadcast self)
        {
            self.update1 = self.PoolGet<UnitDictionary<long, Entity>>();
            self.update2 = self.PoolGet<UnitDictionary<long, Entity>>();
        }
    }

    class SystemBroadcastRemoveSystem : RemoveSystem<SystemBroadcast>
    {
        public override void OnRemove(SystemBroadcast self)
        {
            self.update1.Dispose();
            self.update2.Dispose();
        }
    }
}
