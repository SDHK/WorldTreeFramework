using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public class EntityBox<V> : EntityBox
    where V : class
    {
        public V Value { get => value as V; }
        EntityBox()
        {
            Type = typeof(EntityBox);
            GenericType = typeof(V);
        }
    }


    public class EntityBox : Entity
    {
        public object value;
    }

    class EntityObjectAddSystem : AddSystem<EntityBox>
    {
        public override void OnAdd(EntityBox self)
        {
            self.value = self.PoolGet(self.GenericType);
        }
    }

    class EntityObjectRemoveSystem : RemoveSystem<EntityBox>
    {
        public override void OnRemove(EntityBox self)
        {
            self.PoolRecycle(self.value);
            self.value = null;
        }
    }


}
