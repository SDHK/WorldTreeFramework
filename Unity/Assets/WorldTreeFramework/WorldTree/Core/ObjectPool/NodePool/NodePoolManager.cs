/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 节点对象池管理器。
* 
*/
using System;

namespace WorldTree
{

    public static partial class NodeRule
    {
        /// <summary>
        /// 从池中获取节点对象
        /// </summary>
        public static T PoolGet<T>(this INode self)
        where T : class, INode
        {
            return self.Core.GetNode<T>();
        }

        /// <summary>
        /// 从池中获取节点对象
        /// </summary>
        public static INode PoolGet(this INode self, long type)
        {
            return self.Core.GetNode(type);
        }
    }


    /// <summary>
    /// 节点对象池管理器
    /// </summary>
    public class NodePoolManager : PoolManagerBase<NodePool>
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
        protected override void OnEvent(NodePoolManager self)
        {
            
        }
    }
}
