
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/31 10:08

* 描述： 系统广播组

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 系统广播组
    /// </summary>
    public class SystemBroadcastGroup : Entity
    {
        public UnitDictionary<Type, SystemBroadcast> broadcasts;

        public SystemBroadcast GetBroadcast<T>() => GetBroadcast(typeof(T));

        public SystemBroadcast GetBroadcast(Type type)
        {
            if (!broadcasts.TryGetValue(type, out var broadcast))
            {
                broadcast = this.AddChildren<SystemBroadcast>().Load(type);
                broadcasts.Add(type, broadcast);
            }
            return broadcast;
        }
    }

    class SystemBroadcastGroupAddSystem : AddSystem<SystemBroadcastGroup>
    {
        public override void OnAdd(SystemBroadcastGroup self)
        {
            self.PoolGet(out self.broadcasts);
        }
    }

    class SystemBroadcastGroupRemoveSystem : RemoveSystem<SystemBroadcastGroup>
    {
        public override void OnRemove(SystemBroadcastGroup self)
        {
            self.broadcasts.Dispose();
        }
    }

}
