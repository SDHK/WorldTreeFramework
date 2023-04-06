/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 16:21

* 描述： 动态节点队列
* 
* 主要为了可以按照顺序遍历的同时可随机移除内容

*/

namespace WorldTree
{
    /// <summary>
    /// 动态节点队列
    /// </summary>
    public class DynamicNodeQueue : Node, IAwake, ComponentOf<INode>, ChildOf<INode>
    {
        /// <summary>
        /// 节点id队列
        /// </summary>
        public TreeQueue<long> idQueue;

        /// <summary>
        /// 节点id被移除的次数
        /// </summary>
        public TreeDictionary<long, int> removeIdDictionary;

        /// <summary>
        /// 节点名单
        /// </summary>
        public TreeDictionary<long, INode> nodeDictionary;

        /// <summary>
        /// 当前队列数量
        /// </summary>
        public int Count => nodeDictionary is null ? 0 : nodeDictionary.Count;

        /// <summary>
        /// 动态的遍历数量
        /// </summary>
        /// <remarks>当遍历时移除后，在发生抵消的时候减少数量</remarks>
        public int traversalCount;
    }
}
