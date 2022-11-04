/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/18 16:50

* 描述： 系统执行器

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    /// <summary>
    /// 系统执行器
    /// </summary>
    public partial class SystemActuator : Entity
    {
        public SystemGroup systems;

        public UnitDictionary<long, Entity> update1;
        public UnitDictionary<long, Entity> update2;

        public void Add(Entity entity)
        {
            if (systems.ContainsKey(entity.Type))
            {
                update2.Add(entity.id, entity);
            }
        }

        public void Remove(Entity entity)
        {

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
            self.update1.Clear();
            self.update2.Clear();
            self.update1.Dispose();
            self.update2.Dispose();
        }
    }

    class SystemActuatorSendSystem : SendSystem<SystemActuator, SystemGroup>
    {
        public override void Event(SystemActuator self, SystemGroup arg1)
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
