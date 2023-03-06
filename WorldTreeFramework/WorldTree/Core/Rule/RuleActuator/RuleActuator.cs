/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 法则执行器
* 
* 用于执行拥有指定法则的节点

*/

namespace WorldTree
{
    /// <summary>
    /// 法则执行器
    /// </summary>
    public partial class RuleActuator : Node
    {
        /// <summary>
        /// 法则集合
        /// </summary>
        public RuleGroup ruleGroup;

        /// <summary>
        /// 动态节点队列
        /// </summary>
        public DynamicNodeQueue nodeQueue;

        public override string ToString()
        {
            return $"RuleActuator : {ruleGroup?.RuleType}";
        }

        /// <summary>
        /// 当前数量
        /// </summary>
        public int Count => nodeQueue.Count;

        /// <summary>
        /// 添加实体
        /// </summary>
        public void Enqueue(Node node)
        {
            nodeQueue.Enqueue(node);
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        public void Remove(Node node)
        {
            nodeQueue.Remove(node);
        }
        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            nodeQueue.Clear();
        }

        /// <summary>
        /// 出列
        /// </summary>
        public Node Dequeue()
        {
            return nodeQueue.Dequeue();
        }

        /// <summary>
        /// 尝试出列
        /// </summary>
        public bool TryDequeue(out Node node)
        {
            return nodeQueue.TryDequeue(out node);
        }


    }

    class RuleActuatorAddSystem : AddRule<RuleActuator>
    {
        public override void OnEvent(RuleActuator self)
        {
            if (self.thisPool != null)
            {
                self.AddComponent(out self.nodeQueue);
            }

        }
    }
}
