/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/26 16:29

* 描述： 世界法则管理器
* 
* 通过反射获取全局继承了IRule的接口的法则类
* 

*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WorldTree
{

    /// <summary>
    /// 世界法则管理器
    /// </summary>
    public class RuleManager : Node
    {
        /// <summary>
        /// 动态监听器节点类型哈希名单
        /// </summary>
        public UnitHashSet<Type> DynamicListenerTypeHash = new UnitHashSet<Type>();

        /// <summary>
        /// 监听法则字典 目标节点类型
        /// </summary>
        /// <remarks>
        /// <para>目标节点类型 法则类型 《监听类型,监听法则》</para> 
        /// <para>这个是真正可以被使用的</para> 
        /// </remarks>
        public UnitDictionary<Type, Dictionary<Type, RuleGroup>> TargetRuleDictionary = new UnitDictionary<Type, Dictionary<Type, RuleGroup>>();

        /// <summary>
        ///监听法则字典 监听器类型
        /// </summary>
        /// <remarks> 
        /// <para>监听类型 法则类型 《目标节点类型,监听法则》</para> 
        /// <para>这个是用来查询关系的</para> 
        /// </remarks>
        public UnitDictionary<Type, Dictionary<Type, RuleGroup>> ListenerRuleDictionary = new UnitDictionary<Type, Dictionary<Type, RuleGroup>>();

        /// <summary>
        /// 法则字典
        /// </summary>
        /// <remarks> 法则类型《节点类型,法则》</remarks>
        private UnitDictionary<Type, RuleGroup> RuleDictionary = new UnitDictionary<Type, RuleGroup>();

        public RuleManager() : base()
        {
            Initialize();
        }

        private void Initialize()
        {
            var RuleTypeList = FindTypesIsInterface(typeof(IRule));
            //将按照法则类名进行排序，规范执行顺序

            RuleTypeList.Sort((Rule1, Rule2) => Rule1.Name.CompareTo(Rule2.Name));

            List<IListenerRule> ListenerRuleList = new List<IListenerRule>();//只指定了法则的监听器法则

            foreach (var RuleType in RuleTypeList)//遍历实现接口的类
            {
                //实例化法则类
                IRule rule = Activator.CreateInstance(RuleType, true) as IRule;

                if (rule is IListenerRule)
                {
                    var listenerRule = rule as IListenerRule;//转换为监听法则

                    if (listenerRule.TargetNodeType == typeof(INode) && listenerRule.TargetRuleType != typeof(IRule))
                    {
                        ListenerRuleList.Add(listenerRule); //约束了法则
                    }
                    else
                    {
                        //指定了节点，或 动态指定节点

                        var ListenerRuleGroup = ListenerRuleDictionary.GetValue(listenerRule.NodeType).GetValue(listenerRule.RuleType);
                        ListenerRuleGroup.GetValue(listenerRule.TargetNodeType).Add(listenerRule);
                        ListenerRuleGroup.RuleType = listenerRule.RuleType;

                        var TargetRuleGroup = TargetRuleDictionary.GetValue(listenerRule.TargetNodeType).GetValue(listenerRule.RuleType);
                        TargetRuleGroup.GetValue(listenerRule.NodeType).Add(listenerRule);
                        TargetRuleGroup.RuleType = listenerRule.RuleType;

                        //动态监听器判断
                        if (listenerRule.TargetNodeType == typeof(INode) && listenerRule.TargetRuleType == typeof(IRule))
                        {
                            if (!DynamicListenerTypeHash.Contains(listenerRule.NodeType)) DynamicListenerTypeHash.Add(listenerRule.NodeType);
                        }

                    }
                }
                else
                {
                    var group = RuleDictionary.GetValue(rule.RuleType);
                    group.GetValue(rule.NodeType).Add(rule);
                    group.RuleType = rule.RuleType;
                }
            }


            foreach (IListenerRule listenerRule in ListenerRuleList)//查询法则对应节点 
            {
                if (RuleDictionary.TryGetValue(listenerRule.TargetRuleType, out RuleGroup ruleGroup))
                {
                    foreach (var ruleList in ruleGroup.Values)
                    {
                        foreach (var rule in ruleList)
                        {
                            var ListenerGroup = ListenerRuleDictionary.GetValue(listenerRule.NodeType).GetValue(listenerRule.RuleType);
                            ListenerGroup.GetValue(rule.NodeType).Add(listenerRule);
                            ListenerGroup.RuleType = listenerRule.RuleType;

                            var TargetGroup = TargetRuleDictionary.GetValue(rule.NodeType).GetValue(listenerRule.RuleType);
                            TargetGroup.GetValue(listenerRule.NodeType).Add(listenerRule);
                            TargetGroup.RuleType = listenerRule.RuleType;
                        }

                    }
                }
            }


        }


        #region 多态
        private void SetPolymorphicListenerRule(Type NodeType)
        {
            foreach (var RuleGroupDictionary in ListenerRuleDictionary)
            {

            }
        
        }


        /// <summary>
        /// 补齐多态法则
        /// </summary>
        private void SetPolymorphicRule(Type NodeType)
        {
            foreach (var ruleGroup in RuleDictionary.Values)//遍历法则字典
            {
                if (!ruleGroup.TryGetValue(NodeType, out List<IRule> ruleList))
                {
                    Type typeKey = NodeType;
                    bool isRule = false;
                    while (!isRule && typeKey != null && typeKey != typeof(object))
                    {
                        //判断类型是否有法则列表
                        isRule = ruleGroup.TryGetValue(typeKey, out ruleList);
                        if (!isRule)//不存在则向上找父类
                        {
                            typeKey = typeKey.BaseType;
                        }
                    }
                    if (isRule)
                    {
                        ruleGroup.Add(NodeType, ruleList);
                    }
                }
            }
        }

        #endregion

        #region 监听目标法则组

        /// <summary>
        /// 获取监听目标法则组
        /// </summary>
        public bool TryGetTargetRuleGroup<LR>(Type targetType, out RuleGroup ruleGroup)
            where LR : IListenerRule
        {
            return TryGetTargetRuleGroup(typeof(LR), targetType, out ruleGroup);
        }

        /// <summary>
        /// 获取监听目标法则组
        /// </summary>
        public bool TryGetTargetRuleGroup(Type ruleType, Type targetType, out RuleGroup ruleGroup)
        {
            if (TargetRuleDictionary.TryGetValue(targetType, out var ruleGroupDictionary))
            {
                return ruleGroupDictionary.TryGetValue(ruleType, out ruleGroup);
            }
            ruleGroup = null;
            return false;
        }

        /// <summary>
        /// 获取监听目标法则列表
        /// </summary>
        public bool TryGetTargetRuleList<LR>(Type targetType, Type listenerType, out List<IRule> ruleList)
            where LR : IListenerRule
        {
            if (TargetRuleDictionary.TryGetValue(targetType, out var ruleGroupDictionary))
            {
                if (ruleGroupDictionary.TryGetValue(typeof(LR), out var ruleGroup))
                {
                    return ruleGroup.TryGetValue(listenerType, out ruleList);
                }
            }
            ruleList = null;
            return false;
        }
        #endregion

        #region  监听法则组

        /// <summary>
        /// 获取监听法则组
        /// </summary>
        public bool TryGetListenerRuleGroup<LR>(Type listenerType, out RuleGroup ruleGroup)
            where LR : IListenerRule
        {
            if (ListenerRuleDictionary.TryGetValue(listenerType, out var ruleGroupDictionary))
            {
                return ruleGroupDictionary.TryGetValue(typeof(LR), out ruleGroup);
            }
            ruleGroup = null;
            return false;
        }

        /// <summary>
        /// 获取监听法则
        /// </summary>
        public bool TryGetListenerRuleList<LR>(Type listenerType, Type targetType, out List<IRule> ruleList)
            where LR : IListenerRule
        {
            if (ListenerRuleDictionary.TryGetValue(listenerType, out var ruleGroupDictionary))
            {
                if (ruleGroupDictionary.TryGetValue(typeof(LR), out var ruleGroup))
                {
                    return ruleGroup.TryGetValue(targetType, out ruleList);
                }
            }
            ruleList = null;
            return false;
        }
        #endregion




        #region  法则组
        /// <summary>
        /// 获取法则组
        /// </summary>
        public RuleGroup GetRuleGroup<R>() where R : IRule => GetRuleGroup(typeof(R));

        /// <summary>
        /// 获取法则组
        /// </summary>
        public RuleGroup GetRuleGroup(Type ruleType)
        {
            TryGetRuleGroup(ruleType, out RuleGroup ruleGroup);
            return ruleGroup;
        }

        /// <summary>
        /// 获取法则组
        /// </summary>
        public bool TryGetRuleGroup<R>(out RuleGroup ruleGroup)
         where R : IRule
        {
            return TryGetRuleGroup(typeof(R), out ruleGroup);
        }

        /// <summary>
        /// 获取法则组
        /// </summary>
        public bool TryGetRuleGroup(Type Interface, out RuleGroup ruleGroup)
        {
            return RuleDictionary.TryGetValue(Interface, out ruleGroup);
        }

        #endregion

        #region  法则列表

        /// <summary>
        /// 获取单类型法则列表
        /// </summary>
        public List<IRule> GetRuleList<R>(Type type)
         where R : IRule
        {
            if (RuleDictionary.TryGetValue(typeof(R), out RuleGroup ruleGroup))
            {
                if (ruleGroup.TryGetValue(type, out List<IRule> ruleList))
                {
                    return ruleList;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取单类型法则列表
        /// </summary>
        public bool TryGetRuleList(Type nodeType, Type ruleType, out List<IRule> ruleList)
        {
            if (RuleDictionary.TryGetValue(ruleType, out RuleGroup ruleGroup))
            {
                return ruleGroup.TryGetValue(nodeType, out ruleList);
            }
            else
            {
                ruleList = null;
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            RuleDictionary.Clear();
            ListenerRuleDictionary.Clear();
            TargetRuleDictionary.Clear();
            IsRecycle = true;
            IsDisposed = true;
        }

        /// <summary>
        /// 查找继承了接口的类型
        /// </summary>
        private static List<Type> FindTypesIsInterface(Type Interface)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(T => T.GetInterfaces().Contains(Interface) && !T.IsAbstract)).ToList();
        }

    }
}
