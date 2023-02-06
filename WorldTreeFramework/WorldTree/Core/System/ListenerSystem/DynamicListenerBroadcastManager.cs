using System;
using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 动态监听器广播管理器
    /// </summary>
    public class DynamicListenerBroadcastManager : Entity
    {
        /// <summary>
        /// 目标，系统，广播
        /// </summary>
        public Dictionary<Type, ListenerBroadcastGroup> BroadcastGroups = new Dictionary<Type, ListenerBroadcastGroup>();


        /// <summary>
        /// 监听器类型，监听器实体集合
        /// </summary>
        public UnitDictionary<Type, UnitDictionary<long, Entity>> DynamicListenerPool = new UnitDictionary<Type, UnitDictionary<long, Entity>>();


        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            IsRecycle = true;
            IsDisposed = true;
            BroadcastGroups.Clear();
        }


        #region 获取广播

        /// <summary>
        /// 尝试添加动态广播
        /// </summary>
        public bool TryAddBroadcast(Type Target, Type System, out SystemBroadcast broadcast)
        {
            //则判断这个系统类型是否有动态类型监听系统组
            if (Root.SystemManager.TryGetTargetSystemGroup(System, typeof(Entity), out var systemGroup))
            {

            }
            broadcast = null;
            return false;
        }


        /// <summary>
        /// 尝试获取动态广播
        /// </summary>
        public bool TryGetBroadcast(Type Target, Type System, out SystemBroadcast broadcast)
        {
            if (BroadcastGroups.TryGetValue(Target, out var group))
            {
                return group.TryGetBroadcast(System, out broadcast);
            }
            else
            {
                broadcast = null;
                return false;
            }
        }
        #endregion

        #region 获取广播组

        /// <summary>
        /// 获取广播组
        /// </summary>
        public ListenerBroadcastGroup GetGroup(Type Target)
        {
            if (!BroadcastGroups.TryGetValue(Target, out var group))
            {
                group = new ListenerBroadcastGroup();
                group.Target = Target;
                group.id = Root.IdManager.GetId();
                group.Root = Root;
                BroadcastGroups.Add(Target, group);
                AddChildren(group);
            }
            return group;
        }

        /// <summary>
        /// 尝试获取广播组
        /// </summary>
        public bool TryGetGroup(Type target, out ListenerBroadcastGroup group)
        {
            return BroadcastGroups.TryGetValue(target, out group);
        }

        #endregion
    }
}
