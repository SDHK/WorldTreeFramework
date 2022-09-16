using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public partial class SystemRadio : Entity
    {
        public SystemGroup systems;

        public UnitDictionary<long, Entity> update1 = new UnitDictionary<long, Entity>();
        public UnitDictionary<long, Entity> update2 = new UnitDictionary<long, Entity>();

        //扩展的参考
        public void Send()
        {
            (update1, update2) = (update2, update1);

            while (update1.Count != 0 && IsActice)
            {
                long firstKey = update1.Keys.First();
                Entity entity = update1[firstKey];
                if (entity.IsActice)
                {
                    systems.SendSystem<IUpdateSystem, float>(entity, 0.2f);
                }
                update1.Remove(firstKey);
                if (!entity.IsRecycle)
                {
                    update2.Add(firstKey, entity);
                }
            }
        }

    }
    class SystemRadioEntityAddSystem : EntityAddSystem<SystemRadio>
    {
        public override void OnEntityAdd(SystemRadio self, Entity entity)
        {
            if (self.systems.ContainsKey(entity.Type))
            {
                self.update2.Add(entity.id, entity);
            }
        }
    }
    class SystemRadioEntityRemoveSystem : EntityRemoveSystem<SystemRadio>
    {
        public override void OnEntityRemove(SystemRadio self, Entity entity)
        {
            if (self.systems.ContainsKey(entity.Type))
            {
                self.update1.Remove(entity.id);
                self.update2.Remove(entity.id);
            }
        }
    }

    class SystemRadioSendSystem : SendSystem<SystemRadio, ISendSystem<SystemGroup>, SystemGroup>
    {
        public override void Event(SystemRadio self, SystemGroup arg1)
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
