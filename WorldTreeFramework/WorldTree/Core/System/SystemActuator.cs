/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/18 16:50

* 描述： 系统执行器
* 
* 用于执行拥有指定系统的实体,执行完毕后自动回收

*/

namespace WorldTree
{

    /// <summary>
    /// 系统执行器
    /// </summary>
    public partial class SystemActuator<T> : SystemActuator
        where T : ISystem
    {
        SystemActuator()
        {
            Type = typeof(SystemActuator);
            GenericType = typeof(T);
        }
    }

    /// <summary>
    /// 系统执行器
    /// </summary>
    public partial class SystemActuator : Entity
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

        public override string ToString()
        {
            return Type + ":" + systems.systemType;
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
            self.update1 = self.PoolGet<UnitDictionary<long, Entity>>();
            self.update2 = self.PoolGet<UnitDictionary<long, Entity>>();
        }
    }
    class SystemActuatorRemoveSystem : RemoveSystem<SystemActuator>
    {
        public override void OnRemove(SystemActuator self)
        {
            self.update1.Dispose();
            self.update2.Dispose();
        }
    }
}
