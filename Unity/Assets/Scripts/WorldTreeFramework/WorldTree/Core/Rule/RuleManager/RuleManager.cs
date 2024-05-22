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
		public UnitDictionary<long, UnitHashSet<IListenerRule>> TargetRuleListenerRuleHashDictionary = new();

		/// <summary>
		/// 泛型节点泛型法则哈希表字典
		/// </summary>
		/// <remarks>泛型节点类型，泛型法则类型哈希表</remarks>
		public UnitDictionary<Type, UnitHashSet<Type>> GenericRuleTypeHashDictionary = new();

		/// <summary>
		/// 监听法则字典 目标节点类型
		/// </summary>
		/// <remarks>
		/// <para>目标节点类型 法则类型 《监听器类型,监听法则列表》</para>
		/// <para>这个是真正被使用的</para>
		/// </remarks>
		public UnitDictionary<long, Dictionary<long, RuleGroup>> TargetRuleListenerGroupDictionary = new();

		/// <summary>
		/// 监听法则字典 监听器类型
		/// </summary>
		/// <remarks>
		/// <para>监听器类型 法则类型 《目标节点类型,监听法则列表》</para>
		/// <para>这个是用来查询关系的</para>
		/// </remarks>
		public UnitDictionary<long, Dictionary<long, RuleGroup>> ListenerRuleTargetGroupDictionary = new();

		/// <summary>
		/// 法则字典
		/// </summary>
		/// <remarks> 法则类型《节点类型,法则》</remarks>
		public UnitDictionary<long, RuleGroup> RuleGroupDictionary = new();

		/// <summary>
		/// 节点法则字典
		/// </summary>
		/// <remarks>记录节点拥有的法则类型，用于法则多态化的查询</remarks>
		public UnitDictionary<long, HashSet<long>> NodeTypeRulesDictionary = new();

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
			self.Type = TypeInfo<RuleManager>.TypeCode;

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
			NodeBranchHelper.RemoveBranchNode(self.Parent, self.BranchType, self);//从父节点分支移除

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
			//return self.Core.Assemblys.SelectMany(a => a.GetTypes().Where(T => T.GetInterfaces().Contains(Interface) && !T.IsAbstract)).ToList();
			//System.Reflection.Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
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

				//遍历获取一路查到最底层Rule<N,R>
				while (BaseType.GetGenericTypeDefinition() != typeof(Rule<,>))
				{
					BaseType = BaseType.BaseType;
				}
				var GenericArguments = BaseType.GetGenericArguments();
				//Rule<N,R> 第一个泛型参数就是法则负责的目标节点
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
			if (listenerRule.TargetNodeType == TypeInfo<INode>.TypeCode && listenerRule.TargetRuleType != TypeInfo<IRule>.TypeCode)
			{
				//监听目标为法则的
				self.TargetRuleListenerRuleHashDictionary.GetValue(listenerRule.TargetRuleType).Add(listenerRule);

				//获取 监听法则 目标法则类型，当前的法则组。
				if (self.RuleGroupDictionary.TryGetValue(listenerRule.TargetRuleType, out RuleGroup ruleGroup))
				{
					foreach (var ruleList in ruleGroup)
					{
						self.DictionaryAddNodeRule(ruleList.Key, listenerRule);
					}
				}
			}
			else
			{
				//监听目标为节点，或 动态监听
				self.DictionaryAddNodeRule(listenerRule.TargetNodeType, listenerRule);

				//动态监听器判断
				if (listenerRule.TargetNodeType == TypeInfo<INode>.TypeCode && listenerRule.TargetRuleType == TypeInfo<IRule>.TypeCode)
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
			ruleList.RuleType = rule.RuleType;
			group.RuleType = rule.RuleType;
			ruleList.AddRule(rule);

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
		private static void DictionaryAddNodeRule(this RuleManager self, long TargetNodeType, IListenerRule listenerRule)
		{
			var ListenerRuleGroup = self.ListenerRuleTargetGroupDictionary.GetValue(listenerRule.NodeType).GetValue(listenerRule.RuleType);
			var ListenerRuleList = ListenerRuleGroup.GetValue(TargetNodeType);
			ListenerRuleList.AddRule(listenerRule);
			ListenerRuleList.RuleType = listenerRule.RuleType;
			ListenerRuleGroup.RuleType = listenerRule.RuleType;

			var TargetRuleGroup = self.TargetRuleListenerGroupDictionary.GetValue(TargetNodeType).GetValue(listenerRule.RuleType);
			var TargetRuleList = TargetRuleGroup.GetValue(listenerRule.NodeType);
			TargetRuleList.AddRule(listenerRule);
			TargetRuleList.RuleType = listenerRule.RuleType;
			TargetRuleGroup.RuleType = listenerRule.RuleType;
		}

		#endregion

		#region 补充法则

		/// <summary>
		/// 补充节点法则功能
		/// </summary>
		public static void SupportNodeRule(this RuleManager self, long NodeType)
		{
			if (!self.SupportTypeHash.Contains(NodeType))
			{
				self.SupportGenericNodeRule(NodeType);//支持泛型法则
				self.SupportPolymorphicListenerRule(NodeType);//支撑继承监听法则
				self.SupportPolymorphicRule(NodeType);//支撑继承法则
				self.SupportTypeHash.Add(NodeType);//已支持名单
			}
		}

		#region 法则泛型

		/// <summary>
		/// 支持泛型节点法则
		/// </summary>
		/// <remarks>
		/// <para>将会通过反射查询自身及所有父类是否有泛型</para>
		/// </remarks>
		public static void SupportGenericNodeRule(this RuleManager self, long NodeType)
		{
			Type Type = NodeType.CodeToType();
			while (Type != null && Type != typeof(object))
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
		public static void SupportPolymorphicListenerRule(this RuleManager self, long listenerNodeType)
		{
			//监听器父类类型键值
			Type listenerBaseType = listenerNodeType.CodeToType().BaseType;
			//类型哈希码
			long listenerBaseTypeCodeKey = listenerBaseType.TypeToCode();

			while (listenerBaseType != null && listenerBaseType != typeof(object))
			{
				self.PolymorphicListenerRule(listenerNodeType, listenerBaseTypeCodeKey);
				listenerBaseType = listenerBaseType.BaseType;
				listenerBaseTypeCodeKey = listenerBaseType.TypeToCode();
			}
			if (listenerBaseType.GetInterfaces().Contains(TypeInfo<INode>.Type))
				self.PolymorphicListenerRule(listenerNodeType, TypeInfo<INode>.TypeCode);
		}

		/// <summary>
		/// 多态化一个节点类型的监听法则
		/// </summary>
		private static void PolymorphicListenerRule(this RuleManager self, long listenerNodeType, long listenerBaseTypeCodeKey)
		{
			//判断父类是否有法则，没有则退出
			if (!self.ListenerRuleTargetGroupDictionary.TryGetValue(listenerBaseTypeCodeKey, out var RuleType_TargerGroupDictionary)) return;

			//拿到节点自身的：法则类型 《目标节点类型,监听法则》
			Dictionary<long, RuleGroup> NodeRuleType_TargerGroupDictionary = self.ListenerRuleTargetGroupDictionary.GetValue(listenerNodeType);

			//动态监听法则多态记录
			if (self.DynamicListenerTypeHash.Contains(listenerBaseTypeCodeKey))
			{
				if (!self.DynamicListenerTypeHash.Contains(listenerNodeType))
					self.DynamicListenerTypeHash.Add(listenerNodeType);
			}

			//K:法则类型 , V:《目标节点类型,监听法则列表》
			foreach (var RuleType_TargetGroupKV in RuleType_TargerGroupDictionary)
			{
				//自身已经存在的法则则跳过
				if (NodeRuleType_TargerGroupDictionary.ContainsKey(RuleType_TargetGroupKV.Key)) continue;

				//父类的监听法则添加到自身，也就是继承法则功能
				NodeRuleType_TargerGroupDictionary.Add(RuleType_TargetGroupKV.Key, RuleType_TargetGroupKV.Value);

				//接下来补齐 ListenerRuleTargetGroupDictionary 对应的 TargetRuleListenerGroupDictionary
				//K:目标节点类型 , V:监听法则列表
				foreach (var TargetType_RuleListKV in RuleType_TargetGroupKV.Value)
				{
					//目标类型为主的字典， 进行目标类型查找
					if (!self.TargetRuleListenerGroupDictionary.TryGetValue(TargetType_RuleListKV.Key, out var RuleType_ListenerGroupDictionary)) continue;

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
		public static void SupportPolymorphicRule(this RuleManager self, long NodeType)
		{
			//开始遍历查询父类型法则
			Type BaseType = NodeType.CodeToType().BaseType;
			//父类型哈希码
			long BaseTypeCodeKey = BaseType.TypeToCode();

			while (BaseType != null && BaseType != typeof(object))
			{
				self.PolymorphicRule(NodeType, BaseTypeCodeKey);
				BaseType = BaseType.BaseType;
				BaseTypeCodeKey = BaseType.TypeToCode();
			}
			//检测是否继承了INode 接口,支持INode的多态法则
			if (BaseType.GetInterfaces().Contains(TypeInfo<INode>.Type))
				self.PolymorphicRule(NodeType, TypeInfo<INode>.TypeCode);
		}

		/// <summary>
		/// 多态化一个节点类型的法则
		/// </summary>
		private static void PolymorphicRule(this RuleManager self, long NodeType, long BaseTypeCodeKey)
		{
			//判断父类是否有法则，没有则退出
			if (!self.NodeTypeRulesDictionary.TryGetValue(BaseTypeCodeKey, out var BaseRuleHash)) return;

			//拿到节点类型的法则哈希表
			HashSet<long> ruleTypeHash = self.NodeTypeRulesDictionary.GetValue(NodeType);

			//遍历父类型法则
			foreach (var ruleType in BaseRuleHash)
			{
				//存在的法则则跳过
				if (ruleTypeHash.Contains(ruleType)) continue;

				ruleTypeHash.Add(ruleType);

				//法则字典的补充
				if (!self.RuleGroupDictionary.TryGetValue(ruleType, out var RuleGroup)) continue;

				//获取父类型法则列表
				if (!RuleGroup.TryGetValue(BaseTypeCodeKey, out var ruleList)) continue;

				//法则列表添加进节点类型
				RuleGroup.TryAdd(NodeType, ruleList);
			}
		}

		#endregion

		#endregion

		#region 法则获取

		#region 获取监听目标法则组

		/// <summary>
		/// 获取监听目标法则组
		/// </summary>
		public static bool TryGetTargetRuleGroup<LR>(this RuleManager self, long targetType, out IRuleGroup<LR> ruleGroup)
			where LR : IListenerRule
		{
			if (self.TryGetTargetRuleGroup(TypeInfo<LR>.TypeCode, targetType, out var RuleGroup))
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
		public static bool TryGetTargetRuleGroup<LR>(this RuleManager self, long targetType, out RuleGroup ruleGroup)
			where LR : IListenerRule
		{
			return self.TryGetTargetRuleGroup(TypeInfo<LR>.TypeCode, targetType, out ruleGroup);
		}

		/// <summary>
		/// 获取监听目标法则组
		/// </summary>
		public static bool TryGetTargetRuleGroup(this RuleManager self, long ruleType, long targetType, out RuleGroup ruleGroup)
		{
			if (self.TargetRuleListenerGroupDictionary.TryGetValue(targetType, out var ruleGroupDictionary))
			{
				return ruleGroupDictionary.TryGetValue(ruleType, out ruleGroup);
			}
			ruleGroup = null;
			return false;
		}

		/// <summary>
		/// 强制获取占位目标法则组
		/// </summary>
		public static RuleGroup GetOrNewTargetRuleGroup<LR>(this RuleManager self, long targetType)
			where LR : IListenerRule
		{
			return self.GetOrNewTargetRuleGroup(TypeInfo<LR>.TypeCode, targetType);
		}

		/// <summary>
		/// 强制获取占位目标法则组
		/// </summary>
		public static RuleGroup GetOrNewTargetRuleGroup(this RuleManager self, long ruleType, long targetType)
		{
			if (self.TargetRuleListenerGroupDictionary.TryGetValue(targetType, out var ruleGroupDictionary))
			{
				if (ruleGroupDictionary.TryGetValue(ruleType, out var ruleGroup))
				{
					return ruleGroup;
				}
				else
				{
					ruleGroup = ruleGroupDictionary.GetValue(ruleType);
					ruleGroup.RuleType = ruleType;
					return ruleGroup;
				}
			}
			else
			{
				ruleGroupDictionary = self.TargetRuleListenerGroupDictionary.GetValue(targetType);
				RuleGroup ruleGroup = ruleGroupDictionary.GetValue(ruleType);
				ruleGroup.RuleType = ruleType;
				return ruleGroup;
			}
		}

		#endregion

		#region 获取监听目标法则列表

		/// <summary>
		/// 获取监听目标法则列表
		/// </summary>
		public static bool TryGetTargetRuleList<LR>(this RuleManager self, long targetType, out IRuleList<LR> ruleList)
			where LR : IListenerRule
		{
			if (self.TargetRuleListenerGroupDictionary.TryGetValue(targetType, out var ruleGroupDictionary))
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
		public static bool TryGetListenerRuleGroup<LR>(this RuleManager self, long listenerType, out IRuleGroup<LR> ruleGroup)
			where LR : IListenerRule
		{
			if (self.ListenerRuleTargetGroupDictionary.TryGetValue(listenerType, out var ruleGroupDictionary))
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
		public static bool TryGetListenerRuleList<LR>(this RuleManager self, long listenerType, long targetType, out IRuleList<LR> ruleList)
			where LR : IListenerRule
		{
			if (self.ListenerRuleTargetGroupDictionary.TryGetValue(listenerType, out var ruleGroupDictionary))
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
		public static bool TryGetRuleGroup<R>(this RuleManager self, out IRuleGroup<R> ruleGroup)
		 where R : IRule
		{
			if (self.TryGetRuleGroup(TypeInfo<R>.TypeCode, out var RuleGroup))
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
		public static bool TryGetRuleGroup<R>(this RuleManager self, out RuleGroup ruleGroup)
		 where R : IRule
		{
			return self.TryGetRuleGroup(TypeInfo<R>.TypeCode, out ruleGroup);
		}

		/// <summary>
		/// 尝试获取法则组
		/// </summary>
		public static bool TryGetRuleGroup(this RuleManager self, long ruleType, out RuleGroup ruleGroup)
		{
			return self.RuleGroupDictionary.TryGetValue(ruleType, out ruleGroup);
		}

		/// <summary>
		/// 强制获取占位法则组
		/// </summary>
		public static RuleGroup GetOrNewRuleGroup<R>(this RuleManager self)
		 where R : IRule
		{
			return self.GetOrNewRuleGroup(TypeInfo<R>.TypeCode);
		}

		/// <summary>
		/// 强制获取占位法则组
		/// </summary>
		public static RuleGroup GetOrNewRuleGroup(this RuleManager self, long ruleType)
		{
			var group = self.RuleGroupDictionary.GetValue(ruleType);
			group.RuleType = ruleType;
			return group;
		}

		#endregion

		#region  获取法则列表

		/// <summary>
		/// 获取单类型法则列表
		/// </summary>
		public static bool TryGetRuleList<R>(this RuleManager self, long nodeType, out IRuleList<R> ruleList)
		 where R : IRule
		{
			if (self.TryGetRuleList<R>(nodeType, out RuleList rules))
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
		public static bool TryGetRuleList(this RuleManager self, long nodeType, long ruleType, out RuleList ruleList)
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

		/// <summary>
		/// 获取单类型法则列表
		/// </summary>
		public static bool TryGetRuleList<R>(this RuleManager self, long nodeType, out RuleList ruleList)
		 where R : IRule
		{
			if (self.RuleGroupDictionary.TryGetValue(TypeInfo<R>.TypeCode, out RuleGroup ruleGroup))
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
		public static RuleList GetOrNewRuleList<R>(this RuleManager self, long nodeType)
		 where R : IRule
		{
			return self.GetOrNewRuleList(nodeType, TypeInfo<R>.TypeCode);
		}

		/// <summary>
		/// 强制获取占位法则列表
		/// </summary>
		public static RuleList GetOrNewRuleList(this RuleManager self, long nodeType, long ruleType)
		{
			if (self.RuleGroupDictionary.TryGetValue(ruleType, out RuleGroup ruleGroup))
			{
				if (ruleGroup.TryGetValue(nodeType, out RuleList ruleList))
				{
					return ruleList;
				}
				else
				{
					ruleList = ruleGroup.GetValue(nodeType);
					ruleList.RuleType = ruleType;
					return ruleList;
				}
			}
			else
			{
				ruleGroup = self.RuleGroupDictionary.GetValue(ruleType);
				ruleGroup.RuleType = ruleType;
				var ruleList = ruleGroup.GetValue(nodeType);
				ruleList.RuleType = ruleType;
				return ruleList;
			}
		}

		#endregion

		#endregion
	}
}