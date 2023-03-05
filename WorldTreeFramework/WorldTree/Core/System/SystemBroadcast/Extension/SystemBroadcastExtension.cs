/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 

*/

using System;

namespace WorldTree
{
    public static class SystemBroadcastExtension
    {

        /// <summary>
        /// 广播初始化填装实体
        /// </summary>
        public static SystemBroadcast Load<T>(this SystemBroadcast broadcast) where T : IRule => Load(broadcast, typeof(T));
        /// <summary>
        /// 广播初始化填装实体
        /// </summary>
        public static SystemBroadcast Load(this SystemBroadcast broadcast, Type systemType)
        {
            if (broadcast.Root.RuleManager.TryGetRuleGroup(systemType, out broadcast.systems))
            {
                broadcast.Clear();
                foreach (var item in broadcast.systems)
                {
                    if (broadcast.Root.EntityPoolManager.pools.TryGetValue(item.Key, out EntityPool pool))
                    {
                        foreach (var entity in pool.Entitys)
                        {
                            broadcast.Enqueue(entity.Value);
                        }
                    }
                }
            }
            return broadcast;
        }

        /// <summary>
        /// 获取对应系统的全局广播
        /// </summary>
        public static SystemBroadcast GetSystemGlobalBroadcast<T>(this Node self)
        where T : IRule
        {
            var systemBroadcast = self.Root.AddComponent<SystemBroadcastGroup>().GetBroadcast<T>();
            systemBroadcast.AddComponent<GlobalEntityAddListener>().ListenerSwitchesTarget(typeof(T), ListenerState.Rule);
            systemBroadcast.AddComponent<GlobalEntityRemoveListener>().ListenerSwitchesTarget(typeof(T), ListenerState.Rule);
            return systemBroadcast;
        }
    }

}
