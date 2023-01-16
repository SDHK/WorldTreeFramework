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

namespace WorldTree
{

    /// <summary>
    /// 系统管理器
    /// </summary>
    public class SystemManager : Entity
    {
        public UnitDictionary<Type, HashSet<Type>> ListenerTypes = new UnitDictionary<Type, HashSet<Type>>();

        //接口类型，（实例类型，系统）
        private UnitDictionary<Type, SystemGroup> ListenerSystems = new UnitDictionary<Type, SystemGroup>();


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
            foreach (var itemType in types)//遍历实现接口的类
            {
                //实例化系统类
                ISystem system = Activator.CreateInstance(itemType, true) as ISystem;

                if (system is IListenerSystem)
                {
                    Type SystemType = system.SystemType;
                    Type EntityType = system.EntityType;

                    if (!ListenerSystems.TryGetValue(SystemType, out SystemGroup systemGroup))
                    {
                        systemGroup = new SystemGroup();
                        systemGroup.systemType = SystemType;
                        ListenerSystems.Add(SystemType, systemGroup);
                    }


                    if (!systemGroup.TryGetValue(EntityType, out List<ISystem> systems))
                    {
                        systems = new List<ISystem>();
                        systemGroup.Add(EntityType, systems);
                    }
                    systems.Add(system);


                    if (!ListenerTypes.TryGetValue(SystemType, out HashSet<Type> HashTypes))
                    {
                        HashTypes = new HashSet<Type>();
                        ListenerTypes.Add(SystemType, HashTypes);
                    }
                    var listenerSystem = system as IListenerSystem;

                    if (!HashTypes.Contains(listenerSystem.ListenerEntityType))
                    {
                        HashTypes.Add(listenerSystem.ListenerEntityType);
                    }
                }
                else
                {

                    Type SystemType = system.SystemType;
                    Type EntityType = system.EntityType;
                    if (!InterfaceSystems.TryGetValue(SystemType, out SystemGroup systemGroup))
                    {
                        systemGroup = new SystemGroup();
                        systemGroup.systemType = SystemType;
                        InterfaceSystems.Add(SystemType, systemGroup);
                    }


                    if (!systemGroup.TryGetValue(EntityType, out List<ISystem> systems))
                    {
                        systems = new List<ISystem>();
                        systemGroup.Add(EntityType, systems);
                    }

                    systems.Add(system);
                }
            }
        }

     

        /// <summary>
        /// 获取系统组
        /// </summary>
        public SystemGroup GetListenerGroup<T>() where T : ISystem => GetListenerGroup(typeof(T));

        /// <summary>
        /// 获取系统组
        /// </summary>
        public SystemGroup GetListenerGroup(Type Interface)
        {
            if (ListenerSystems.TryGetValue(Interface, out SystemGroup systemGroup))
            {
                return systemGroup;
            }
            return null;
        }
        /// <summary>
        /// 获取监听系统
        /// </summary>
        public List<ISystem> GetListenerSystems<T>(Type type)
        {
            if (ListenerSystems.TryGetValue(typeof(T), out SystemGroup systemGroup))
            {
                if (systemGroup.TryGetValue(type, out List<ISystem> systems))
                {
                    return systems;
                }
            }
            return null;
        }

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

        public bool TryGetSystems(Type EntityType, Type SystemType, out List<ISystem> systems)
        {
            if (InterfaceSystems.TryGetValue(SystemType, out SystemGroup systemGroup))
            {
                return systemGroup.TryGetValue(EntityType, out systems);
            }
            systems = null;
            return false;
        }

        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            InterfaceSystems.Clear();
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
