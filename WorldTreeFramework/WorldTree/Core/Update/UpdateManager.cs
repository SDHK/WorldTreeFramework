using System.Collections.Generic;
using System.Linq;

namespace WorldTree
{

    /// <summary>
    /// Update生命周期管理器实体 
    /// </summary>
    public class UpdateManager : Entity
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
                        foreach (IUpdateSystem system in systemList)
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
    class UpdateManagerUpdateSystem : UpdateSystem<UpdateManager>
    {
        public override void Update(UpdateManager self, float deltaTime)
        {
            self.deltaTime = deltaTime;
            self.Update();
        }
    }

    class UpdateManagerNewSystem : NewSystem<UpdateManager>
    {
        public override void OnNew(UpdateManager self)
        {
            self.systems = self.RootGetSystemGroup<IUpdateSystem>();
        }
    }

    class UpdateManagerAddSystem : AddSystem<UpdateManager>
    {
        public override void OnAdd(UpdateManager self)
        {
            if (self.Root.Parent != null)
            {
                var ParentUpdate = self.Root.Parent.Root.GetComponent<UpdateManager>();//父节点
                ParentUpdate.update2.Add(self.id, self);
            }
        }
    }

    class UpdateManagerRemoveSystem : RemoveSystem<UpdateManager>
    {
        public override void OnRemove(UpdateManager self)
        {
            if (self.Root.Parent != null)
            {
                var ParentUpdate = self.Root.Parent.Root.GetComponent<UpdateManager>();//父节点
                ParentUpdate.update1.Remove(self.id);
                ParentUpdate.update2.Remove(self.id);
            }
        }
    }

    class UpdateManagerDestroySystem : DestroySystem<UpdateManager>
    {
        public override void OnDestroy(UpdateManager self)
        {
            self.systems = null;
        }
    }

    class UpdateManagerEntitySystem : EntitySystem<UpdateManager>
    {
        public override void OnAddEntity(UpdateManager self, Entity entity)
        {
            if (self.systems.ContainsKey(entity.Type))
            {
                self.update2.Add(entity.id, entity);
            }
        }

        public override void OnRemoveEntity(UpdateManager self, Entity entity)
        {
            if (self.systems.ContainsKey(entity.Type))
            {
                self.update1.Remove(entity.id);
                self.update2.Remove(entity.id);
            }
        }
    }








}