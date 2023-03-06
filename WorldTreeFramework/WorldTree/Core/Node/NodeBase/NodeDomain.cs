
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/3 14:51

* 描述： 域节点
* 
* 用于分组，标签，获取上层节点。
* 
* 从字典查询节点是否存在，不存在则，
* 从父节点开始反向向上查询节点是否存在。
* 存在则存入字典。
* 
*/

using System;

namespace WorldTree
{
    public abstract partial class Node
    {
        /// <summary>
        /// 域节点
        /// </summary>
        public UnitDictionary<Type, Node> m_Domains;

        /// <summary>
        /// 域节点
        /// </summary>
        public UnitDictionary<Type, Node> Domains
        {
            get
            {
                if (m_Domains == null)
                {
                    m_Domains = this.PoolGet<UnitDictionary<Type, Node>>();
                }
                return m_Domains;
            }
            set { m_Domains = value; }
        }



    }
}
