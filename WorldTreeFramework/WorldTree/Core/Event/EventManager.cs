
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/2 15:28

* 描述： 事件管理器
* 
* 启动后查询是否有事件系统类
* 存在，则反射特性获取键值，
* 将系统方法注册到对应的事件上
* 

*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventManager : Entity
    {
        public SystemGroup systemGroup;
    }

    class EventManagerAddSystem : AddSystem<EventManager>
    {
        public override void OnAdd(EventManager self)
        {
            //进行遍历分类
            self.systemGroup = self.RootGetSystemGroup<IEventSystem>();

            foreach (var systems in self.systemGroup.Values)
            {
                foreach (IEventSystem system in systems)
                {
                    //反射属性获取键值
                    Type key = typeof(EventDelegate);
                    object[] attributes = system.GetType().GetCustomAttributes(typeof(EventKeyAttribute), true);
                    if (attributes.Length != 0)
                    {
                        key = (attributes[0] as EventKeyAttribute)?.key;
                    }
                    //分组注册事件
                    self.AddComponent(key).To<EventDelegate>().AddDelegate(system.GetDeleate());
                }
            }
        }
    }
}
