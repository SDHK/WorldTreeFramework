
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 法则集合

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 法则集合
    /// </summary>
    /// <remarks>
    /// <para>Key是类型，Value是类型的法则列表</para>
    /// <para>法则集合储存了 不同类型 对应的 同种法则</para>
    /// </remarks>
    public class RuleGroup : Dictionary<Type, List<IRule>>
    {
        /// <summary>
        /// 法则的类型
        /// </summary>
        public Type RuleType;
    }

}
