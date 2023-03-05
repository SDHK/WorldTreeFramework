
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/6 10:17

* 描述： 

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 监听器广播集合
    /// </summary>
    public class ListenerBroadcastGroup : Node
    {
        /// <summary>
        /// 目标类型
        /// </summary>
        public Type Target;

        /// <summary>
        /// 法则广播器字典
        /// </summary>
        /// <remarks>法则类型,广播器</remarks>
        public Dictionary<Type, SystemBroadcast> Broadcasts = new Dictionary<Type, SystemBroadcast>();

        public override string ToString()
        {
            return $"{Type.Name} : {Target}" ; 
        }

        /// <summary>
        /// 获取广播
        /// </summary>
        public SystemBroadcast GetBroadcast(Type system)
        {
            if (!Broadcasts.TryGetValue(system, out var broadcast))
            {
                broadcast = new SystemBroadcast();
                broadcast.entityQueue = new DynamicEntityQueue();
                broadcast.entityQueue.idQueue = new UnitQueue<long>();
                broadcast.entityQueue.removeId = new UnitDictionary<long, int>();
                broadcast.entityQueue.entitys = new UnitDictionary<long, Node>();

                broadcast.id = Root.IdManager.GetId();
                broadcast.Root = Root;
                Broadcasts.Add(system, broadcast);
                AddChildren(broadcast);
            }
            return broadcast;
        }

        /// <summary>
        /// 尝试获取广播
        /// </summary>
        public bool TryGetBroadcast(Type system, out SystemBroadcast broadcast)
        {
            return Broadcasts.TryGetValue(system, out broadcast);
        }



    }
}
