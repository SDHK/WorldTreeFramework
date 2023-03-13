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
        public UnitDictionary<Type, Dictionary<Type, RuleGroup>> TargetRuleListenerGroupDictionary = new UnitDictionary<Type, Dictionary<Type, RuleGroup>>();

        /// <summary>
        ///监听法则字典 监听器类型
        /// </summary>
        /// <remarks> 
        /// <para>监听类型 法则类型 《目标节点类型,监听法则》</para> 
        /// <para>这个是用来查询关系的</para> 
        /// </remarks>
        public UnitDictionary<Type, Dictionary<Type, RuleGroup>> ListenerRuleTargetGroupDictionary = new UnitDictionary<Type, Dictionary<Type, RuleGroup>>();

        /// <summary>
        /// 法则字典
        /// </summary>
        /// <remarks> 法则类型《节点类型,法则》</remarks>
        private UnitDictionary<Type, RuleGroup> RuleGroupDictionary = new UnitDictionary<Type, RuleGroup>();

        /// <summary>
        /// 节点法则字典
        /// </summary>
        /// <remarks>记录节点拥有的法则类型</remarks>
        private Dictionary<Type, HashSet<Type>> NodeTypeRulesDictionary = new Dictionary<Type, HashSet<Type>>();

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

                        var ListenerRuleGroup = ListenerRuleTargetGroupDictionary.GetValue(listenerRule.NodeType).GetValue(listenerRule.RuleType);
                        ListenerRuleGroup.GetValue(listenerRule.TargetNodeType).Add(listenerRule);
                        ListenerRuleGroup.RuleType = listenerRule.RuleType;

                        var TargetRuleGroup = TargetRuleListenerGroupDictionary.GetValue(listenerRule.TargetNodeType).GetValue(listenerRule.RuleType);
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
                    var group = RuleGroupDictionary.GetValue(rule.RuleType);
                    group.GetValue(rule.NodeType).Add(rule);
                    group.RuleType = rule.RuleType;

                    NodeTypeRulesDictionary.GetValue(rule.NodeType).Add(rule.RuleType);
                }
            }


            foreach (IListenerRule listenerRule in ListenerRuleList)//查询法则对应节点 
            {
                if (RuleGroupDictionary.TryGetValue(listenerRule.TargetRuleType, out RuleGroup ruleGroup))
                {
                    foreach (var ruleList in ruleGroup.Values)
                    {
                        foreach (var rule in ruleList)
                        {
                            var ListenerGroup = ListenerRuleTargetGroupDictionary.GetValue(listenerRule.NodeType).GetValue(listenerRule.RuleType);
                            ListenerGroup.GetValue(rule.NodeType).Add(listenerRule);
                            ListenerGroup.RuleType = listenerRule.RuleType;

                            var TargetGroup = TargetRuleListenerGroupDictionary.GetValue(rule.NodeType).GetValue(listenerRule.RuleType);
                            TargetGroup.GetValue(listenerRule.NodeType).Add(listenerRule);
                            TargetGroup.RuleType = listenerRule.RuleType;
                        }

                    }
                }
            }


        }



        #region 法则多态

        /// <summary>
        /// 补齐监听器的多态法则
        /// </summary>
        /// <remarks>
        /// <para>这个功能设定为只在对象池建立时执行一次</para>
        /// <para>只多态监听器，不会多态监听目标 </para>
        /// </remarks>
        public void SetPolymorphicListenerRule(Type listenerNodeType)
        {
            //判断如果没有这样的监听器
            if (!ListenerRuleTargetGroupDictionary.ContainsKey(listenerNodeType))
            {
                //监听器父类类型键值
                Type listenerBaseTypeKey = listenerNodeType.BaseType;
                //父类法则查询标记
                bool isBaseRule = false;

                //法则类型 《目标节点类型,监听法则》
                Dictionary<Type, RuleGroup> RuleType_TargerGroupDictionary = null;

                //在没有找到法则的时候向上查找父类法则
                while (!isBaseRule && listenerBaseTypeKey != null && listenerBaseTypeKey != typeof(object))
                {
                    //判断类型是否有法则列表
                    isBaseRule = ListenerRuleTargetGroupDictionary.TryGetValue(listenerBaseTypeKey, out RuleType_TargerGroupDictionary);
                    if (!isBaseRule)//不存在则向上找父类
                    {
                        listenerBaseTypeKey = listenerBaseTypeKey.BaseType;
                    }
                }

                if (isBaseRule)//如果找到了法则
                {
                    //动态监听法则多态
                    if (DynamicListenerTypeHash.Contains(listenerBaseTypeKey))
                    {
                        DynamicListenerTypeHash.Add(listenerNodeType);
                    }

                    //监听器为主的字典添加相应的父类法则
                    ListenerRuleTargetGroupDictionary.Add(listenerNodeType, RuleType_TargerGroupDictionary);

                    //遍历这个被多态的法则字典

                    //K:法则类型 , V:《目标节点类型,监听法则》
                    foreach (var RuleType_TargetGroupKV in RuleType_TargerGroupDictionary)
                    {
                        //K:目标节点类型 , V:监听法则列表
                        foreach (var TargetType_RuleListKV in RuleType_TargetGroupKV.Value)
                        {
                            //目标类型为主的字典， 进行目标类型查找
                            if (TargetRuleListenerGroupDictionary.TryGetValue(TargetType_RuleListKV.Key, out var RuleType_ListenerGroupDictionary))
                            {
                                //法则类型查找
                                if (RuleType_ListenerGroupDictionary.TryGetValue(RuleType_TargetGroupKV.Key, out var ListenerGroup))
                                {
                                    //监听器存在的父类型 查找 法则列表
                                    if (ListenerGroup.TryGetValue(listenerBaseTypeKey, out var ruleList))
                                    {
                                        //将父类的 法则列表，添加进 没有法则的 监听器类型。
                                        ListenerGroup.TryAdd(listenerNodeType, ruleList);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// 补齐节点多态法则
        /// </summary>
        /// <remarks>这个功能设定为只在对象池建立时执行一次</remarks>
        public void SetPolymorphicRule(Type NodeType)
        {
            //拿到节点类型的法则哈希表
            HashSet<Type> ruleTypeHash = NodeTypeRulesDictionary.GetValue(NodeType);

            //开始遍历查询父类型法则
            Type BaseTypeKey = NodeType.BaseType;
            while (BaseTypeKey != null && BaseTypeKey != typeof(object))
            {
                //尝试获取父类型法则
                if (NodeTypeRulesDictionary.TryGetValue(BaseTypeKey, out var BaseRuleHash))
                {
                    //遍历父类型法则
                    foreach (var ruleType in BaseRuleHash)
                    {
                        //法则不存在，则添加到节点的哈希表里
                        if (!ruleTypeHash.Contains(ruleType))
                        {
                            ruleTypeHash.Add(ruleType);
                            //法则字典的补充
                            if (RuleGroupDictionary.TryGetValue(ruleType, out var RuleGroup))
                            {
                                //获取父类型法则列表
                                if (RuleGroup.TryGetValue(BaseTypeKey, out var ruleList))
                                {
                                    //法则列表添加进节点类型
                                    RuleGroup.TryAdd(NodeType, ruleList);
                                }
                            }
                        }
                    }
                }
                BaseTypeKey = BaseTypeKey.BaseType;
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
            if (TargetRuleListenerGroupDictionary.TryGetValue(targetType, out var ruleGroupDictionary))
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
            if (TargetRuleListenerGroupDictionary.TryGetValue(targetType, out var ruleGroupDictionary))
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
            if (ListenerRuleTargetGroupDictionary.TryGetValue(listenerType, out var ruleGroupDictionary))
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
            if (ListenerRuleTargetGroupDictionary.TryGetValue(listenerType, out var ruleGroupDictionary))
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
            return RuleGroupDictionary.TryGetValue(Interface, out ruleGroup);
        }

        #endregion

        #region  法则列表

        /// <summary>
        /// 获取单类型法则列表
        /// </summary>
        public List<IRule> GetRuleList<R>(Type type)
         where R : IRule
        {
            if (RuleGroupDictionary.TryGetValue(typeof(R), out RuleGroup ruleGroup))
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
            if (RuleGroupDictionary.TryGetValue(ruleType, out RuleGroup ruleGroup))
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
            RuleGroupDictionary.Clear();
            ListenerRuleTargetGroupDictionary.Clear();
            TargetRuleListenerGroupDictionary.Clear();
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
