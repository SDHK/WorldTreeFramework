/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/18 16:50

* 描述： 系统执行器
* 
* 用于执行拥有指定系统的实体,执行完毕后自动回收

*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 系统执行器
    /// </summary>
    public partial class SystemActuator : Entity
    {
        public SystemGroup systems;

        public UnitQueue<long> updateQueue;
        public UnitDictionary<long, Entity> update1;
        public UnitDictionary<long, Entity> update2;

        public void AddEntity(Entity entity)
        {
            if (systems != null)
                if (systems.ContainsKey(entity.Type))
                {
                    updateQueue.Enqueue(entity.id);
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

        public override string ToString()
        {
            return Type + ":" + systems.systemType;
        }
    }

    class SystemActuatorAwakeSystem : AwakeSystem<SystemActuator,Type>
    {
        public override void OnAwake(SystemActuator self,Type type)
        {
            self.Root.SystemManager.TryGetGroup(type, out self.systems);
        }
    }

    class SystemActuatorEntityRemoveSystem : EntityRemoveSystem<SystemActuator>
    {
        public override void OnEntityRemove(SystemActuator self, Entity entity)
        {
            self.RemoveEntity(entity);
        }
    }
    class SystemActuatorAddSystem : AddSystem<SystemActuator>
    {
        public override void OnAdd(SystemActuator self)
        {
            self.updateQueue = self.PoolGet<UnitQueue<long>>();
            self.update1 = self.PoolGet<UnitDictionary<long, Entity>>();
            self.update2 = self.PoolGet<UnitDictionary<long, Entity>>();
        }
    }
    class SystemActuatorRemoveSystem : RemoveSystem<SystemActuator>
    {
        public override void OnRemove(SystemActuator self)
        {
            self.updateQueue.Dispose();
            self.update1.Dispose();
            self.update2.Dispose();
        }
    }
}
