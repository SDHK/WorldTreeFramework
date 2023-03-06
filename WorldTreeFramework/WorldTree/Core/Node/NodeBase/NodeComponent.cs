
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 11:37

* 描述： 组件节点
* 
* 用节点类型作为键值，因此同种类型的组件只能一个

*/

using System;

namespace WorldTree
{
    public abstract partial class Node
    {
        /// <summary>
        /// 组件标记
        /// </summary>
        public bool isComponent;

        /// <summary>
        /// 组件节点
        /// </summary>
        public UnitDictionary<Type, Node> m_Components;

        /// <summary>
        /// 组件节点
        /// </summary>
        public UnitDictionary<Type, Node> Components
        {
            get
            {
                if (m_Components == null)
                {
                    m_Components = Root.PoolGet<UnitDictionary<Type, Node>>();
                }
                return m_Components;
            }

            set { m_Components = value; }
        }
    }
}
