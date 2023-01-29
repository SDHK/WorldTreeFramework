using System;

namespace WorldTree
{
    /// <summary>
    /// 监听器系统广播管理器
    /// </summary>
    public class ListenerSystemBroadcastManager : Entity
    {
        public UnitDictionary<Type, ListenerSystemBroadcast> listenerSystemBroadcasts = new UnitDictionary<Type, ListenerSystemBroadcast>();


        /// <summary>
        /// 获取监听器广播
        /// </summary>
        public ListenerSystemBroadcast Get<T>()
        {
            Type type = typeof(T);
            return Get(type);
        }

        /// <summary>
        /// 获取监听器广播
        /// </summary>
        public ListenerSystemBroadcast Get(Type type)
        {
            if (!listenerSystemBroadcasts.TryGetValue(type, out ListenerSystemBroadcast systemBroadcast))
            {
                systemBroadcast = new ListenerSystemBroadcast(type);
                systemBroadcast.id = Root.IdManager.GetId();
                systemBroadcast.Root = Root;
                listenerSystemBroadcasts.Add(type, systemBroadcast);
                AddChildren(systemBroadcast);
            }
            return systemBroadcast;
        }

        /// <summary>
        /// 释放广播器
        /// </summary>
        public void DisposeBroadcast<T>()
        {
            Type type = typeof(T);
            if (listenerSystemBroadcasts.TryGetValue(type, out ListenerSystemBroadcast systemBroadcast))
            {
                systemBroadcast.Dispose();
                listenerSystemBroadcasts.Remove(type);
            }
        }



        /// <summary>
        /// 添加监听器
        /// </summary>
        public void AddListener(Entity listener)//是否添加any
        {

        }

    }

}
