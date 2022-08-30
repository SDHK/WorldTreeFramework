/******************************

 * 作者: 闪电黑客

 * 日期: 2021/10/03 05:12:29

 * 最后日期: 。。。

 * 描述: 
    饿汉式泛型单例服务

    通过untiy提供的[RuntimeInitializeOnLoadMethod]启动

    自动全局反射实例化 继承了饿汉单例抽象类的子类:

    普通类：     SingletonEagerBase<T>
    mono组件：   SingletonMonoEagerBase<T>

******************************/

using WorldTree.SingletonAttribute;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WorldTree
{
    namespace SingletonAttribute
    {
        //       AttributeTargets.All(说明这个特性可以标记在什么元素上，类、字段、属性、方法、返回值等元素，ALL代表所有)
        //       AllowMultiple(说明这个特性在同一个元素上使用的次数，默认一个元素只能标记一种特性，但可以多种特性并存)
        //       Inherited(说明这个特性是否可以继承)

        /// <summary>
        /// 饿汉单例基类标记
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public class SingletonEagerBaseAttribute : Attribute { }

        /// <summary>
        /// Mono饿汉单例基类标记
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public class SingletonMonoEagerBaseAttribute : Attribute { }


        /// <summary>
        /// 饿汉单例初始化方法接口
        /// </summary>
        public interface ISingletonEager
        {
            /// <summary>
            /// 饿汉单例初始化方法
            /// </summary>
            void InitializeOnLoadMethod();
        }
    }




    /// <summary>
    /// 饿汉单例服务：通过反射全局查询饿汉类
    /// </summary>
    public class SingletonEagerServer
    {

        [RuntimeInitializeOnLoadMethod]
        private static void SingletonEager()
        {
            Type[] singletonEagers = FindTypes_Interface(AppDomain.CurrentDomain.GetAssemblies(), typeof(ISingletonEager));

            foreach (var item in singletonEagers)
            {
                if (item.BaseType.IsDefined(typeof(SingletonEagerBaseAttribute), false))
                {
                    (Activator.CreateInstance(item) as ISingletonEager).InitializeOnLoadMethod();
                }
                else if (item.BaseType.IsDefined(typeof(SingletonMonoEagerBaseAttribute), false))
                {
                    var gameObj = new GameObject(item.Name);
                    var component = gameObj.AddComponent(item);
                    UnityEngine.Object.DontDestroyOnLoad(gameObj);

                    (component as ISingletonEager).InitializeOnLoadMethod();
                }
            }
        }

        /// <summary>
        /// 查询继承了接口的类
        /// </summary>
        /// <param name="assemblys">程序集</param>
        /// <param name="Interface">接口</param>
        /// <returns>类型集合</returns>
        private static Type[] FindTypes_Interface(Assembly[] assemblys, Type Interface)
        {
            return assemblys.SelectMany(Assambly => Assambly.GetTypes().Where(T => T.GetInterfaces().Contains(Interface))).ToArray();
        }
    }
}