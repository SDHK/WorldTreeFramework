
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/12 19:00

* 描述： 数组对象池管理器

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 数组对象池管理器
    /// </summary>
    public class ArrayPoolManager : Node, ComponentOf<WorldTreeCore>
    {
        public TreeDictionary<Type, ArrayPoolGroup> PoolGroups;
    }
}
