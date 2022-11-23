
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/23 18:17

* 描述： 系统全局广播器
* 
* 用于全局广播拥有指定系统的实体

*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 系统全局广播
    /// </summary>
    public class SystemGlobalBroadcast : SystemBroadcast{ }

    class SystemGlobalBroadcastNewSystem : AwakeSystem<SystemGlobalBroadcast, Type>
    {
        public override void OnAwake(SystemGlobalBroadcast self, Type type)
        {
            if (self.Root.SystemManager.TryGetGroup(type, out self.systems))
            {
                foreach (var entity in self.Root.allEntity.Values)
                {

                    self.AddEntity(entity);
                }
            }
        }
    }

    class SystemGlobalBroadcastEntityAddSystem : EntityAddSystem<SystemGlobalBroadcast>
    {
        public override void OnEntityAdd(SystemGlobalBroadcast self, Entity entity)
        {
            self.AddEntity(entity);
        }
    }
    class SystemGlobalBroadcastEntityRemoveSystem : EntityRemoveSystem<SystemGlobalBroadcast>
    {
        public override void OnEntityRemove(SystemGlobalBroadcast self, Entity entity)
        {
            self.RemoveEntity(entity);
        }
    }

    class SystemGlobalBroadcastAddSystem : AddSystem<SystemGlobalBroadcast>
    {
        public override void OnAdd(SystemGlobalBroadcast self)
        {
            self.update1 = self.PoolGet<UnitDictionary<long, Entity>>();
            self.update2 = self.PoolGet<UnitDictionary<long, Entity>>();
        }
    }

    class SystemGlobalBroadcastRemoveSystem : RemoveSystem<SystemGlobalBroadcast>
    {
        public override void OnRemove(SystemGlobalBroadcast self)
        {
            self.update1.Dispose();
            self.update2.Dispose();
        }
    }
}
