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
* 支持泛型节点：设计目的是更进一步复用代码，同时附带了策略模式。不过泛型类型在第一次生成时，会有一次反射进行泛型组装。
* 
* 特殊支持泛型参数：用于节点不是泛型，而只有一个泛型参数的情况。
* 这种情况是极端的，不提倡使用，和泛型节点一样，内部是反射组装，但这不是自动的，需要动态的在运行时调用泛型支持。
* 
* 总之这是一个功能类似接口的事件系统。
* 
* 

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;

namespace WorldTree
{
	/// <summary>
	/// 世界法则管理器
	/// </summary>
	public class RuleManager : Node, IListenerIgnorer, ComponentOf<WorldTreeCore>
	{
		/// <summary>
		/// 法则字典
		/// </summary>
		/// <remarks> 法则类型《节点类型,法则》</remarks>
		public Dictionary<long, RuleGroup> RuleGroupDict = new();

		/// <summary>
		/// 节点法则字典
		/// </summary>
		/// <remarks> 
		/// <para> 节点类型《法则类型，法则列表》</para>
		/// <para> 记录节点拥有的法则类型，也用于法则多态化的查询</para>
		/// </remarks>
		public Dictionary<long, Dictionary<long, RuleList>> NodeTypeRulesDict = new();

		#region 监听法则

		///// <summary>
		///// 动态监听器节点类型哈希名单
		///// </summary>
		//public HashSet<long> DynamicListenerTypeHash = new();

		/// <summary>
		/// 监听目标为 法则 的 ，监听器法则 字典
		/// </summary>
		public Dictionary<long, HashSet<IListenerRule>> TargetRuleListenerRuleHashDict = new();

		/// <summary>
		/// 监听法则字典 目标节点类型
		/// </summary>
		/// <remarks>
		/// <para>目标节点类型 法则类型 《监听器类型,监听法则列表》</para>
		/// <para>这个是真正被使用的</para>
		/// </remarks>
		public Dictionary<long, Dictionary<long, RuleGroup>> TargetRuleListenerGroupDict = new();

		/// <summary>
		/// 监听法则字典 监听器类型
		/// </summary>
		/// <remarks>
		/// <para>监听器类型 法则类型 《目标节点类型,监听法则列表》</para>
		/// <para>这个是用来查询关系的</para>
		/// </remarks>
		public Dictionary<long, Dictionary<long, RuleGroup>> ListenerRuleTargetGroupDict = new();

		#endregion


		#region 泛型支持

		/// <summary>
		/// 已存在的法则类型哈希名单
		/// </summary>
		/// <remarks>法则类型，节点类型，用于记录已存在的法则，防止泛型动态支持覆盖已有法则</remarks>
		public Dictionary<long, HashSet<long>> RuleTypeHashDict = new();


		/// <summary>
		/// 已支持的类型哈希名单
		/// </summary>
		public HashSet<long> SupportTypeHash = new();

		/// <summary>
		/// 泛型节点 泛型法则哈希表字典
		/// </summary>
		/// <remarks>泛型节点类型，泛型法则类型哈希表</remarks>
		public Dictionary<Type, HashSet<Type>> GenericNodeRuleTypeHashDict = new();


		/// <summary>
		/// 已支持的泛型参数类型哈希名单
		/// </summary>
		/// <remarks>法则类型定义，泛型类型 </remarks>
		public Dictionary<Type, HashSet<long>> SupportGenericTypeDict = new();

		/// <summary>
		/// 泛型参数类型 泛型法则哈希表字典  , 增加法则键值
		/// </summary>
		/// <remarks>法则类型，泛型类型，泛型法则类型哈希表</remarks>
		public Dictionary<Type, Dictionary<Type, HashSet<Type>>> GenericTypeRuleTypeHashDict = new();

		/// <summary>
		/// 未知泛型参数类型 泛型法则哈希表字典 , 增加法则键值
		/// </summary>
		/// <remarks>法则类型，未知泛型类型，泛型法则类型</remarks>
		public Dictionary<Type, HashSet<Type>> GenericRuleTypeDict = new();

		/// <summary>
		/// 泛型参数法则目标节点子类型哈希名单
		/// </summary>
		/// <remarks>节点类型，子类型集合。用于泛型参数法则子类继承支持</remarks>
		public Dictionary<long, HashSet<long>> GenericNodeSubTypeDict = new();

