/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 法则执行器
* 
* 用于执行拥有指定法则的节点
* 可用于代替委托事件
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 法则执行器 接口基类
    /// </summary>
    public interface IRuleActuator : INode
    {
        /// <summary>
        /// 尝试获得法则集合
        /// </summary>
        public bool TryGetNodeRuleGroup(INode node, out RuleGroup ruleGroup);

    }


    /// <summary>
    /// 法则执行器 逆变泛型接口
    /// </summary>
    /// <typeparam name="T">法则类型</typeparam>
    /// <remarks>
    /// <para>主要通过法则类型逆变提示可填写参数</para>
    /// <para> RuleActuator 是没有泛型的实例，所以执行参数可能填错</para>
    /// </remarks>
    public interface IRuleActuator<in T> : IRuleActuator where T : IRule { }


    /// <summary>
    /// 法则执行器基类
    /// </summary>
    public abstract class RuleActuatorBase : DynamicNodeQueue, ChildOf<INode>, IRuleActuator
    {
        public abstract bool TryGetNodeRuleGroup(INode node, out RuleGroup ruleGroup);


    }


    /// <summary>
    /// 单法则执行器
    /// </summary>
    public partial class RuleActuator : RuleActuatorBase, IRuleActuator<IRule>
    {
        /// <summary>
        /// 默认法则集合
        /// </summary>
        public RuleGroup ruleGroup;

        /// <summary>
        /// 节点id队列
        /// </summary>
        public TreeQueue<long> idQueue;

        /// <summary>
        /// 节点id被移除的次数
        /// </summary>
        public TreeDictionary<long, int> removeIdDictionary;

        /// <summary>
        /// 节点字典
        /// </summary>
        public TreeDictionary<long, INode> nodeDictionary;

        /// <summary>
        /// 法则集合字典
        /// </summary>
        public TreeDictionary<long, RuleGroup> ruleGroupDictionary;


        public override string ToString()
        {
            return $"RuleActuator : {ruleGroup.RuleType}";
        }

        public override bool TryGetNodeRuleGroup(INode node, out RuleGroup ruleGroup)
        {
            if (ruleGroupDictionary != null && ruleGroupDictionary.TryGetValue(node.Id, out ruleGroup))
            {
                return true;
            }
            else
            {
                ruleGroup = this.ruleGroup;
                return this.ruleGroup != null;
            }
        }
    }





    /// <summary>
    /// 单法则执行器
    /// </summary>
    public class RuleActuator<R> : RuleActuator
      where R : IRule
    {

    }




    class RuleActuatorRemoveRule : RemoveRule<RuleActuator>
    {
        public override void OnEvent(RuleActuator self)
        {
            self.ruleGroup = null;
        }
    }


    public static class RuleActuatorRule
    {
        /// <summary>
        /// 执行器初始化全局填装节点
        /// </summary>
        public static RuleActuator LoadGlobalNode<R>(this RuleActuator ruleActuator)
        where R : IRule
        {
            var ruleType = typeof(R);

            if (ruleActuator.Core.RuleManager.TryGetRuleGroup(ruleType, out ruleActuator.ruleGroup))
            {
                ruleActuator.Clear();
                foreach (var item in ruleActuator.ruleGroup)
                {
                    if (ruleActuator.Core.NodePoolManager.m_Pools.TryGetValue(item.Key, out NodePool pool))
                    {
                        foreach (var node in pool.Nodes)
                        {
                            ruleActuator.Enqueue(node.Value);
                        }
                    }
                }
            }
            return ruleActuator;
        }

        /// <summary>
        /// 节点入列并建立引用关系
        /// </summary>
        public static void EnqueueReferenced<R, N>(this IRuleActuator<R> self, N node)
            where R : IRule
            where N : class, INode, AsRule<R>
        {
            ((RuleActuator)self).EnqueueReferenced(node);
        }

        /// <summary>
        /// 节点入列
        /// </summary>
        public static void Enqueue<R, N>(this IRuleActuator<R> self, N node)
            where R : IRule
            where N : class, INode, AsRule<R>
        {
            ((RuleActuator)self).Enqueue(node);
        }


        /// <summary>
        /// 移除节点
        /// </summary>
        public static void Remove<R, N>(this IRuleActuator<R> self, N node)
            where R : IRule
            where N : class, INode, AsRule<R>
        {
            ((RuleActuator)self).Remove(node);
        }



        /// <summary>
        /// 添加法则节点并建立引用关系
        /// </summary>
        public static void Add<N, R, RN>(this IRuleActuator<R> self, N node, out RN ruleNode)
            where N : class, INode
            where R : IRule
            where RN : class, INode, ComponentOf<N>, AsRule<R>
        {
            ((RuleActuator)self).EnqueueReferenced(node.AddComponent(out ruleNode));
        }

    }
}
