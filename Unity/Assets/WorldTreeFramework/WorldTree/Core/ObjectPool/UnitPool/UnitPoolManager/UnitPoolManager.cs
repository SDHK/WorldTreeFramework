
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/11 11:05

* 描述： 单位对象池管理器

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 单位对象池管理器
    /// </summary>
    public class UnitPoolManager : Node, ComponentOf<WorldTreeCore>
        , AsRule<IAwakeRule>
    {
        public TreeDictionary<Type, UnitPool> m_Pools;

    }
}