		#endregion

		public override void OnCreate()
		{
			base.OnCreate();
			LoadRule();
		}

		/// <summary>
		/// 重新加载法则
		/// </summary>
		public void LoadRule()
		{
			Clear();
			//反射获取全局继承IRule的法则类型列表
			List<Type> ruleTypeList = new();
			foreach (Type type in Core.TypeInfo.TypeHash64Dict.Keys)
			{
				if (type.GetInterfaces().Contains(typeof(IRule)) && !type.IsAbstract)
					ruleTypeList.Add(type);
			}
			//将按照法则类名进行排序，规范执行顺序
			ruleTypeList.Sort((rule1, rule2) => rule1.Name.CompareTo(rule2.Name));
			foreach (var RuleType in ruleTypeList)//遍历类型列表
			{
				AddRuleType(RuleType);
			}
		}

		/// <summary>
		/// 清空
		/// </summary>
		public void Clear()
		{

			foreach (var RuleGroup in RuleGroupDict)
			{
				foreach (var RuleList in RuleGroup.Value)
				{
					RuleList.Value.Clear();
				}
			}

			foreach (var RuleGroup in NodeTypeRulesDict)
			{
				foreach (var RuleList in RuleGroup.Value)
				{
					RuleList.Value.Clear();
				}
			}

			GenericNodeRuleTypeHashDict.Clear();

			TargetRuleListenerRuleHashDict.Clear();


			foreach (var ListenerRuleGroup in TargetRuleListenerGroupDict)
			{
				foreach (var RuleGroup in ListenerRuleGroup.Value)
				{
					foreach (var RuleList in RuleGroup.Value)
					{
						RuleList.Value.Clear();
					}
				}
			}

			foreach (var ListenerRuleGroup in ListenerRuleTargetGroupDict)
			{
				foreach (var RuleGroup in ListenerRuleGroup.Value)
				{
					foreach (var RuleList in RuleGroup.Value)
					{
						RuleList.Value.Clear();
					}
				}
			}

			RuleTypeHashDict.Clear();
			SupportTypeHash.Clear();
			GenericNodeRuleTypeHashDict.Clear();
			SupportGenericTypeDict.Clear();
			GenericTypeRuleTypeHashDict.Clear();
			GenericRuleTypeDict.Clear();
			GenericNodeSubTypeDict.Clear();
		}

		/// <summary>
		/// 释放后
		/// </summary>
		public override void OnDispose()
		{
			NodeBranchHelper.RemoveBranchNode(Parent, BranchType, this);//从父节点分支移除

			RuleGroupDict.Clear();
			NodeTypeRulesDict.Clear();

			//DynamicListenerTypeHash.Clear();
			GenericNodeRuleTypeHashDict.Clear();
			TargetRuleListenerRuleHashDict.Clear();

			TargetRuleListenerGroupDict.Clear();
			ListenerRuleTargetGroupDict.Clear();

			SupportTypeHash.Clear();
			GenericNodeRuleTypeHashDict.Clear();
			SupportGenericTypeDict.Clear();
			GenericTypeRuleTypeHashDict.Clear();
			GenericRuleTypeDict.Clear();
			GenericNodeSubTypeDict.Clear();

			IsDisposed = true;
		}


		#region 添加法则



