using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public class SystemRadio : Entity
    {
        public SystemGroup systems;

        public UnitDictionary<long, Entity> update1 = new UnitDictionary<long, Entity>();
        public UnitDictionary<long, Entity> update2 = new UnitDictionary<long, Entity>();


        public void Update()
        {

            (update1, update2) = (update2, update1);

            while (update1.Count != 0 && IsActice)
            {
                long firstKey = update1.Keys.First();
                Entity entity = update1[firstKey];
                if (entity.IsActice)
                {
                    systems.SendSystem<IUpdateSystem,float>(entity,0.2f);
                }
                update1.Remove(firstKey);
                if (!entity.IsRecycle)
                {
                    update2.Add(firstKey, entity);
                }
            }
        }



    }
}
