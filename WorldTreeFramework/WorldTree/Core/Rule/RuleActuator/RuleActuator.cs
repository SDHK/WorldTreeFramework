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
        /// 系统组
        /// </summary>
        public RuleGroup ruleGroup;

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
        public void Enqueue(Node entity)
        {
            nodeQueue.Enqueue(entity);
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        public void Remove(Node entity)
        {
            nodeQueue.Remove(entity);
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
        public bool TryDequeue(out Node entity)
        {
            return nodeQueue.TryDequeue(out entity);
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
