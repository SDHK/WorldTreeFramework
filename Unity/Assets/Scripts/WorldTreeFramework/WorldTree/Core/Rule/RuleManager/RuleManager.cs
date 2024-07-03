/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/26 16:29

* 描述： 世界法则管理器
*
* 法则其实是一个事件系统，以接口为键值进行调用。
* 
* 启动时反射获取全局继承了IRule的接口的法则类，进行实例化并注册。
*
* 支持多播：法则可以实现多个，将通过名称顺序执行，同时附带责任链模式。
* 支持多态：设计目的是可通过继承复用代码，不提倡设计复杂的多重继承，能拆分写的功能就拆分写。
* 支持泛型：设计目的是更进一步复用代码，同时附带了策略模式。不过泛型类型在第一次生成时，会有一次反射进行泛型组装。
* 
* 这是一个功能类似接口的事件系统。
* 

*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldTree
{
	/// <summary>
	/// 世界法则管理器
	/// </summary>
	public class RuleManager : Node, IListenerIgnorer, ComponentOf<WorldTreeCore>
	{
		/// <summary>
		/// 已支持的类型哈希名单
		/// </summary>
		public UnitHashSet<long> SupportTypeHash = new();

		/// <summary>
		/// 动态监听器节点类型哈希名单
		/// </summary>
		public UnitHashSet<long> DynamicListenerTypeHash = new();

		/// <summary>
		/// 监听目标为 法则 的 ，监听器法则 字典
		/// </summary>
		public UnitDictionary<long, UnitHashSet<IListenerRule>> TargetRuleListenerRuleHashDict = new();

		/// <summary>
		/// 泛型节点泛型法则哈希表字典
		/// </summary>
		/// <remarks>泛型节点类型，泛型法则类型哈希表</remarks>
		public UnitDictionary<Type, UnitHashSet<Type>> GenericRuleTypeHashDict = new();

		/// <summary>
		/// 监听法则字典 目标节点类型
		/// </summary>
		/// <remarks>
		/// <para>目标节点类型 法则类型 《监听器类型,监听法则列表》</para>
		/// <para>这个是真正被使用的</para>
		/// </remarks>
		public UnitDictionary<long, Dictionary<long, RuleGroup>> TargetRuleListenerGroupDict = new();

		/// <summary>
		/// 监听法则字典 监听器类型
		/// </summary>
		/// <remarks>
		/// <para>监听器类型 法则类型 《目标节点类型,监听法则列表》</para>
		/// <para>这个是用来查询关系的</para>
		/// </remarks>
		public UnitDictionary<long, Dictionary<long, RuleGroup>> ListenerRuleTargetGroupDict = new();

		/// <summary>
		/// 法则字典
		/// </summary>
		/// <remarks> 法则类型《节点类型,法则》</remarks>
		public UnitDictionary<long, RuleGroup> RuleGroupDict = new();

		/// <summary>
		/// 节点法则字典
		/// </summary>
		/// <remarks>记录节点拥有的法则类型，用于法则多态化的查询</remarks>
		public UnitDictionary<long, HashSet<long>> NodeTypeRulesDict = new();



		public RuleManager()
		{
			Type = TypeInfo<RuleManager>.TypeCode;

			//反射获取全局继承IRule的法则类型列表
			var ruleTypeList = FindTypesIsInterface(typeof(IRule));

			//将按照法则类名进行排序，规范执行顺序
			ruleTypeList.Sort((rule1, rule2) => rule1.Name.CompareTo(rule2.Name));

			foreach (var RuleType in ruleTypeList)//遍历类型列表
			{
				AddRuleType(RuleType);
			}
		}

		/// <summary>
		/// 释放后
		/// </summary>
		public override void OnDispose()
		{
			NodeBranchHelper.RemoveBranchNode(Parent, BranchType, this);//从父节点分支移除

			RuleGroupDict.Clear();
			NodeTypeRulesDict.Clear();

			DynamicListenerTypeHash.Clear();
			GenericRuleTypeHashDict.Clear();
			TargetRuleListenerRuleHashDict.Clear();

			TargetRuleListenerGroupDict.Clear();
			ListenerRuleTargetGroupDict.Clear();

			IsDisposed = true;
		}


		#region 添加法则

		/// <summary>
		/// 查找继承了接口的类型
		/// </summary>
		private List<Type> FindTypesIsInterface(Type interfaceType)
		{
			//return self.Core.Assemblys.SelectMany(a => a.GetTypes().Where(T => T.GetInterfaces().Contains(Interface) && !T.IsAbstract)).ToList();
			//System.Reflection.Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
			return AppDomain.CurrentDomain.GetAssemblies().SelectMany(anyAssembly => anyAssembly.GetTypes().Where(anyType => anyType.GetInterfaces().Contains(interfaceType) && !anyType.IsAbstract)).ToList();
		}

		/// <summary>
		/// 添加法则类型
		/// </summary>
		private void AddRuleType(Type ruleType)
		{
			if (ruleType.IsGenericType) //判断法则类型是泛型
			{
				var baseType = ruleType.BaseType;

				//遍历获取一路查到最底层Rule<N,R>
				while (baseType.GetGenericTypeDefinition() != typeof(Rule<,>))
				{
					baseType = baseType.BaseType;
				}
				var genericArguments = baseType.GetGenericArguments();
				//Rule<N,R> 第一个泛型参数就是法则负责的目标节点
				GenericRuleTypeHashDict.GetOrNewValue(genericArguments[0].GetGenericTypeDefinition()).Add(ruleType);
			}
			else
			{
				//实例化法则类
				IRule rule = Activator.CreateInstance(ruleType, true) as IRule;
				AddRule(rule);
			}
		}

		/// <summary>
		/// 添加法则
		/// </summary>
		private void AddRule(IRule rule)
		{
			if (rule is IListenerRule)
			{
				var listenerRule = rule as IListenerRule;//转换为监听法则
				AddListenerRule(listenerRule);
			}
			else
			{
				AddNodeRule(rule);
			}
		}

		/// <summary>
		/// 添加监听器法则
		/// </summary>
		private void AddListenerRule(IListenerRule listenerRule)
		{
			if (listenerRule.TargetNodeType == TypeInfo<INode>.TypeCode && listenerRule.TargetRuleType != TypeInfo<IRule>.TypeCode)
			{
				//监听目标为法则的
				TargetRuleListenerRuleHashDict.GetOrNewValue(listenerRule.TargetRuleType).Add(listenerRule);

				//获取 监听法则 目标法则类型，当前的法则组。
				if (RuleGroupDict.TryGetValue(listenerRule.TargetRuleType, out RuleGroup ruleGroup))
				{
					foreach (var ruleList in ruleGroup)
					{
						DictionaryAddNodeRule(ruleList.Key, listenerRule);
					}
				}
			}
			else
			{
				//监听目标为节点，或 动态监听
				DictionaryAddNodeRule(listenerRule.TargetNodeType, listenerRule);

				//动态监听器判断
				if (listenerRule.TargetNodeType == TypeInfo<INode>.TypeCode && listenerRule.TargetRuleType == TypeInfo<IRule>.TypeCode)
				{
					if (!DynamicListenerTypeHash.Contains(listenerRule.NodeType)) DynamicListenerTypeHash.Add(listenerRule.NodeType);
				}
			}
		}

		/// <summary>
		/// 添加节点法则
		/// </summary>
		private void AddNodeRule(IRule rule)
		{
			var groupDict = RuleGroupDict.GetOrNewValue(rule.RuleType);
			var ruleList = groupDict.GetOrNewValue(rule.NodeType);
			ruleList.RuleType = rule.RuleType;
			groupDict.RuleType = rule.RuleType;
			ruleList.AddRule(rule);

			NodeTypeRulesDict.GetOrNewValue(rule.NodeType).Add(rule.RuleType);

			//监听器法则补齐
			if (TargetRuleListenerRuleHashDict.TryGetValue(rule.NodeType, out var listenerRuleHash))
			{
				foreach (var listenerRule in listenerRuleHash)
				{
					DictionaryAddNodeRule(rule.NodeType, listenerRule);
				}
			}
		}

		/// <summary>
		/// 字典分组添加监听器法则
		/// </summary>
		private void DictionaryAddNodeRule(long targetNodeType, IListenerRule listenerRule)
		{
			var listenerRuleGroupDict = ListenerRuleTargetGroupDict.GetOrNewValue(listenerRule.NodeType).GetOrNewValue(listenerRule.RuleType);
			var listenerRuleList = listenerRuleGroupDict.GetOrNewValue(targetNodeType);
			listenerRuleList.AddRule(listenerRule);
			listenerRuleList.RuleType = listenerRule.RuleType;
			listenerRuleGroupDict.RuleType = listenerRule.RuleType;

			var targetRuleGroupDict = TargetRuleListenerGroupDict.GetOrNewValue(targetNodeType).GetOrNewValue(listenerRule.RuleType);
			var targetRuleList = targetRuleGroupDict.GetOrNewValue(listenerRule.NodeType);
			targetRuleList.AddRule(listenerRule);
			targetRuleList.RuleType = listenerRule.RuleType;
			targetRuleGroupDict.RuleType = listenerRule.RuleType;
		}

		#endregion

		#region 补充法则

		/// <summary>
		/// 补充节点法则功能
		/// </summary>
		public void SupportNodeRule(long nodeType)
		{
			if (!SupportTypeHash.Contains(nodeType))
			{
				SupportGenericNodeRule(nodeType);//支持泛型法则
				SupportPolymorphicListenerRule(nodeType);//支撑继承监听法则
				SupportPolymorphicRule(nodeType);//支撑继承法则
				SupportTypeHash.Add(nodeType);//已支持名单
			}
		}

		#region 法则泛型

		/// <summary>
		/// 支持泛型节点法则
		/// </summary>
		/// <remarks>
		/// <para>将会通过反射查询自身及所有父类是否有泛型</para>
		/// </remarks>
		private void SupportGenericNodeRule(long nodeType)
		{
			Type type = nodeType.CodeToType();
			while (type != null && type != typeof(object))
			{
				//节点可能会是非泛型，但父类则有泛型的情况，需要多态化所有父类泛型法则
				if (type.IsGenericType)
				{
					//获取泛型本体类型
					Type genericNodeType = type.GetGenericTypeDefinition();
					//获取泛型参数数组
					Type[] genericTypes = type.GetGenericArguments();

					if (GenericRuleTypeHashDict.TryGetValue(genericNodeType, out var RuleTypeHash))
					{
						foreach (var RuleType in RuleTypeHash)
						{
							//填入对应的泛型参数，实例化泛型监听系统
							IRule rule = (IRule)Activator.CreateInstance(RuleType.MakeGenericType(genericTypes));
							AddRule(rule);
						}
					}
				}
				type = type.BaseType;
			}
		}

		#endregion

		#region 法则多态

		/// <summary>
		/// 支持监听器的多态法则
		/// </summary>
		private void SupportPolymorphicListenerRule(long listenerNodeType)
		{
			//监听器父类类型键值
			Type listenerBaseType = listenerNodeType.CodeToType().BaseType;
			//类型哈希码
			long listenerBaseTypeCodeKey = listenerBaseType.TypeToCode();

			while (listenerBaseType != null && listenerBaseType != typeof(object))
			{
				PolymorphicListenerRule(listenerNodeType, listenerBaseTypeCodeKey);
				listenerBaseType = listenerBaseType.BaseType;
				listenerBaseTypeCodeKey = listenerBaseType.TypeToCode();
			}
			if (listenerBaseType.GetInterfaces().Contains(TypeInfo<INode>.Type))
				PolymorphicListenerRule(listenerNodeType, TypeInfo<INode>.TypeCode);
		}

		/// <summary>
		/// 多态化一个节点类型的监听法则
		/// </summary>
		private void PolymorphicListenerRule(long listenerNodeType, long listenerBaseTypeCodeKey)
		{
			//判断父类是否有法则，没有则退出
			if (!ListenerRuleTargetGroupDict.TryGetValue(listenerBaseTypeCodeKey, out var RuleType_TargerGroupDictionary)) return;

			//拿到节点自身的：法则类型 《目标节点类型,监听法则》
			Dictionary<long, RuleGroup> nodeRuleType_TargerGroupDict = ListenerRuleTargetGroupDict.GetOrNewValue(listenerNodeType);

			//动态监听法则多态记录
			if (DynamicListenerTypeHash.Contains(listenerBaseTypeCodeKey))
			{
				if (!DynamicListenerTypeHash.Contains(listenerNodeType))
					DynamicListenerTypeHash.Add(listenerNodeType);
			}

			//K:法则类型 , V:《目标节点类型,监听法则列表》
			foreach (var RuleType_TargetGroupKV in RuleType_TargerGroupDictionary)
			{
				//自身已经存在的法则则跳过
				if (nodeRuleType_TargerGroupDict.ContainsKey(RuleType_TargetGroupKV.Key)) continue;

				//父类的监听法则添加到自身，也就是继承法则功能
				nodeRuleType_TargerGroupDict.Add(RuleType_TargetGroupKV.Key, RuleType_TargetGroupKV.Value);

				//接下来补齐 ListenerRuleTargetGroupDictionary 对应的 TargetRuleListenerGroupDictionary
				//K:目标节点类型 , V:监听法则列表
				foreach (var TargetType_RuleListKV in RuleType_TargetGroupKV.Value)
				{
					//目标类型为主的字典， 进行目标类型查找
					if (!TargetRuleListenerGroupDict.TryGetValue(TargetType_RuleListKV.Key, out var RuleType_ListenerGroupDictionary)) continue;

					//法则类型查找
					if (!RuleType_ListenerGroupDictionary.TryGetValue(RuleType_TargetGroupKV.Key, out var ListenerGroup)) continue;

					//监听器存在的父类型 查找 法则列表
					if (!ListenerGroup.TryGetValue(listenerBaseTypeCodeKey, out var ruleList)) continue;

					//尝试将父类的 法则列表，添加进 没有法则的 监听器类型。
					ListenerGroup.TryAdd(listenerNodeType, ruleList);
				}
			}
		}


		/// <summary>
		/// 支持节点多态法则
		/// </summary>
		private void SupportPolymorphicRule(long nodeType)
		{
			//开始遍历查询父类型法则
			Type baseType = nodeType.CodeToType().BaseType;
			//父类型哈希码
			long baseTypeCodeKey = baseType.TypeToCode();

			while (baseType != null && baseType != typeof(object))
			{
				PolymorphicRule(nodeType, baseTypeCodeKey);
				baseType = baseType.BaseType;
				baseTypeCodeKey = baseType.TypeToCode();
			}
			//检测是否继承了INode 接口,支持INode的多态法则
			if (baseType.GetInterfaces().Contains(TypeInfo<INode>.Type))
				PolymorphicRule(nodeType, TypeInfo<INode>.TypeCode);
		}

		/// <summary>
		/// 多态化一个节点类型的法则
		/// </summary>
		private void PolymorphicRule(long nodeType, long baseTypeCodeKey)
		{
			//判断父类是否有法则，没有则退出
			if (!NodeTypeRulesDict.TryGetValue(baseTypeCodeKey, out var BaseRuleHash)) return;

			//拿到节点类型的法则哈希表
			HashSet<long> ruleTypeHash = NodeTypeRulesDict.GetOrNewValue(nodeType);

			//遍历父类型法则
			foreach (var ruleType in BaseRuleHash)
			{
				//存在的法则则跳过
				if (ruleTypeHash.Contains(ruleType)) continue;

				ruleTypeHash.Add(ruleType);

				//法则字典的补充
				if (!RuleGroupDict.TryGetValue(ruleType, out var RuleGroup)) continue;

				//获取父类型法则列表
				if (!RuleGroup.TryGetValue(baseTypeCodeKey, out var ruleList)) continue;

				//法则列表添加进节点类型
				RuleGroup.TryAdd(nodeType, ruleList);
			}
		}

		#endregion

		#endregion

		#region 法则获取

		#region 获取监听目标法则组

		/// <summary>
		/// 获取监听目标法则组
		/// </summary>
		public bool TryGetTargetRuleGroup<LR>(long targetType, out IRuleGroup<LR> ruleGroup)
			where LR : IListenerRule
		{
			if (TryGetTargetRuleGroup(TypeInfo<LR>.TypeCode, targetType, out var RuleGroup))
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
		public bool TryGetTargetRuleGroup<LR>(long targetType, out RuleGroup ruleGroup)
			where LR : IListenerRule
		{
			return TryGetTargetRuleGroup(TypeInfo<LR>.TypeCode, targetType, out ruleGroup);
		}

		/// <summary>
		/// 获取监听目标法则组
		/// </summary>
		public bool TryGetTargetRuleGroup(long ruleType, long targetType, out RuleGroup ruleGroup)
		{
			if (TargetRuleListenerGroupDict.TryGetValue(targetType, out var ruleGroupDictionary))
			{
				return ruleGroupDictionary.TryGetValue(ruleType, out ruleGroup);
			}
			ruleGroup = null;
			return false;
		}

		/// <summary>
		/// 强制获取占位目标法则组
		/// </summary>
		public RuleGroup GetOrNewTargetRuleGroup<LR>(long targetType)
			where LR : IListenerRule
		{
			return GetOrNewTargetRuleGroup(TypeInfo<LR>.TypeCode, targetType);
		}

		/// <summary>
		/// 强制获取占位目标法则组
		/// </summary>
		public RuleGroup GetOrNewTargetRuleGroup(long ruleType, long targetType)
		{
			if (TargetRuleListenerGroupDict.TryGetValue(targetType, out var ruleGroupDictionary))
			{
				if (ruleGroupDictionary.TryGetValue(ruleType, out var ruleGroup))
				{
					return ruleGroup;
				}
				else
				{
					ruleGroup = ruleGroupDictionary.GetOrNewValue(ruleType);
					ruleGroup.RuleType = ruleType;
					return ruleGroup;
				}
			}
			else
			{
				ruleGroupDictionary = TargetRuleListenerGroupDict.GetOrNewValue(targetType);
				RuleGroup ruleGroupDict = ruleGroupDictionary.GetOrNewValue(ruleType);
				ruleGroupDict.RuleType = ruleType;
				return ruleGroupDict;
			}
		}

		#endregion

		#region 获取监听目标法则列表

		/// <summary>
		/// 获取监听目标法则列表
		/// </summary>
		public bool TryGetTargetRuleList<LR>(long targetType, out IRuleList<LR> ruleList)
			where LR : IListenerRule
		{
			if (TargetRuleListenerGroupDict.TryGetValue(targetType, out var ruleGroupDictionary))
			{
				if (ruleGroupDictionary.TryGetValue(TypeInfo<LR>.TypeCode, out var ruleGroup))
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

		#region  获取监听法则组

		/// <summary>
		/// 获取监听法则组
		/// </summary>
		public bool TryGetListenerRuleGroup<LR>(long listenerType, out IRuleGroup<LR> ruleGroup)
			where LR : IListenerRule
		{
			if (ListenerRuleTargetGroupDict.TryGetValue(listenerType, out var ruleGroupDictionary))
			{
				if (ruleGroupDictionary.TryGetValue(TypeInfo<LR>.TypeCode, out var RuleGroup))
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
		public bool TryGetListenerRuleList<LR>(long listenerType, long targetType, out IRuleList<LR> ruleList)
			where LR : IListenerRule
		{
			if (ListenerRuleTargetGroupDict.TryGetValue(listenerType, out var ruleGroupDictionary))
			{
				if (ruleGroupDictionary.TryGetValue(TypeInfo<LR>.TypeCode, out var ruleGroup))
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

		#region  获取法则组

		/// <summary>
		/// 获取逆变法则组
		/// </summary>
		public bool TryGetRuleGroup<R>(out IRuleGroup<R> ruleGroup)
		 where R : IRule
		{
			if (TryGetRuleGroup(TypeInfo<R>.TypeCode, out var RuleGroup))
			{
				ruleGroup = (IRuleGroup<R>)RuleGroup;
				return true;
			}
			ruleGroup = default;
			return false;
		}

		/// <summary>
		/// 尝试获取法则组
		/// </summary>
		public bool TryGetRuleGroup<R>(out RuleGroup ruleGroup)
		 where R : IRule
		{
			return TryGetRuleGroup(TypeInfo<R>.TypeCode, out ruleGroup);
		}

		/// <summary>
		/// 尝试获取法则组
		/// </summary>
		public bool TryGetRuleGroup(long ruleType, out RuleGroup ruleGroup)
		{
			return RuleGroupDict.TryGetValue(ruleType, out ruleGroup);
		}

		/// <summary>
		/// 强制获取占位法则组
		/// </summary>
		public RuleGroup GetOrNewRuleGroup<R>()
		 where R : IRule
		{
			return GetOrNewRuleGroup(TypeInfo<R>.TypeCode);
		}

		/// <summary>
		/// 强制获取占位法则组
		/// </summary>
		public RuleGroup GetOrNewRuleGroup(long ruleType)
		{
			var groupDict = RuleGroupDict.GetOrNewValue(ruleType);
			groupDict.RuleType = ruleType;
			return groupDict;
		}

		#endregion

		#region  获取法则列表

		/// <summary>
		/// 获取单类型法则列表
		/// </summary>
		public bool TryGetRuleList<R>(long nodeType, out IRuleList<R> ruleList)
		 where R : IRule
		{
			if (TryGetRuleList<R>(nodeType, out RuleList rules))
			{
				ruleList = rules as IRuleList<R>;
				return ruleList != null;
			}
			else
			{
				ruleList = null;
				return false;
			}
		}

		/// <summary>
		/// 获取单类型法则列表
		/// </summary>
		public bool TryGetRuleList(long nodeType, long ruleType, out RuleList ruleList)
		{
			if (RuleGroupDict.TryGetValue(ruleType, out RuleGroup ruleGroup))
			{
				return ruleGroup.TryGetValue(nodeType, out ruleList);
			}
			else
			{
				ruleList = null;
				return false;
			}
		}

		/// <summary>
		/// 获取单类型法则列表
		/// </summary>
		public bool TryGetRuleList<R>(long nodeType, out RuleList ruleList)
		 where R : IRule
		{
			if (RuleGroupDict.TryGetValue(TypeInfo<R>.TypeCode, out RuleGroup ruleGroup))
			{
				return ruleGroup.TryGetValue(nodeType, out ruleList);
			}
			else
			{
				ruleList = null;
				return false;
			}
		}

		/// <summary>
		/// 强制获取占位法则列表
		/// </summary>
		public RuleList GetOrNewRuleList<R>(long nodeType)
		 where R : IRule
		{
			return GetOrNewRuleList(nodeType, TypeInfo<R>.TypeCode);
		}

		/// <summary>
		/// 强制获取占位法则列表
		/// </summary>
		public RuleList GetOrNewRuleList(long nodeType, long ruleType)
		{
			if (RuleGroupDict.TryGetValue(ruleType, out RuleGroup ruleGroup))
			{
				if (ruleGroup.TryGetValue(nodeType, out RuleList ruleList))
				{
					return ruleList;
				}
				else
				{
					ruleList = ruleGroup.GetOrNewValue(nodeType);
					ruleList.RuleType = ruleType;
					return ruleList;
				}
			}
			else
			{
				ruleGroup = RuleGroupDict.GetOrNewValue(ruleType);
				ruleGroup.RuleType = ruleType;
				var ruleList = ruleGroup.GetOrNewValue(nodeType);
				ruleList.RuleType = ruleType;
				return ruleList;
			}
		}

		#endregion

		#endregion
	}
}