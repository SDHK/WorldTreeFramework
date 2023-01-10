
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/23 18:17

* 描述： 系统全局广播器
* 
* 用于系统广播监听全局实体添加

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 系统广播全局实体监听器
    /// </summary>
    public class SystemBroadcastGlobalListener : Entity
    {
        public SystemBroadcast systemBroadcast;
    }
    class SystemBroadcastGlobalListenerAddSystem : AddSystem<SystemBroadcastGlobalListener>
    {
        public override void OnAdd(SystemBroadcastGlobalListener self)
        {
            if (self.TryGetParent(out self.systemBroadcast))
            {
                foreach (var entity in self.Root.allEntity.Values)//填装实体
                {
                    self.systemBroadcast.AddEntity(entity);
                }
            }
            else
            {
                throw new Exception("父节点不是SystemBroadcast");
            }
        }
    }

    class SystemBroadcastGlobalListenerEntityAddSystem : EntityAddSystem<SystemBroadcastGlobalListener>
    {
        public override void OnEntityAdd(SystemBroadcastGlobalListener self, Entity entity)
        {
            self.systemBroadcast?.AddEntity(entity);
        }
    }
}
