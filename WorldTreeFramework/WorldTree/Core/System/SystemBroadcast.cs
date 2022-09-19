/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 系统全局广播器
* 
* 用于全局广播拥有指定系统的实体

*/

namespace WorldTree
{
    /// <summary>
    /// 系统全局广播
    /// </summary>
    public partial class SystemBroadcast : Entity
    {

        public SystemGroup systems;

        public UnitDictionary<long, Entity> update1 = new UnitDictionary<long, Entity>();
        public UnitDictionary<long, Entity> update2 = new UnitDictionary<long, Entity>();
        public override string ToString()
        {
            return Type + ":" + systems.systemType;
        }
    }

    class SystemBroadcastEntityAddSystem : EntityAddSystem<SystemBroadcast>
    {
        public override void OnEntityAdd(SystemBroadcast self, Entity entity)
        {
            if (self.systems.ContainsKey(entity.Type))
            {
                self.update2.Add(entity.id, entity);
            }
        }
    }
    class SystemBroadcastEntityRemoveSystem : EntityRemoveSystem<SystemBroadcast>
    {
        public override void OnEntityRemove(SystemBroadcast self, Entity entity)
        {
            if (self.systems.ContainsKey(entity.Type))
            {
                self.update1.Remove(entity.id);
                self.update2.Remove(entity.id);
            }
        }
    }

    class SystemBroadcastSendSystem : SendSystem<SystemBroadcast, SystemGroup>
    {
        public override void Event(SystemBroadcast self, SystemGroup arg1)
        {
            self.systems = arg1;
            foreach (var entity in self.Root.allEntity.Values)
            {
                if (arg1.ContainsKey(entity.Type))
                {
                    self.update2.Add(entity.id, entity);
                }
            }
        }
    }


}