		/// <summary>
		/// 添加法则类型
		/// </summary>
		private void AddRuleType(Type ruleType)
		{
			if (ruleType.IsGenericType) //判断法则类型是泛型
			{
				var baseType = ruleType.BaseType;

				//遍历获取一路查到最底层Rule<N,R>
				while (baseType.GetGenericTypeDefinition() != typeof(Rule<,>)) baseType = baseType.BaseType;

				var genericArguments = baseType.GetGenericArguments();
				//Rule<N,R> 第一个泛型参数就是法则负责的目标节点
				if (genericArguments[0].IsGenericType)
				{
					GenericNodeRuleTypeHashDict.GetOrNewValue(genericArguments[0].GetGenericTypeDefinition()).Add(ruleType);
				}
				//假如Node不是泛型，那么就是参数是泛型的情况
				else
				{
					//重新遍历基类，查找泛型参数
					baseType = ruleType.BaseType;
					Type genericType = null;

					//往下找到法则基类
					Type ruleBaseType = baseType;
					while (ruleBaseType.GetGenericTypeDefinition() != typeof(Rule<,>)) ruleBaseType = ruleBaseType.BaseType;
					//Rule<,> 的第一个是节点标记第二个是法则标记。
					Type nodeKeyType = ruleBaseType.GetGenericArguments()[0];
					Type ruleKeyType = ruleBaseType.GetGenericArguments()[1];

					//遍历基类，查找泛型参数
					while (baseType.GetGenericTypeDefinition() != typeof(Rule<,>))
					{
						// 父类是泛型的情况
						if (baseType.IsGenericType)
						{
							//获取泛型参数数组
							genericArguments = baseType.GetGenericArguments();
							foreach (var arg in genericArguments)
							{
								// 泛型参数是泛型的情况
								if (!arg.IsGenericType) continue;
								//判断这个泛型参数，不是法则标记，才是泛型参数。
								if (arg != ruleKeyType)
								{
									genericType = arg;
									break;
								}
							}
						}
						if (genericType != null) break;
						baseType = baseType.BaseType;
					}
					//如果找到了泛型参数
					if (genericType != null && genericType.IsGenericType)
					{
						GenericTypeRuleTypeHashDict.GetOrNewValue(ruleKeyType.GetGenericTypeDefinition()).GetOrNewValue(genericType.GetGenericTypeDefinition()).Add(ruleType);
						GenericNodeSubTypeDict.GetOrNewValue(Core.TypeToCode(nodeKeyType));
					}
					else //泛型参数是泛型本身的情况
					{
						GenericRuleTypeDict.GetOrNewValue(ruleKeyType.GetGenericTypeDefinition()).Add(ruleType);
						GenericNodeSubTypeDict.GetOrNewValue(Core.TypeToCode(nodeKeyType));
					}
				}
			}
			else
			{
				//实例化法则类
				IRule rule = Core.NewUnit(ruleType, out _) as IRule;
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
			if (listenerRule.TargetNodeType == Core.TypeToCode<INode>() && listenerRule.TargetRuleType != Core.TypeToCode<IRule>())
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
				//if (listenerRule.TargetNodeType == TypeInfo<INode>.TypeCode && listenerRule.TargetRuleType == TypeInfo<IRule>.TypeCode)
				//{
				//	if (!DynamicListenerTypeHash.Contains(listenerRule.NodeType)) DynamicListenerTypeHash.Add(listenerRule.NodeType);
				//}
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

			//记录已存在的法则类型
			var typeHash = RuleTypeHashDict.GetOrNewValue(rule.RuleType);
			if (!typeHash.Contains(rule.NodeType)) typeHash.Add(rule.NodeType);


			NodeTypeRulesDict.GetOrNewValue(rule.NodeType).TryAdd(rule.RuleType, ruleList);

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
		/// 支持泛型参数法则
		/// </summary>
		/// <remarks>用于参数是泛型的情况</remarks>
		public void SupportGenericRule<T>(Type ruleTypeDefinition)
		{
			if (SupportGenericTypeDict.TryGetValue(ruleTypeDefinition, out HashSet<long> typeHash))
			{
				if (typeHash.Contains(Core.TypeToCode<T>())) return;
			}
			else
			{
				typeHash = SupportGenericTypeDict.GetOrNewValue(ruleTypeDefinition);
			}

			Type genericType = typeof(T);
			if (genericType.IsGenericType)
			{
				//获取泛型本体类型
				Type genericDefinition = genericType.GetGenericTypeDefinition();
				//获取泛型参数数组
				Type[] genericTypes = genericType.GetGenericArguments();

				if (GenericTypeRuleTypeHashDict.TryGetValue(ruleTypeDefinition, out var RuleTypeDict))
				{
					if (RuleTypeDict.TryGetValue(genericDefinition, out var RuleTypeHash))
					{
						UnitList<IRule> ruleList = this.Core.PoolGetUnit(out UnitList<IRule> _);
						foreach (var RuleType in RuleTypeHash)
						{
							//填入对应的泛型参数，实例化泛型监听系统

							IRule rule = (IRule)Core.NewUnit(RuleType.MakeGenericType(genericTypes), out _);
							//添加法则，泛型动态支持不可覆盖已有定义
							if (!(RuleTypeHashDict.TryGetValue(rule.RuleType, out var ruleGroup) && ruleGroup.Contains(rule.NodeType)))
								ruleList.Add(rule);
						}
						foreach (var rule in ruleList) AddRule(rule);
						foreach (var rule in ruleList)
						{
							if (!GenericNodeSubTypeDict.TryGetValue(rule.NodeType, out var subTypeHash)) continue;
							foreach (long nodeType in subTypeHash) SupportPolymorphicRule(nodeType);
						}
						ruleList.Dispose();

						typeHash.Add(Core.TypeToCode<T>());//已支持名单
						return;
					}
				}
			}

			// 走到这里说明 这个泛型参数其实是未知的，需要动态支持
			if (GenericRuleTypeDict.TryGetValue(ruleTypeDefinition, out var ruleTypeHash))
			{
				UnitList<IRule> ruleList = this.Core.PoolGetUnit(out UnitList<IRule> _);
				Type genericDefinition = genericType;
				foreach (var RuleType in ruleTypeHash)
				{
					//填入对应的泛型参数，实例化泛型监听系统
					IRule rule = (IRule)Core.NewUnit(RuleType.MakeGenericType(genericDefinition), out _);
					//添加法则，泛型动态支持不可覆盖已有定义
					if (!(RuleTypeHashDict.TryGetValue(rule.RuleType, out var ruleGroup) && ruleGroup.Contains(rule.NodeType)))
						ruleList.Add(rule);
				}
				foreach (var rule in ruleList) AddRule(rule);
				foreach (var rule in ruleList)
				{
					if (!GenericNodeSubTypeDict.TryGetValue(rule.NodeType, out var subTypeHash)) continue;
					foreach (long nodeType in subTypeHash) SupportPolymorphicRule(nodeType);
				}

				ruleList.Dispose();
			}
		}



		/// <summary>
		/// 支持泛型节点法则
		/// </summary>
		/// <remarks>
		/// <para>将会通过反射查询自身及所有父类是否有泛型</para>
		/// </remarks>
		private void SupportGenericNodeRule(long nodeType)
		{
			Type type = Core.CodeToType(nodeType);
			Type[] interfaces = type.GetInterfaces();

			//开始遍历查询父类型泛型法则
			while (type != null && type != typeof(object))
			{
				//节点可能会是非泛型，但父类则有泛型的情况，需要多态化所有父类泛型法则
				CreateGenericNodeRule(type);
				type = type.BaseType;

				//泛型参数法则子类继承支持记录
				//检测类型父类是否有泛型参数法则，有则记录
				long typeCode = Core.TypeToCode(type);
				if (GenericNodeSubTypeDict.TryGetValue(typeCode, out var nodeSubTypeHash))
				{
					if (!nodeSubTypeHash.Contains(nodeType)) nodeSubTypeHash.Add(nodeType);
				}
			}

			//遍历接口
			foreach (var interfaceType in interfaces)
			{
				CreateGenericNodeRule(interfaceType);
				long typeCode = Core.TypeToCode(interfaceType);
				if (GenericNodeSubTypeDict.TryGetValue(typeCode, out var nodeSubTypeHash))
				{
					if (!nodeSubTypeHash.Contains(nodeType)) nodeSubTypeHash.Add(nodeType);
				}
			}
		}

		/// <summary>
		/// 组合创建泛型节点法则
		/// </summary>
		private void CreateGenericNodeRule(Type type)
		{
			if (type.IsGenericType)
			{
				//获取泛型本体类型
				Type genericNodeType = type.GetGenericTypeDefinition();
				//获取泛型参数数组
				Type[] genericTypes = type.GetGenericArguments();
				if (GenericNodeRuleTypeHashDict.TryGetValue(genericNodeType, out var RuleTypeHash))
				{
					UnitList<IRule> ruleList = this.Core.PoolGetUnit(out UnitList<IRule> _);

					foreach (var RuleType in RuleTypeHash)
					{
						//填入对应的泛型参数，实例化泛型监听系统
						IRule rule = (IRule)Core.NewUnit(RuleType.MakeGenericType(genericTypes), out _);
						//添加法则，泛型动态支持不可覆盖已有定义
						if (!(RuleTypeHashDict.TryGetValue(rule.RuleType, out var ruleGroup) && ruleGroup.Contains(rule.NodeType)))
							ruleList.Add(rule);
					}
					foreach (var rule in ruleList) AddRule(rule);
					ruleList.Dispose();
				}
			}
		}


		#endregion

		#region 法则多态

		/// <summary>
		/// 支持监听器的多态法则
		/// </summary>
		private void SupportPolymorphicListenerRule(long listenerNodeTypeCode)
		{
			Type listenerNodeType = Core.CodeToType(listenerNodeTypeCode);

			//监听器父类类型键值
			Type listenerBaseType = listenerNodeType.BaseType;
			while (listenerBaseType != null && listenerBaseType != typeof(object))
			{
				PolymorphicListenerRule(listenerNodeTypeCode, Core.TypeToCode(listenerBaseType));
				listenerBaseType = listenerBaseType.BaseType;
			}

			//遍历接口
			Type[] interfaceTypes = listenerNodeType.GetInterfaces();
			foreach (var interfaceType in interfaceTypes)
			{
				PolymorphicListenerRule(listenerNodeTypeCode, Core.TypeToCode(interfaceType));
			}
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

			///动态监听法则多态记录
			//if (DynamicListenerTypeHash.Contains(listenerBaseTypeCodeKey))
			//{
			//	if (!DynamicListenerTypeHash.Contains(listenerNodeType))
			//		DynamicListenerTypeHash.Add(listenerNodeType);
			//}

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
		private void SupportPolymorphicRule(long nodeTypeCode)
		{
			Type nodeType = Core.CodeToType(nodeTypeCode);

			//开始遍历查询父类型法则
			Type baseType = nodeType.BaseType;
			while (baseType != null && baseType != typeof(object))
			{
				PolymorphicRule(nodeTypeCode, Core.TypeToCode(baseType));
				baseType = baseType.BaseType;
			}

			//遍历接口
			Type[] interfaceTypes = nodeType.GetInterfaces();
			foreach (var interfaceType in interfaceTypes)
			{
				PolymorphicRule(nodeTypeCode, Core.TypeToCode(interfaceType));
			}
		}

		/// <summary>
		/// 多态化一个节点类型的法则
		/// </summary>
		private void PolymorphicRule(long nodeType, long baseTypeCodeKey)
		{
			//判断父类是否有法则，没有则退出
			if (!NodeTypeRulesDict.TryGetValue(baseTypeCodeKey, out var BaseRuleHash)) return;

			//拿到节点类型的法则哈希表
			Dictionary<long, RuleList> ruleTypeDict = NodeTypeRulesDict.GetOrNewValue(nodeType);

			//遍历父类型法则
			foreach (var ruleType in BaseRuleHash)
			{
				//存在的法则则跳过
				if (ruleTypeDict.Contains(ruleType)) continue;
				//节点记录法则
				ruleTypeDict.TryAdd(ruleType.Key, ruleType.Value);
				//法则字典的补充
				if (!RuleGroupDict.TryGetValue(ruleType.Key, out var RuleGroup)) continue;
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
			if (TryGetTargetRuleGroup(Core.TypeToCode<LR>(), targetType, out var RuleGroup))
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
			return TryGetTargetRuleGroup(Core.TypeToCode<LR>(), targetType, out ruleGroup);
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
			return GetOrNewTargetRuleGroup(Core.TypeToCode<LR>(), targetType);
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
				if (ruleGroupDictionary.TryGetValue(Core.TypeToCode<LR>(), out var ruleGroup))
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
				if (ruleGroupDictionary.TryGetValue(Core.TypeToCode<LR>(), out var RuleGroup))
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
				if (ruleGroupDictionary.TryGetValue(Core.TypeToCode<LR>(), out var ruleGroup))
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
			if (TryGetRuleGroup(Core.TypeToCode<R>(), out var RuleGroup))
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
			return TryGetRuleGroup(Core.TypeToCode<R>(), out ruleGroup);
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
			return GetOrNewRuleGroup(Core.TypeToCode<R>());
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
			if (RuleGroupDict.TryGetValue(Core.TypeToCode<R>(), out RuleGroup ruleGroup))
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
			return GetOrNewRuleList(nodeType, Core.TypeToCode<R>());
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