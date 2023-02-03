
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/31 19:08

* 描述： 实体监听器
* 
* 用于实体动态监听全局任意实体事件

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 监听器状态
    /// </summary>
    public enum ListenerState
    {
        /// <summary>
        /// 不是监听器
        /// </summary>
        Not,
        /// <summary>
        /// 监听目标是实体
        /// </summary>
        Entity,
        /// <summary>
        /// 监听目标是系统
        /// </summary>
        System
    }

    public abstract partial class Entity
    {
        /// <summary>
        /// 动态监听器状态
        /// </summary>
        public ListenerState listenerState = ListenerState.Not;

        /// <summary>
        /// 动态监听目标类型
        /// </summary>
        public Type listenerTarget = null;

        /// <summary>
        /// 动态监听器切换目标
        /// </summary>
        public bool ListenerSwitchesTarget(Type targetType, ListenerState state)
        {
            if (listenerTarget != targetType)
            {
                //判断是否为监听器
                if (Root.SystemManager.DynamicListenerTypes.Contains(Type))
                {
                    DynamicListenerRemove();
                    listenerTarget = targetType;
                    listenerState = state;
                    DynamicListenerAdd();
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 动态监听器清除目标
        /// </summary>
        public void ListenerClearTarget()
        {
            DynamicListenerRemove();
            listenerTarget = null;
            listenerState = ListenerState.Not;
        }




        #region 私有

        #region 添加

        /// <summary>
        /// 监听器根据标记添加目标
        /// </summary>
        private void DynamicListenerAdd()
        {
            if (listenerTarget != null)
            {
                if (listenerState == ListenerState.Entity)
                {
                    if (listenerTarget == typeof(Entity))
                    {
                        DynamicListenerAddAll();
                    }
                    else
                    {
                        DynamicListenerAddTarget(listenerTarget);
                    }
                }
                else if (listenerState == ListenerState.System)
                {
                    DynamicListenerAddSystem(listenerTarget);
                }
            }
        }

        /// <summary>
        /// 监听器添加 所有实体
        /// </summary>
        private void DynamicListenerAddAll()
        {
            //获取 Entity 动态目标 系统组集合
            if (Root.SystemManager.TargetSystems.TryGetValue(typeof(Entity), out var systemGroups))
            {
                //遍历现有全部池
                foreach (var pool in Root.EntityPoolManager.pools)
                {
                    //遍历获取动态系统组，并添加自己
                    foreach (var systemGroup in systemGroups)
                    {
                        pool.Value.AddComponent<ListenerSystemBroadcastGroup>().GetDynamicBroadcast(systemGroup.Key).AddEntity(this);
                    }
                }
            }
        }

        /// <summary>
        /// 监听器添加 系统目标
        /// </summary>
        private void DynamicListenerAddSystem(Type type)
        {
            //获取系统组
            if (Root.SystemManager.TryGetGroup(type, out var targetSystemGroup))
            {
                //遍历系统组
                foreach (var targetSystems in targetSystemGroup)
                {
                    DynamicListenerAddTarget(targetSystems.Key);
                }
            }
        }

        /// <summary>
        /// 监听器添加 实体目标
        /// </summary>
        private void DynamicListenerAddTarget(Type type)
        {
            if (Root.EntityPoolManager.TryGetPool(type, out var pool))
            {
                PoolAddDynamicListener(pool);
            }
        }



        #endregion

        #region 移除

        /// <summary>
        /// 监听器根据标记移除目标
        /// </summary>
        private void DynamicListenerRemove()
        {
            if (listenerTarget != null)
            {
                if (listenerState == ListenerState.Entity)
                {
                    if (listenerTarget == typeof(Entity))
                    {
                        DynamicListenerRemoveAll();
                    }
                    else
                    {
                        DynamicListenerRemoveTarget(listenerTarget);
                    }
                }
                else if (listenerState == ListenerState.System)
                {
                    DynamicListenerRemoveSystem(listenerTarget);
                }
            }
        }

        /// <summary>
        /// 监听器移除 所有实体
        /// </summary>
        private void DynamicListenerRemoveAll()
        {
            //获取 Entity 动态目标 系统组集合
            if (Root.SystemManager.TargetSystems.TryGetValue(typeof(Entity), out var systemGroups))
            {
                //遍历现有全部池
                foreach (var pool in Root.EntityPoolManager.pools)
                {
                    //遍历获取动态系统组，并移除自己
                    foreach (var systemGroup in systemGroups)
                    {
                        pool.Value.AddComponent<ListenerSystemBroadcastGroup>().GetDynamicBroadcast(systemGroup.Key).RemoveEntity(this);
                    }
                }
            }
        }
        /// <summary>
        /// 监听器移除 系统目标
        /// </summary>
        private void DynamicListenerRemoveSystem(Type type)
        {
            //获取系统组
            if (Root.SystemManager.TryGetGroup(type, out var targetSystemGroup))
            {
                //遍历系统组
                foreach (var targetSystems in targetSystemGroup)
                {
                    DynamicListenerRemoveTarget(targetSystems.Key);
                }
            }
        }

        /// <summary>
        /// 监听器移除 实体目标
        /// </summary>
        private void DynamicListenerRemoveTarget(Type type)
        {
            if (Root.EntityPoolManager.TryGetPool(type, out var pool))
            {
                PoolRemoveDynamicListener(pool);
            }
        }
        #endregion

        #region 基础方法

        #region 动态

        /// <summary>
        /// 目标池添加动态监听器
        /// </summary>
        /// <param name="pool"></param>
        private void PoolAddDynamicListener(EntityPool pool)
        {
            //获取 Entity 动态目标 系统组集合
            if (Root.SystemManager.TargetSystems.TryGetValue(typeof(Entity), out var systemGroups))
            {
                //遍历获取动态系统组，并添加自己
                foreach (var systemGroup in systemGroups)
                {
                    pool.AddComponent<ListenerSystemBroadcastGroup>().GetDynamicBroadcast(systemGroup.Key).AddEntity(this);
                }
            }
        }
        /// <summary>
        /// 目标池移除动态监听器
        /// </summary>
        /// <param name="pool"></param>
        private void PoolRemoveDynamicListener(EntityPool pool)
        {
            //获取 Entity 动态目标 系统组集合
            if (Root.SystemManager.TargetSystems.TryGetValue(typeof(Entity), out var systemGroups))
            {
                //遍历获取动态系统组，并移除自己
                foreach (var systemGroup in systemGroups)
                {
                    pool.AddComponent<ListenerSystemBroadcastGroup>().GetDynamicBroadcast(systemGroup.Key).RemoveEntity(this);
                }
            }
        }
        #endregion
        #endregion

        #endregion

    }
}
