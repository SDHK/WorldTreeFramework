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

namespace WorldTree
{

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


}
