/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/26 16:29

* 描述： 世界法则管理器
* 
* 通过反射获取全局继承了IRule的接口的法则类
* 
* 支持多态：设计目的是可通过继承复用代码，不提倡设计复杂的多重继承，能拆分写的功能就拆分写。
* 支持泛型：设计目的是更进一步复用代码。
* 

*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WorldTree
{

    public static partial class NodeRule
    {
        /// <summary>
        /// 获取法则集合
        /// </summary>
        public static IRuleGroup<T> GetRuleGroup<T>(this INode self)
        where T : IRule
        {
            return self.Core.RuleManager.GetRuleGroup<T>();
        }


        /// <summary>
        /// 获取法则集合
        /// </summary>
        public static RuleGroup GetRuleGroup(this INode self, Type type)
        {
            return self.Core.RuleManager.GetRuleGroup(type);
        }

        /// <summary>
        /// 获取单类型法则列表
        /// </summary>
        public static IRuleList<R> GetRuleList<R>(this INode self, Type type)
        where R : IRule
        {
            return self.Core.RuleManager.GetRuleList<R>(type);
        }
    }


    /// <summary>
    /// 世界法则管理器
    /// </summary>
    public class RuleManager : Node, ComponentOf<WorldTreeCore>
    {
        /// <summary>
        /// 动态监听器节点类型哈希名单
        /// </summary>
        public UnitHashSet<Type> DynamicListenerTypeHash = new UnitHashSet<Type>();

        /// <summary>
        /// 指定法则的 监听器法则哈希表 字典
        /// </summary>
        public UnitDictionary<Type, UnitHashSet<IListenerRule>> TargetRuleListenerRuleHashDictionary = new UnitDictionary<Type, UnitHashSet<IListenerRule>>();

        /// <summary>
        /// 泛型节点泛型法则哈希表字典
        /// </summary>
        /// <remarks>泛型节点类型，泛型法则类型哈希表</remarks>
        public UnitDictionary<Type, UnitHashSet<Type>> GenericRuleTypeHashDictionary = new UnitDictionary<Type, UnitHashSet<Type>>();

        /// <summary>
        /// 监听法则字典 目标节点类型
        /// </summary>
        /// <remarks>
        /// <para>目标节点类型 法则类型 《监听类型,监听法则》</para> 
        /// <para>这个是真正可以被使用的</para> 
        /// </remarks>
        public UnitDictionary<Type, Dictionary<Type, RuleGroup>> TargetRuleListenerGroupDictionary = new UnitDictionary<Type, Dictionary<Type, RuleGroup>>();

        /// <summary>
        /// 监听法则字典 监听器类型
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
        public UnitDictionary<Type, RuleGroup> RuleGroupDictionary = new UnitDictionary<Type, RuleGroup>();

        /// <summary>
        /// 节点法则字典
        /// </summary>
        /// <remarks>记录节点拥有的法则类型，用于法则多态化的查询</remarks>
        public Dictionary<Type, HashSet<Type>> NodeTypeRulesDictionary = new Dictionary<Type, HashSet<Type>>();

        public RuleManager()
        {
            this.Awake();
        }

        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            this.Destroy();
        }
    }


    public static class RuleManagerRule
    {
        public static void Awake(this RuleManager self)
        {
            self.Type = self.GetType();

            //反射获取全局继承IRule的法则类型列表
            var RuleTypeList = self.FindTypesIsInterface(typeof(IRule));

            //将按照法则类名进行排序，规范执行顺序
            RuleTypeList.Sort((Rule1, Rule2) => Rule1.Name.CompareTo(Rule2.Name));

            foreach (var RuleType in RuleTypeList)//遍历类型列表
            {
                self.AddRuleType(RuleType);
            }
        }

        public static void Destroy(this RuleManager self)
        {
            self.RuleGroupDictionary.Clear();
            self.NodeTypeRulesDictionary.Clear();

            self.DynamicListenerTypeHash.Clear();
            self.GenericRuleTypeHashDictionary.Clear();
            self.TargetRuleListenerRuleHashDictionary.Clear();

            self.TargetRuleListenerGroupDictionary.Clear();
            self.ListenerRuleTargetGroupDictionary.Clear();

            self.IsRecycle = true;
            self.IsDisposed = true;
        }


        #region 添加法则

        /// <summary>
        /// 查找继承了接口的类型
        /// </summary>
        private static List<Type> FindTypesIsInterface(this RuleManager self, Type Interface)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(T => T.GetInterfaces().Contains(Interface) && !T.IsAbstract)).ToList();
        }

        /// <summary>
        /// 添加法则类型
        /// </summary>
        private static void AddRuleType(this RuleManager self, Type RuleType)
        {
            if (RuleType.IsGenericType) //判断法则类型是泛型
            {
                var BaseType = RuleType.BaseType;

                //遍历获取一路查到最底层RuleBase<N,R>
                while (BaseType.GetGenericTypeDefinition() != typeof(RuleBase<,>))
                {
                    BaseType = BaseType.BaseType;
                }
                var GenericArguments = BaseType.GetGenericArguments();
                //RuleBase<N,R> 第一个泛型参数就是法则负责的目标节点
                self.GenericRuleTypeHashDictionary.GetValue(GenericArguments[0].GetGenericTypeDefinition()).Add(RuleType);
            }
            else
            {
                //实例化法则类
                IRule rule = Activator.CreateInstance(RuleType, true) as IRule;
                self.AddRule(rule);
            }
        }

        /// <summary>
        /// 添加法则
        /// </summary>
        private static void AddRule(this RuleManager self, IRule rule)
        {
            if (rule is IListenerRule)
            {
                var listenerRule = rule as IListenerRule;//转换为监听法则
                self.AddListenerRule(listenerRule);
            }
            else
            {
                self.AddNodeRule(rule);
            }
        }

        /// <summary>
        /// 添加监听器法则
        /// </summary>
        private static void AddListenerRule(this RuleManager self, IListenerRule listenerRule)
        {
            if (listenerRule.TargetNodeType == typeof(INode) && listenerRule.TargetRuleType != typeof(IRule))
            {
                //只约束了法则
                self.TargetRuleListenerRuleHashDictionary.GetValue(listenerRule.TargetRuleType).Add(listenerRule);

                //获取 监听法则 目标法则类型，当前的法则组。
                if (self.RuleGroupDictionary.TryGetValue(listenerRule.TargetRuleType, out RuleGroup ruleGroup))
                {
                    foreach (var ruleList in ruleGroup.Values)
                    {
                        foreach (var rule in ruleList)//遍历法则组所有法则
                        {
                            self.DictionaryAddNodeRule(rule.NodeType, listenerRule);
                        }
                    }
                }
            }
            else
            {
                //指定了节点，或 动态指定节点
                self.DictionaryAddNodeRule(listenerRule.TargetNodeType, listenerRule);

                //动态监听器判断
                if (listenerRule.TargetNodeType == typeof(INode) && listenerRule.TargetRuleType == typeof(IRule))
                {
                    if (!self.DynamicListenerTypeHash.Contains(listenerRule.NodeType)) self.DynamicListenerTypeHash.Add(listenerRule.NodeType);
                }

            }
        }

        /// <summary>
        /// 添加节点法则
        /// </summary>
        private static void AddNodeRule(this RuleManager self, IRule rule)
        {
            var group = self.RuleGroupDictionary.GetValue(rule.RuleType);
            var ruleList = group.GetValue(rule.NodeType);
            if (!ruleList.TryAdd(rule)) return;

            group.RuleType = rule.RuleType;

            self.NodeTypeRulesDictionary.GetValue(rule.NodeType).Add(rule.RuleType);

            //监听器法则补齐
            if (self.TargetRuleListenerRuleHashDictionary.TryGetValue(rule.NodeType, out var listenerRuleHash))
            {
                foreach (var listenerRule in listenerRuleHash)
                {
                    self.DictionaryAddNodeRule(rule.NodeType, listenerRule);
                }
            }
        }

        /// <summary>
        /// 字典分组添加监听器法则
        /// </summary>
        private static void DictionaryAddNodeRule(this RuleManager self, Type NodeType, IListenerRule listenerRule)
        {
            var ListenerRuleGroup = self.ListenerRuleTargetGroupDictionary.GetValue(listenerRule.NodeType).GetValue(listenerRule.RuleType);
            ListenerRuleGroup.GetValue(NodeType).TryAdd(listenerRule);
            ListenerRuleGroup.RuleType = listenerRule.RuleType;

            var TargetRuleGroup = self.TargetRuleListenerGroupDictionary.GetValue(NodeType).GetValue(listenerRule.RuleType);
            TargetRuleGroup.GetValue(listenerRule.NodeType).TryAdd(listenerRule);
            TargetRuleGroup.RuleType = listenerRule.RuleType;
        }

        #endregion

        #region 补充法则

        /// <summary>
        /// 补充节点法则功能
        /// </summary>
        /// <remarks>
        /// <para>这个功能设定为只在对象池建立时执行一次</para>
        /// </remarks>
        public static void SupportNodeRule(this RuleManager self, Type NodeType)
        {
            self.SupportGenericNodeRule(NodeType);
            self.SupportPolymorphicListenerRule(NodeType);
            self.SupportPolymorphicRule(NodeType);
        }

        #region 法则泛型

        /// <summary>
        /// 支持泛型节点法则
        /// </summary>
        /// <remarks>
        /// <para>这个功能设定为只在对象池建立时执行一次</para>
        /// <para>将会通过反射查询自身及所有父类是否有泛型</para>
        /// </remarks>
        public static void SupportGenericNodeRule(this RuleManager self, Type NodeType)
        {
            Type Type = NodeType;
            while (Type != null && Type != typeof(IUnitPoolItem) && Type != typeof(object))
            {
                //节点可能会是非泛型，但父类则有泛型的情况，需要多态化所有父类泛型法则
                if (Type.IsGenericType)
                {
                    //获取泛型本体类型
                    Type GenericNodeType = Type.GetGenericTypeDefinition();
                    //获取泛型参数数组
                    Type[] GenericTypes = Type.GetGenericArguments();

                    if (self.GenericRuleTypeHashDictionary.TryGetValue(GenericNodeType, out var RuleTypeHash))
                    {
                        foreach (var RuleType in RuleTypeHash)
                        {
                            //填入对应的泛型参数，实例化泛型监听系统
                            IRule rule = (IRule)Activator.CreateInstance(RuleType.MakeGenericType(GenericTypes));
                            self.AddRule(rule);
                        }
                    }
                }
                Type = Type.BaseType;
            }
        }

        #endregion


        #region 法则多态

        /// <summary>
        /// 支持监听器的多态法则
        /// </summary>
        /// <remarks>
        /// <para>这个功能设定为只在对象池建立时执行一次</para>
        /// <para>只多态监听器，不会多态监听目标 </para>
        /// </remarks>
        public static void SupportPolymorphicListenerRule(this RuleManager self, Type listenerNodeType)
        {
            //判断如果没有这样的监听器
            if (!self.ListenerRuleTargetGroupDictionary.ContainsKey(listenerNodeType))
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
                    isBaseRule = self.ListenerRuleTargetGroupDictionary.TryGetValue(listenerBaseTypeKey, out RuleType_TargerGroupDictionary);
                    if (!isBaseRule)//不存在则向上找父类
                    {
                        listenerBaseTypeKey = listenerBaseTypeKey.BaseType;
                    }
                }

                if (isBaseRule)//如果找到了法则
                {
                    //动态监听法则多态
                    if (self.DynamicListenerTypeHash.Contains(listenerBaseTypeKey))
                    {
                        self.DynamicListenerTypeHash.Add(listenerNodeType);
                    }

                    //监听器为主的字典添加相应的父类法则
                    self.ListenerRuleTargetGroupDictionary.Add(listenerNodeType, RuleType_TargerGroupDictionary);

                    //遍历这个被多态的法则字典

                    //K:法则类型 , V:《目标节点类型,监听法则》
                    foreach (var RuleType_TargetGroupKV in RuleType_TargerGroupDictionary)
                    {
                        //K:目标节点类型 , V:监听法则列表
                        foreach (var TargetType_RuleListKV in RuleType_TargetGroupKV.Value)
                        {
                            //目标类型为主的字典， 进行目标类型查找
                            if (self.TargetRuleListenerGroupDictionary.TryGetValue(TargetType_RuleListKV.Key, out var RuleType_ListenerGroupDictionary))
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
        /// 支持节点多态法则
        /// </summary>
        /// <remarks>这个功能设定为只在对象池建立时执行一次</remarks>
        public static void SupportPolymorphicRule(this RuleManager self, Type NodeType)
        {

            //拿到节点类型的法则哈希表
            HashSet<Type> ruleTypeHash = self.NodeTypeRulesDictionary.GetValue(NodeType);

            //开始遍历查询父类型法则
            Type BaseTypeKey = NodeType.BaseType;

            while (BaseTypeKey != null && BaseTypeKey != typeof(IUnitPoolItem))
            {
                //尝试获取父类型法则
                if (self.NodeTypeRulesDictionary.TryGetValue(BaseTypeKey, out var BaseRuleHash))
                {
                    //遍历父类型法则
                    foreach (var ruleType in BaseRuleHash)
                    {
                        //法则不存在，则添加到节点的哈希表里
                        if (!ruleTypeHash.Contains(ruleType))
                        {
                            ruleTypeHash.Add(ruleType);
                            //法则字典的补充
                            if (self.RuleGroupDictionary.TryGetValue(ruleType, out var RuleGroup))
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


        #endregion


        #region 法则获取




        #region 监听目标法则组

        /// <summary>
        /// 获取监听目标法则组
        /// </summary>
        public static bool TryGetTargetRuleGroup<LR>(this RuleManager self, Type targetType, out IRuleGroup<LR> ruleGroup)
            where LR : IListenerRule
        {
            if (self.TryGetTargetRuleGroup(typeof(LR), targetType, out var RuleGroup))
            {
                ruleGroup = RuleGroup as IRuleGroup<LR>;
                return true;
            }
            ruleGroup = default;
            return false;
        }

        /// <summary>
        /// 获取监听目标法则组
        /// </summary>
        public static bool TryGetTargetRuleGroup(this RuleManager self, Type ruleType, Type targetType, out RuleGroup ruleGroup)
        {
            if (self.TargetRuleListenerGroupDictionary.TryGetValue(targetType, out var ruleGroupDictionary))
            {
                return ruleGroupDictionary.TryGetValue(ruleType, out ruleGroup);
            }
            ruleGroup = null;
            return false;
        }

        /// <summary>
        /// 获取监听目标法则列表
        /// </summary>
        public static bool TryGetTargetRuleList<LR>(this RuleManager self, Type targetType, Type listenerType, out IRuleList<LR> ruleList)
            where LR : IListenerRule
        {
            if (self.TargetRuleListenerGroupDictionary.TryGetValue(targetType, out var ruleGroupDictionary))
            {
                if (ruleGroupDictionary.TryGetValue(typeof(LR), out var ruleGroup))
                {
                    if (ruleGroup.TryGetValue(targetType, out RuleList RuleList))
                    {
                        ruleList = RuleList as IRuleList<LR>;
                        return true;
                    }
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
        public static bool TryGetListenerRuleGroup<LR>(this RuleManager self, Type listenerType, out IRuleGroup<LR> ruleGroup)
            where LR : IListenerRule
        {
            if (self.ListenerRuleTargetGroupDictionary.TryGetValue(listenerType, out var ruleGroupDictionary))
            {
                if (ruleGroupDictionary.TryGetValue(typeof(LR), out var RuleGroup))
                {
                    ruleGroup = RuleGroup as IRuleGroup<LR>;
                    return true;
                }
            }
            ruleGroup = null;
            return false;
        }

        /// <summary>
        /// 获取监听法则
        /// </summary>
        public static bool TryGetListenerRuleList<LR>(this RuleManager self, Type listenerType, Type targetType, out IRuleList<LR> ruleList)
            where LR : IListenerRule
        {
            if (self.ListenerRuleTargetGroupDictionary.TryGetValue(listenerType, out var ruleGroupDictionary))
            {
                if (ruleGroupDictionary.TryGetValue(typeof(LR), out var ruleGroup))
                {
                    if (ruleGroup.TryGetValue(targetType, out RuleList RuleList))
                    {
                        ruleList = RuleList as IRuleList<LR>;
                        return true;
                    }
                }
            }
            ruleList = default;
            return false;
        }
        #endregion




        #region  法则组
        /// <summary>
        /// 获取法则组
        /// </summary>
        public static IRuleGroup<R> GetRuleGroup<R>(this RuleManager self) where R : IRule { return self.GetRuleGroup(typeof(R)) as IRuleGroup<R>; }

        /// <summary>
        /// 获取法则组
        /// </summary>
        public static RuleGroup GetRuleGroup(this RuleManager self, Type ruleType)
        {
            self.TryGetRuleGroup(ruleType, out RuleGroup ruleGroup);
            return ruleGroup;
        }

        /// <summary>
        /// 获取法则组
        /// </summary>
        public static bool TryGetRuleGroup<R>(this RuleManager self, out IRuleGroup<R> ruleGroup)
         where R : IRule
        {
            if (self.TryGetRuleGroup(typeof(R), out var RuleGroup))
            {
                ruleGroup = (IRuleGroup<R>)RuleGroup;
                return true;
            }
            ruleGroup = default;
            return false;
        }

        /// <summary>
        /// 获取法则组
        /// </summary>
        public static bool TryGetRuleGroup(this RuleManager self, Type Interface, out RuleGroup ruleGroup)
        {
            return self.RuleGroupDictionary.TryGetValue(Interface, out ruleGroup);
        }

        #endregion

        #region  法则列表

        /// <summary>
        /// 获取单类型法则列表
        /// </summary>
        public static IRuleList<R> GetRuleList<R>(this RuleManager self, Type type)
         where R : IRule
        {
            if (self.RuleGroupDictionary.TryGetValue(typeof(R), out RuleGroup ruleGroup))
            {
                if (ruleGroup.TryGetValue(type, out RuleList ruleList))
                {
                    return ruleList as IRuleList<R>;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取单类型法则列表
        /// </summary>
        public static bool TryGetRuleList(this RuleManager self, Type nodeType, Type ruleType, out RuleList ruleList)
        {
            if (self.RuleGroupDictionary.TryGetValue(ruleType, out RuleGroup ruleGroup))
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




        #endregion
    }
}
