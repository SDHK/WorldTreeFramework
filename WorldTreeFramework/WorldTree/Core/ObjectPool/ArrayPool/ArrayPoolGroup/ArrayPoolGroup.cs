/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/12 10:37

* 描述： 数组对象池集合

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 数组对象池集合
    /// </summary>
    public class ArrayPoolGroup : Node, ChildOf<ArrayPoolManager>
        , AsRule<IAwakeRule<Type>>
    {
        /// <summary>
        /// 数组类型
        /// </summary>
        public Type ArrayType;
        public TreeDictionary<int, ArrayPool> Pools;
    }
}
