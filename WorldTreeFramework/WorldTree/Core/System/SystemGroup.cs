
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
    public class SystemGroup : Dictionary<Type, List<IEntitySystem>>
    {
        /// <summary>
        /// 系统的类型
        /// </summary>
        public Type systemType;
    }

}
