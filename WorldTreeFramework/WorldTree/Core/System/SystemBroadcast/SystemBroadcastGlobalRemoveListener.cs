namespace WorldTree
{
    /// <summary>
    /// 系统广播全局实体移除监听器
    /// </summary>
    public class SystemBroadcastGlobalRemoveListener : Entity
    {
        public SystemBroadcast systemBroadcast;
    }

    class SystemBroadcastGlobalRemoveListenerAddSystem : AddSystem<SystemBroadcastGlobalRemoveListener>
    {
        public override void OnAdd(SystemBroadcastGlobalRemoveListener self)
        {
            self.TryGetParent(out self.systemBroadcast);
        }
    }
    class SystemBroadcastGlobalRemoveListenerRemoveSystem : RemoveSystem<SystemBroadcastGlobalRemoveListener>
    {
        public override void OnRemove(SystemBroadcastGlobalRemoveListener self)
        {
            self.systemBroadcast = null;
        }
    }
    class SystemBroadcastGlobalRemoveListenerEntityAddSystem : ListenerRemoveSystem<SystemBroadcastGlobalRemoveListener>
    {
        public override void OnRemove(SystemBroadcastGlobalRemoveListener self, Entity entity)
        {
            self.systemBroadcast?.RemoveEntity(entity);
        }
    }
}
