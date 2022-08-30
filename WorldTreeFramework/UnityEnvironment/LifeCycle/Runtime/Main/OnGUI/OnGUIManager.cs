using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// OnGUI生命周期管理器实体
    /// </summary>
    public class OnGUIManager : Entity
    {
        public float deltaTime = 0.2f;
        public UnitDictionary<long, Entity> update1 = new UnitDictionary<long, Entity>();
        public UnitDictionary<long, Entity> update2 = new UnitDictionary<long, Entity>();
        public SystemGroup systems;
        public void Update()
        {
            (update1, update2) = (update2, update1);

            while (update1.Count != 0 && IsActice)
            {
                long firstKey = update1.Keys.First();
                Entity entity = update1[firstKey];
                if (entity.IsActice)
                {
                    if (systems.TryGetValue(entity.Type, out List<ISystem> systemList))
                    {
                        foreach (IOnGUISystem system in systemList)
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
        }


        class OnGUIManagerUpdateSystem : OnGUISystem<OnGUIManager>
        {
            public override void OnGUI(OnGUIManager self, float deltaTime)
            {
                self.deltaTime = deltaTime;
                self.Update();
            }
        }
        class OnGUIManagerNewSystem : NewSystem<OnGUIManager>
        {
            public override void OnNew(OnGUIManager self)
            {
                self.systems = self.RootGetSystemGroup<IOnGUISystem>();
            }
        }

        class OnGUIManagerAddSystem : AddSystem<OnGUIManager>
        {
            public override void OnAdd(OnGUIManager self)
            {
                if (self.Root.Parent != null)
                {
                    var ParentUpdate = self.Root.Parent.Root.GetComponent<OnGUIManager>();//父节点
                    ParentUpdate.update2.Add(self.id, self);
                }
            }
        }

        class OnGUIManagerRemoveSystem : RemoveSystem<OnGUIManager>
        {
            public override void OnRemove(OnGUIManager self)
            {
                if (self.Root.Parent != null)
                {
                    var ParentUpdate = self.Root.Parent.Root.GetComponent<OnGUIManager>();//父节点
                    ParentUpdate.update1.Remove(self.id);
                    ParentUpdate.update2.Remove(self.id);
                }
            }
        }

        class OnGUIManagerDestroySystem : DestroySystem<OnGUIManager>
        {
            public override void OnDestroy(OnGUIManager self)
            {
                self.systems = null;
            }
        }

        class OnGUIManagerEntitySystem : EntitySystem<OnGUIManager>
        {
            public override void OnAddEntity(OnGUIManager self, Entity entity)
            {
                if (self.systems.ContainsKey(entity.Type))
                {
                    self.update2.Add(entity.id, entity);
                }
            }

            public override void OnRemoveEntity(OnGUIManager self, Entity entity)
            {
                if (self.systems.ContainsKey(entity.Type))
                {
                    self.update1.Remove(entity.id);
                    self.update2.Remove(entity.id);
                }
            }
        }

    }
}
