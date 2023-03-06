/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 11:30

* 描述： 子节点
* 
* 用节点ID作为键值，因此可以添加相同类型的子节点

*/

namespace WorldTree
{
    public abstract partial class Node
    {
        /// <summary>
        /// 子节点
        /// </summary>
        public UnitDictionary<long, Node> m_Children;

        /// <summary>
        /// 子节点
        /// </summary>
        public UnitDictionary<long, Node> Children
        {
            get
            {
                if (m_Children == null)
                {
                    m_Children = this.PoolGet<UnitDictionary<long, Node>>();
                }
                return m_Children;
            }
            set { m_Children = value; }
        }
    }
}
