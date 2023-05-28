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
    public abstract class RuleActuatorBase : Node, ChildOf<INode>, IRuleActuator
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



        /// <summary>
        /// 动态的遍历数量
        /// </summary>
        /// <remarks>当遍历时移除后，在发生抵消的时候减少数量</remarks>
        public int traversalCount;

        public override string ToString()
        {
            return $"RuleActuator : {ruleGroup.RuleType}";
        }

        /// <summary>
        /// 刷新动态遍历数量
        /// </summary>
        public  void RefreshTraversalCount()
        {
            traversalCount = idQueue is null ? 0 : idQueue.Count;
        }

        public void Enqueue(long id)
        {
            if (nodeDictionary.ContainsKey(id)) return;
            idQueue.Enqueue(id);
        }
        public bool TryDequeue(out INode node, out RuleGroup ruleGroup)
        {
            //尝试获取一个id
            if (idQueue.TryDequeue(out long id))
            {
                //假如id被回收了
                while (removeIdDictionary.TryGetValue(id, out int count))
                {
                    //回收次数抵消
                    removeIdDictionary[id] = --count;
                    if (traversalCount > 0) traversalCount--;

                    //次数为0时删除id
                    if (count == 0) removeIdDictionary.Remove(id);

                    //获取下一个id
                    if (!idQueue.TryDequeue(out id))
                    {
                        //假如队列空了,则直接返回退出
                        ruleGroup = this.ruleGroup;
                        node = null;
                        return false;
                    }
                }

                //此时的id是正常id
                if (ruleGroupDictionary == null || !ruleGroupDictionary.TryGetValue(id, out ruleGroup))
                {
                    ruleGroup = this.ruleGroup;
                }
                return nodeDictionary.TryGetValue(id, out node);
            }
            ruleGroup = this.ruleGroup;
            node = null;
            return false;
        }


        public void AddReferenced(INode node)
        {
            if (nodeDictionary.ContainsKey(node.Id)) return;
            this.Referenced(node);
            idQueue.Enqueue(node.Id);
            nodeDictionary.Add(node.Id, node);
        }
        public void Add(INode node)
        {
            if (nodeDictionary.ContainsKey(node.Id)) return;
            idQueue.Enqueue(node.Id);
            nodeDictionary.Add(node.Id, node);
        }

        public void AddReferenced(INode node,RuleGroup ruleGroup)
        {
            if (nodeDictionary.ContainsKey(node.Id)) return;
            this.Referenced(node);
            idQueue.Enqueue(node.Id);
            nodeDictionary.Add(node.Id, node);
            ruleGroupDictionary.Add(node.Id, ruleGroup);
        }

        public void Add(INode node, RuleGroup ruleGroup)
        {
            if (nodeDictionary.ContainsKey(node.Id)) return;
            idQueue.Enqueue(node.Id);
            nodeDictionary.Add(node.Id, node);
            ruleGroupDictionary.Add(node.Id, ruleGroup);
        }



        public void Remove(INode node)
        {

            if (nodeDictionary.ContainsKey(node.Id))
            {
                nodeDictionary.Remove(node.Id);
                ruleGroupDictionary.Remove(node.Id);
                this.DeReferenced(node);
                //累计强制移除的节点id
                if (removeIdDictionary.TryGetValue(node.Id, out var count))
                {
                    removeIdDictionary[node.Id] = count + 1;
                }
                else
                {
                    removeIdDictionary.Add(node.Id, 1);
                }
            }
        }
        public void Clear()
        {
            idQueue.Clear();
            ruleGroupDictionary.Clear();
            nodeDictionary.Clear();
            removeIdDictionary.Clear();
        }
    }


    /// <summary>
    /// 单法则执行器
    /// </summary>
    public partial class RuleActuator : RuleActuatorBase, IRuleActuator<IRule>
    {
       


      
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
                            ruleActuator.Add(node.Value);
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
            ((RuleActuator)self).AddReferenced(node);
        }

        /// <summary>
        /// 节点入列
        /// </summary>
        public static void Enqueue<R, N>(this IRuleActuator<R> self, N node)
            where R : IRule
            where N : class, INode, AsRule<R>
        {
            ((RuleActuator)self).Add(node);
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
            ((RuleActuator)self).AddReferenced(node.AddComponent(out ruleNode));
        }

    }
}
