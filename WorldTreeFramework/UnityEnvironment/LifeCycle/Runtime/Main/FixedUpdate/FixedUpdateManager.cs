using System.Collections.Generic;
using System.Linq;

namespace WorldTree
{
    /// <summary>
    /// FixedUpdate生命周期管理器实体
    /// </summary>
    public class FixedUpdateManager : Entity
    {
        public float deltaTime = 0.02f;
        public UnitDictionary<long, Entity> update1 = new UnitDictionary<long, Entity>();
        public UnitDictionary<long, Entity> update2 = new UnitDictionary<long, Entity>();
        public SystemGroup systems;

        public void Update()
        {
            while (update1.Count != 0 && IsActice)
            {
                long firstKey = update1.Keys.First();
                Entity entity = update1[firstKey];
                if (entity.IsActice)
                {
                    if (systems.TryGetValue(entity.Type, out List<ISystem> systemList))
                    {
                        foreach (IFixedUpdateSystem system in systemList)
                        {
                            system.Execute(entity, deltaTime);
                        }
                    }
                }
                update1.Remove(firstKey);
                if (!entity.IsRecycle)
                {
                    update2.Add(firstKey, entity);
                }

            }
            (update1, update2) = (update2, update1);
        }
    }

    class FixedUpdateManagerUpdateSystem : FixedUpdateSystem<FixedUpdateManager>
    {
        public override void FixedUpdate(FixedUpdateManager self, float deltaTime)
        {
            self.deltaTime = deltaTime;
            self.Update();
        }
    }
    class FixedUpdateManagerNewSystem : NewSystem<FixedUpdateManager>
    {
        public override void OnNew(FixedUpdateManager self)
        {
            self.systems = self.RootGetSystemGroup<IFixedUpdateSystem>();
        }
    }

    class FixedUpdateManagerAddSystem : AddSystem<FixedUpdateManager>
    {
        public override void OnAdd(FixedUpdateManager self)
        {
            if (self.Root.Parent != null)
            {
                var ParentUpdate = self.Root.Parent.Root.GetComponent<FixedUpdateManager>();//父节点
                ParentUpdate.update2.Add(self.id, self);
            }
        }
    }

    class FixedUpdateManagerRemoveSystem : RemoveSystem<FixedUpdateManager>
    {
        public override void OnRemove(FixedUpdateManager self)
        {
            if (self.Root.Parent != null)
            {
                var ParentUpdate = self.Root.Parent.Root.GetComponent<FixedUpdateManager>();//父节点
                ParentUpdate.update1.Remove(self.id);
                ParentUpdate.update2.Remove(self.id);
            }
        }
    }

    class FixedUpdateManagerDestroySystem : DestroySystem<FixedUpdateManager>
    {
        public override void OnDestroy(FixedUpdateManager self)
        {
            self.systems = null;
        }
    }

    class FixedUpdateManagerEntitySystem : EntitySystem<FixedUpdateManager>
    {
        public override void OnAddEntity(FixedUpdateManager self, Entity entity)
        {
            if (self.systems.ContainsKey(entity.Type))
            {
                self.update2.Add(entity.id, entity);
            }
        }

        public override void OnRemoveEntity(FixedUpdateManager self, Entity entity)
        {
            if (self.systems.ContainsKey(entity.Type))
            {
                self.update1.Remove(entity.id);
                self.update2.Remove(entity.id);
            }
        }
    }
}
