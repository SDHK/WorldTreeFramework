/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 节点对象池管理器。
* 
*/
using System;

namespace WorldTree
{
    /// <summary>
    /// 节点对象池管理器
    /// </summary>
    public class NodePoolManager : PoolManagerBase<NodePool>,ComponentOf<WorldTreeCore>
    {
        /// <summary>
        /// 尝试获取节点
        /// </summary>
        public bool TryGet(long type, out INode node)
        {
            if (TryGet(type, out object obj))
            {
                node = obj as INode;
                return true;
            }
            else
            {
                node = null;
                return false;
            }
        }
    }

    class NodePoolManagerAddRule : AddRule<NodePoolManager>
    {
        protected override void Execute(NodePoolManager self)
        {
            
        }
    }
}
