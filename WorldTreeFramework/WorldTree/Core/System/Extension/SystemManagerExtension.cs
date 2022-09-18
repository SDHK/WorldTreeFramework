/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/17 14:22

* 描述： 

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static SystemGroup GetSystemGroup<T>(this Entity self)
        where T : ISystem
        {
            return self.Root.SystemManager.GetGroup<T>();
        }

        /// <summary>
        /// 获取单类型系统列表
        /// </summary>
        public static List<ISystem> GetSystems<T>(this Entity self, Type type)
        {
            return self.Root.SystemManager.GetSystems<T>(type);
        }

        /// <summary>
        /// 获取系统全局广播
        /// </summary>
        public static SystemBroadcast GetSystemBroadcast<T>(this Entity self)
        where  T : ISystem
        {
            return self.Root.SystemManager.GetBroadcast<T>();
        }

        /// <summary>
        /// 获取系统执行器
        /// </summary>
        public static SystemActuator GetSystemActuator<T>(this Entity self)
        where T : ISystem
        {
            if (self.Root.SystemManager.TryGetGroup<T>(out SystemGroup group))
            {
                return self.AddChildren<SystemActuator, SystemGroup>(group);
            }
            else
            {
                return null;
            }
        }
    }

}
