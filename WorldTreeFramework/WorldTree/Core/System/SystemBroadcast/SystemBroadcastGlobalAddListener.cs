
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
    /// 系统广播全局实体添加监听器
    /// </summary>
    public class SystemBroadcastGlobalAddListener : Entity
    {
        public SystemBroadcast systemBroadcast;
    }
    class SystemBroadcastGlobalListenerEntityAddSystem : ListenerAddSystem<SystemBroadcastGlobalAddListener>
    {
        public override void OnAdd(SystemBroadcastGlobalAddListener self, Entity entity)
        {
            self.systemBroadcast?.AddEntity(entity);
        }
    }
}
