/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/4 14:41

* 描述： 引用池管理器

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 引用池管理器
    /// </summary>
    public class ReferencedPoolManager : Node
        , ComponentOf<WorldTreeCore>
    {
        /// <summary>
        /// 全部节点
        /// </summary>
        public UnitDictionary<long, INode> allNode = new UnitDictionary<long, INode>();
        /// <summary>
        /// 分类节点
        /// </summary>
        public UnitDictionary<Type, ReferencedPool> pools = new UnitDictionary<Type, ReferencedPool>();

        public override void OnDispose()
        {
            allNode.Clear();
            pools.Clear();
            allNode = null;
            pools = null;
        }
    }
}
