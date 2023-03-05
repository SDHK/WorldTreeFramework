/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/26 16:29

* 描述： 法则管理器
* 
* 通过反射获取全局继承了ISystem的接口的方法类
* 

*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WorldTree
{

    /// <summary>
    /// 法则管理器
    /// </summary>
    public class RuleManager : Node
    {

        /// <summary>
        /// 动态监听器类型名单
        /// </summary>
        public UnitHashSet<Type> DynamicListenerTypes = new UnitHashSet<Type>();

        /// <summary>
        /// 目标， 接口类型 ，(监听类,法则)    动态目标是Entity
        /// </summary>
        public UnitDictionary<Type, Dictionary<Type, RuleGroup>> TargetSystems = new UnitDictionary<Type, Dictionary<Type, RuleGroup>>();

        /// <summary>
        /// 监听类 ，接口类型，（目标，法则） 动态目标是Entity
        /// </summary>
        public UnitDictionary<Type, Dictionary<Type, RuleGroup>> ListenerSystems = new UnitDictionary<Type, Dictionary<Type, RuleGroup>>();


        //接口类型，（实例类型，法则）
        private UnitDictionary<Type, RuleGroup> InterfaceSystems = new UnitDictionary<Type, RuleGroup>();

        public RuleManager() : base()
        {
            Initialize();
        }

        private void Initialize()
        {
            var types = FindTypesIsInterface(typeof(IRule));
            //将名字进行排序，规范触发顺序
            types.Sort((item1, item2) => item1.Name.CompareTo(item2.Name));

            List<IListenerSystem> EntitySystems = new List<IListenerSystem>();//只指定了法则的监听器法则

            foreach (var itemType in types)//遍历实现接口的类
            {
                //实例化法则类
                IRule system = Activator.CreateInstance(itemType, true) as IRule;

                if (system is IListenerSystem)
                {
                    var LSystem = system as IListenerSystem;//转换为监听法则

                    if (LSystem.TargetEntityType == typeof(Node) && LSystem.TargetSystemType != typeof(IRule))
                    {
                        EntitySystems.Add(LSystem); //约束了法则
                    }
                    else
                    {
                        //指定了实体，或 动态指定实体

                        var ListenerGroup = ListenerSystems.GetValue(LSystem.EntityType).GetValue(LSystem.RuleType);
                        ListenerGroup.GetValue(LSystem.TargetEntityType).Add(LSystem);
                        ListenerGroup.RuleType = LSystem.RuleType;

                        var TargetGroup = TargetSystems.GetValue(LSystem.TargetEntityType).GetValue(LSystem.RuleType);
                        TargetGroup.GetValue(LSystem.EntityType).Add(LSystem);
                        TargetGroup.RuleType = LSystem.RuleType;

                        //动态监听器判断
                        if (LSystem.TargetEntityType == typeof(Node) && LSystem.TargetSystemType == typeof(IRule))
                        {
                            if (!DynamicListenerTypes.Contains(LSystem.EntityType)) DynamicListenerTypes.Add(LSystem.EntityType);
                        }

                    }
                }
                else
                {
                    var group = InterfaceSystems.GetValue(system.RuleType);
                    group.GetValue(system.EntityType).Add(system);
                    group.RuleType = system.RuleType;
                }
            }


            foreach (IListenerSystem LSystem in EntitySystems)//查询法则对应实体 
            {
                if (InterfaceSystems.TryGetValue(LSystem.TargetSystemType, out RuleGroup group))
                {
                    foreach (var systemList in group)
                    {
                        foreach (var system in systemList.Value)
                        {
                            var ListenerGroup = ListenerSystems.GetValue(LSystem.EntityType).GetValue(LSystem.RuleType);
                            ListenerGroup.GetValue(system.EntityType).Add(LSystem);
                            ListenerGroup.RuleType = LSystem.RuleType;

                            var TargetGroup = TargetSystems.GetValue(system.EntityType).GetValue(LSystem.RuleType);
                            TargetGroup.GetValue(LSystem.EntityType).Add(LSystem);
                            TargetGroup.RuleType = LSystem.RuleType;
                        }

                    }
                }
            }


        }

        #region 监听目标法则组

        /// <summary>
        /// 获取监听目标法则组
        /// </summary>
        public bool TryGetTargetSystemGroup<T>(Type targetType, out RuleGroup systemGroup)
            where T : IListenerSystem
        {
            return TryGetTargetSystemGroup(typeof(T), targetType, out systemGroup);
        }

        /// <summary>
        /// 获取监听目标法则组
        /// </summary>
        public bool TryGetTargetSystemGroup(Type systemType, Type targetType, out RuleGroup systemGroup)
        {
            if (TargetSystems.TryGetValue(targetType, out var systemGroups))
            {
                return systemGroups.TryGetValue(systemType, out systemGroup);
            }
            systemGroup = null;
            return false;
        }

        /// <summary>
        /// 获取监听目标法则列表
        /// </summary>
        public bool TryGetTargetSystems<T>(Type targetType, Type listenerType, out List<IRule> systems)
        {
            if (TargetSystems.TryGetValue(targetType, out var systemGroups))
            {
                if (systemGroups.TryGetValue(typeof(T), out var systemGroup))
                {
                    return systemGroup.TryGetValue(listenerType, out systems);
                }
            }
            systems = null;
            return false;
        }
        #endregion

        #region  监听法则组

        /// <summary>
        /// 获取监听法则组
        /// </summary>
        public bool TryGetListenerSystemGroup<T>(Type listenerType, out RuleGroup systemGroup)
        {
            if (ListenerSystems.TryGetValue(listenerType, out var systemGroups))
            {
                return systemGroups.TryGetValue(typeof(T), out systemGroup);
            }
            systemGroup = null;
            return false;
        }

        /// <summary>
        /// 获取监听法则
        /// </summary>
        public bool TryGetListenerSystems<T>(Type listenerType, Type targetType, out List<IRule> systems)
        {
            if (ListenerSystems.TryGetValue(listenerType, out var systemGroups))
            {
                if (systemGroups.TryGetValue(typeof(T), out var systemGroup))
                {
                    return systemGroup.TryGetValue(targetType, out systems);
                }
            }
            systems = null;
            return false;
        }
        #endregion




        #region  法则组
        /// <summary>
        /// 获取法则组
        /// </summary>
        public RuleGroup GetGroup<T>() where T : IRule => GetGroup(typeof(T));

        /// <summary>
        /// 获取法则组
        /// </summary>
        public RuleGroup GetGroup(Type Interface)
        {
            TryGetGroup(Interface, out RuleGroup systemGroup);
            return systemGroup;
        }

        /// <summary>
        /// 获取法则组
        /// </summary>
        public bool TryGetGroup<T>(out RuleGroup systemGroup)
         where T : IRule
        {
            return TryGetGroup(typeof(T), out systemGroup);
        }

        /// <summary>
        /// 获取法则组
        /// </summary>
        public bool TryGetGroup(Type Interface, out RuleGroup systemGroup)
        {
            return InterfaceSystems.TryGetValue(Interface, out systemGroup);
        }

        #endregion

        #region  法则列表

        /// <summary>
        /// 获取单类型法则列表
        /// </summary>
        public List<IRule> GetSystems<T>(Type type)
        {
            if (InterfaceSystems.TryGetValue(typeof(T), out RuleGroup systemGroup))
            {
                if (systemGroup.TryGetValue(type, out List<IRule> systems))
                {
                    return systems;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取单类型法则列表
        /// </summary>
        public bool TryGetSystems(Type EntityType, Type SystemType, out List<IRule> systems)
        {
            if (InterfaceSystems.TryGetValue(SystemType, out RuleGroup systemGroup))
            {
                return systemGroup.TryGetValue(EntityType, out systems);
            }
            else
            {
                systems = null;
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            InterfaceSystems.Clear();
            ListenerSystems.Clear();
            TargetSystems.Clear();
            IsRecycle = true;
            IsDisposed = true;
        }

        /// <summary>
        /// 查找继承了接口的类型
        /// </summary>
        private static List<Type> FindTypesIsInterface(Type Interface)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(T => T.GetInterfaces().Contains(Interface) && !T.IsAbstract)).ToList();
        }

    }
}
