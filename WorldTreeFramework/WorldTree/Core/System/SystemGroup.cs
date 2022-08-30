
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 系统集合组

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{

    /// <summary>
    /// 系统集合组
    /// </summary>
    public class SystemGroup : Dictionary<Type, List<ISystem>>
    {
        /// <summary>
        /// 获取系统类列表
        /// </summary>
        public List<ISystem> GetSystems(Type type)
        {
            List<ISystem> Isystems;
            if (!TryGetValue(type, out Isystems))
            {
                Isystems = new List<ISystem>();
                Add(type, Isystems);
            }

            return Isystems;
        }
    }
}
