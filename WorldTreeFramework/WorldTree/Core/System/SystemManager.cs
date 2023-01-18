/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/26 16:29

* 描述： 系统管理器
* 
* 通过反射获取全局继承了ISystem的接口的方法类
* 

*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine.Rendering.VirtualTexturing;

namespace WorldTree
{

    /// <summary>
    /// 系统管理器
    /// </summary>
    public class SystemManager : Entity
    {
        public UnitDictionary<Type, HashSet<Type>> ListenerTypes = new UnitDictionary<Type, HashSet<Type>>();//?

        //接口类型，（实例类型，系统）
        private UnitDictionary<Type, SystemGroup> ListenerSystems = new UnitDictionary<Type, SystemGroup>();
        private UnitDictionary<Type, SystemGroup> TargetSystems = new UnitDictionary<Type, SystemGroup>();


        /// <summary>
        /// 目标，接口类型，(监听类,系统)
        /// </summary>
        private UnitDictionary<Type, Dictionary<Type, SystemGroup>> ListenerSystems_1 = new UnitDictionary<Type, Dictionary<Type, SystemGroup>>();



        //接口类型，（实例类型，系统）
        private UnitDictionary<Type, SystemGroup> InterfaceSystems = new UnitDictionary<Type, SystemGroup>();

        public SystemManager() : base()
        {
            Initialize();
        }

        private void Initialize()
        {
            var types = FindTypesIsInterface(typeof(ISystem));
            //将名字进行排序，规范触发顺序
            types.Sort((item1, item2) => item1.Name.CompareTo(item2.Name));

            List<IListenerSystem> EntitySystems = new List<IListenerSystem>();//只指定了系统的监听器系统

            foreach (var itemType in types)//遍历实现接口的类
            {
                //实例化系统类
                ISystem system = Activator.CreateInstance(itemType, true) as ISystem;

                if (system is IListenerSystem)
                {
                    var LSystem = system as IListenerSystem;//转换为监听系统

                    ListenerSystems.GetOrNewValue(LSystem.SystemType).GetOrNewValue(LSystem.EntityType).Add(LSystem);

                    //?
                    if (LSystem.TargetEntityType == typeof(Entity) && LSystem.TargetSystemType != typeof(ISystem))
                    {
                        EntitySystems.Add(LSystem);
                    }
                    else
                    {
                        ListenerSystems_1.GetOrNewValue(LSystem.TargetEntityType).GetOrNewValue(LSystem.SystemType).GetOrNewValue(LSystem.EntityType).Add(LSystem);

                        //TargetSystems.GetOrNewValue(LSystem.SystemType).GetOrNewValue(LSystem.TargetEntityType).Add(LSystem);
                    }

                    HashSet<Type> HashTypes = ListenerTypes.GetOrNewValue(LSystem.SystemType);
                    if (!HashTypes.Contains(LSystem.EntityType))
                    {
                        HashTypes.Add(LSystem.EntityType);
                    }
                }
                else
                {

                    InterfaceSystems.GetOrNewValue(system.SystemType).GetOrNewValue(system.EntityType).Add(system);
                }
            }


            foreach (IListenerSystem LSystem in EntitySystems)//? 
            {
                if (InterfaceSystems.TryGetValue(LSystem.TargetSystemType, out SystemGroup group))
                {
                    foreach (var systemList in group)
                    {
                        foreach (var system in systemList.Value)
                        {
                            ListenerSystems_1.GetOrNewValue(LSystem.TargetEntityType).GetOrNewValue(LSystem.SystemType).GetOrNewValue(LSystem.EntityType).Add(LSystem);

                            //TargetSystems.GetOrNewValue(LSystem.SystemType).GetOrNewValue(system.EntityType).Add(LSystem);
                        }
                    }
                }
            }


        }

        #region 监听目标系统组

        /// <summary>
        /// 获取监听目标系统组
        /// </summary>
        public bool TryGetListenerTargetGroup<T>(out SystemGroup systemGroup) where T : ISystem => TryGetListenerTargetGroup(typeof(T), out systemGroup);

        /// <summary>
        /// 获取监听目标系统组
        /// </summary>
        public bool TryGetListenerTargetGroup(Type Interface, out SystemGroup systemGroup) => TargetSystems.TryGetValue(Interface, out systemGroup);

        /// <summary>
        /// 获取监听目标系统列表
        /// </summary>
        public bool TryGetListenerTargetSystems<T>(Type type, out List<ISystem> systems)
        {
            if (TargetSystems.TryGetValue(typeof(T), out SystemGroup systemGroup))
            {
                return systemGroup.TryGetValue(type, out systems);
            }
            else
            {
                systems = null;
                return false;
            }
        }
        #endregion

        #region  监听系统组

        /// <summary>
        /// 获取监听系统组
        /// </summary>
        public bool TryGetListenerGroup<T>(out SystemGroup systemGroup) where T : ISystem => TryGetListenerGroup(typeof(T), out systemGroup);

        /// <summary>
        /// 获取监听系统组
        /// </summary>
        public bool TryGetListenerGroup(Type Interface, out SystemGroup systemGroup) => ListenerSystems.TryGetValue(Interface, out systemGroup);
        /// <summary>
        /// 获取监听系统
        /// </summary>
        public bool TryGetListenerSystems<T>(Type type, out List<ISystem> systems)
        {
            if (ListenerSystems.TryGetValue(typeof(T), out SystemGroup systemGroup))
            {
                return systemGroup.TryGetValue(type, out systems);
            }
            else
            {
                systems = null;
                return false;
            }
        }
        #endregion




        #region  系统组
        /// <summary>
        /// 获取系统组
        /// </summary>
        public SystemGroup GetGroup<T>() where T : ISystem => GetGroup(typeof(T));

        /// <summary>
        /// 获取系统组
        /// </summary>
        public SystemGroup GetGroup(Type Interface)
        {
            TryGetGroup(Interface, out SystemGroup systemGroup);
            return systemGroup;
        }

        /// <summary>
        /// 获取系统组
        /// </summary>
        public bool TryGetGroup<T>(out SystemGroup systemGroup)
         where T : ISystem
        {
            return TryGetGroup(typeof(T), out systemGroup);
        }

        /// <summary>
        /// 获取系统组
        /// </summary>
        public bool TryGetGroup(Type Interface, out SystemGroup systemGroup)
        {
            return InterfaceSystems.TryGetValue(Interface, out systemGroup);
        }

        #endregion

        #region  系统列表

        /// <summary>
        /// 获取单类型系统列表
        /// </summary>
        public List<ISystem> GetSystems<T>(Type type)
        {
            if (InterfaceSystems.TryGetValue(typeof(T), out SystemGroup systemGroup))
            {
                if (systemGroup.TryGetValue(type, out List<ISystem> systems))
                {
                    return systems;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取单类型系统列表
        /// </summary>
        public bool TryGetSystems(Type EntityType, Type SystemType, out List<ISystem> systems)
        {
            if (InterfaceSystems.TryGetValue(SystemType, out SystemGroup systemGroup))
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
