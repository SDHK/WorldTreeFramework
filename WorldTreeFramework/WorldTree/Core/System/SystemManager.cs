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
    public static class SystemManagerExtension
    {

        public static SystemManager SystemManager(this Entity self)
        {
            return self.Root.SystemManager;
        }

        /// <summary>
        /// 获取系统组
        /// </summary>
        public static SystemGroup RootGetSystemGroup<T>(this Entity self)
        where T : ISystem
        {
            return self.Root.SystemManager.GetSystemGroup<T>();
        }

        /// <summary>
        /// 获取单类型系统列表
        /// </summary>
        public static List<ISystem> RootGetSystems<T>(this Entity self, Type type)
        {
            return self.Root.SystemManager.GetSystems<T>(type);
        }

    }


    class SystemManagerRemove : RemoveSystem<SystemManager>
    {
        public override void OnRemove(SystemManager self)
        {
            self.Dispose();//全部释放
        }
    }


    /// <summary>
    /// 系统管理器
    /// </summary>
    public class SystemManager : Entity, IUnit
    {
        //接口类型，（实例类型，实例方法）
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

                Type SystemType = system.SystemType;
                Type EntityType = system.EntityType;
                if (!InterfaceSystems.TryGetValue(SystemType, out SystemGroup systemGroup))
                {
                    systemGroup = new SystemGroup();
                    systemGroup.systemType = SystemType;
                    InterfaceSystems.Add(SystemType, systemGroup);
                }

                systemGroup.GetSystems(EntityType).Add(system);
            }
        }

        /// <summary>
        /// 获取系统组
        /// </summary>
        public SystemGroup GetSystemGroup<T>() where T : ISystem => GetSystemGroup(typeof(T));

        /// <summary>
        /// 获取系统组
        /// </summary>
        public SystemGroup GetSystemGroup(Type Interface)
        {
            TryGetSystemGroup(Interface, out  SystemGroup systemGroup);
            return systemGroup;
        }

        /// <summary>
        /// 获取系统组
        /// </summary>
        public bool TryGetSystemGroup<T>( out SystemGroup systemGroup)
         where T : ISystem
        {
            return TryGetSystemGroup(typeof(T),out systemGroup);
        }

        /// <summary>
        /// 获取系统组
        /// </summary>
        public bool TryGetSystemGroup(Type Interface, out SystemGroup systemGroup)
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
                return systemGroup.GetSystems(type);
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

        public override void OnDispose()
        {
            InterfaceSystems.Clear();
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
