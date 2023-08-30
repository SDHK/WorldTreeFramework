
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/30 20:29

* 描述： 全局监听器法则执行器

*/

namespace WorldTree
{
    /// <summary>
    /// 全局监听器法则执行器
    /// </summary>
    public class ListenerRuleActuator : RuleActuatorBase, ICoreNode, IRuleActuator<IRule>, ComponentOf<GlobalRuleActuatorManager>
        , AsRule<IAwakeRule<RuleGroup>>
    {
        public override string ToString()
        {
            return $"ListenerRuleActuator : {ruleGroup?.RuleType.HashCore64ToType()}";
        }
    }

    class ListenerRuleActuatorAwakeRule : AwakeRule<ListenerRuleActuator, RuleGroup>
    {
        protected override void OnEvent(ListenerRuleActuator self, RuleGroup arg1)
        {
            self.ruleGroup = arg1;
        }
    }


    public class ListenerRuleActuator1 :CoreNode  , IRuleActuator<IRule>, ComponentOf<GlobalRuleActuatorManager>
        , AsRule<IAwakeRule<RuleGroup>>
    {
        /// <summary>
        /// 单法则集合
        /// </summary>
        public RuleGroup ruleGroup;
        /// <summary>
        /// 节点id队列
        /// </summary>
        public TreeQueue<long> idQueue;

        /// <summary>
        /// 节点字典
        /// </summary>
        public TreeDictionary<long, INode> nodeDictionary;

        /// <summary>
        /// 节点id被移除的次数
        /// </summary>
        public TreeDictionary<long, int> removeIdDictionary;

        /// <summary>
        /// 动态的遍历数量
        /// </summary>
        /// <remarks>当遍历时移除后，减少数量</remarks>
        public int traversalCount;

        public override string ToString()
        {
            return $"ListenerRuleActuator : {ruleGroup?.RuleType.HashCore64ToType()}";
        }

        public int RefreshTraversalCount()
        {
            return traversalCount = idQueue is null ? 0 : idQueue.Count;
        }

        public bool TryGetNext(out INode node, out RuleList ruleList)
        {
            throw new System.NotImplementedException();
        }


        public void Clear()
        {
            throw new System.NotImplementedException();
        }

      
        public void Remove(long id)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(INode node)
        {
            throw new System.NotImplementedException();
        }

       

        public bool TryAdd(INode node)
        {
            throw new System.NotImplementedException();
        }

        public bool TryAdd(INode node, RuleList ruleList)
        {
            throw new System.NotImplementedException();
        }

        public bool TryAddReferenced(INode node)
        {
            throw new System.NotImplementedException();
        }

        public bool TryAddReferenced(INode node, RuleList ruleList)
        {
            throw new System.NotImplementedException();
        }

      
    }

}
