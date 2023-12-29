/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/4 20:34

* 描述： 混合型监听器法则执行器组

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 混合型监听器法则执行器组
	/// </summary>
	public class HybridListenerRuleActuatorGroup : Node, IListenerIgnorer, ComponentOf<ReferencedPool>
		, AsRule<IAwakeRule>
	{
		/// <summary>
		/// 监听器执行器字典集合
		/// </summary>
		public UnitDictionary<long, HybridListenerRuleActuator> actuatorDictionary = new();
	}

	public static class HybridListenerRuleActuatorGroupRule
	{

		/// <summary>
		/// 获取节点监听执行器
		/// </summary>
		public static IRuleActuator<R> GetListenerActuator<R>(this INode node)
		   where R : IListenerRule
		{
			if (node.Core.ReferencedPoolManager != null)
			{
				if (node.Core.ReferencedPoolManager.TryGetPool(node.Type, out ReferencedPool nodePool))
				{
					if (nodePool.AddComponent(out HybridListenerRuleActuatorGroup _, isPool: false).TryAddRuleActuator(node.Type, out IRuleActuator<R> actuator))
					{
						return actuator;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 添加监听执行器,并自动填装监听器
		/// </summary>
		public static bool TryAddRuleActuator<R>(this HybridListenerRuleActuatorGroup self, long nodeType, out IRuleActuator<R> actuator)
			where R : IListenerRule
		{
			long ruleType = TypeInfo<R>.TypeCode;

			//执行器已存在，直接返回
			if (self.actuatorDictionary.TryGetValue(ruleType, out HybridListenerRuleActuator ruleActuator))
			{
				actuator = ruleActuator as IRuleActuator<R>;
				//检测是否存在目标法则集合，不存在则添加
				if (ruleActuator.staticListenerRuleActuator == null)
				{
					if (self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, nodeType, out RuleGroup staticRuleGroup1))
					{
						ruleActuator.AddComponent(out ruleActuator.staticListenerRuleActuator, staticRuleGroup1).RuleActuatorAddListener();
					}
				}
				if (ruleActuator.dynamicListenerRuleActuator == null)
				{
					if (self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, TypeInfo<INode>.TypeCode, out RuleGroup dynamicRuleGroup1))
					{
						ruleActuator.AddComponent(out ruleActuator.dynamicListenerRuleActuator, dynamicRuleGroup1).RuleActuatorAddListener(nodeType);
					}
				}
				return true;
			}
			//执行器不存在，检测获取目标法则集合，并新建执行器
			bool checkStatic = self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, nodeType, out RuleGroup staticRuleGroup);
			bool checkDynamic = self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, TypeInfo<INode>.TypeCode, out RuleGroup dynamicRuleGroup);
			if (checkStatic || checkDynamic)
			{
				self.actuatorDictionary.Add(ruleType, self.AddComponent(out ruleActuator, isPool: false));
				if (checkStatic) ruleActuator.AddComponent(out ruleActuator.staticListenerRuleActuator, staticRuleGroup).RuleActuatorAddListener();
				if (checkDynamic) ruleActuator.AddComponent(out ruleActuator.dynamicListenerRuleActuator, dynamicRuleGroup).RuleActuatorAddListener(nodeType);
			}

			actuator = ruleActuator as IRuleActuator<R>;
			return actuator != null;
		}


		#region 静态监听器

		/// <summary>
		/// 尝试添加监听器监听目标池
		/// </summary>
		/// <remarks>只在目标池存在时有效</remarks>
		public static void TryAddStaticListener(this ReferencedPoolManager self, INodeListener listener)
		{
			//判断是否为监听器
			if (self.Core.RuleManager.ListenerRuleTargetGroupDictionary.TryGetValue(listener.Type, out var ruleGroupDictionary))
			{
				foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
				{
					//判断监听法则集合 是否有这个 监听器节点类型
					if (ruleGroup.Value.ContainsKey(listener.Type))
					{
						foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
						{
							//是否有这个目标缓存池
							if (self.TryGetPool(ruleList.Key, out ReferencedPool nodePool))
							{
								//是否有监听器集合组件
								if (nodePool.TryGetComponent(out HybridListenerRuleActuatorGroup ListenerRuleActuatorGroup))
								{
									//是否有这个监听类型的执行器
									if (ListenerRuleActuatorGroup.actuatorDictionary.TryGetValue(ruleGroup.Key, out HybridListenerRuleActuator listenerRuleActuator))
									{
										//监听器添加到执行器
										listenerRuleActuator.staticListenerRuleActuator?.TryAdd(listener);
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 移除这个监听器
		/// </summary>
		/// <remarks>只在目标池存在时有效</remarks>
		public static void RemoveStaticListener(this ReferencedPoolManager self, INodeListener listener)
		{
			//判断是否为监听器
			if (self.Core.RuleManager.ListenerRuleTargetGroupDictionary.TryGetValue(listener.Type, out var ruleGroupDictionary))
			{
				foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
				{
					//判断监听法则集合 是否有这个 监听器节点类型
					if (ruleGroup.Value.ContainsKey(listener.Type))
					{
						foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
						{
							//是否有这个目标池
							if (self.TryGetPool(ruleList.Key, out ReferencedPool nodePool))
							{
								//是否有监听器集合组件
								if (nodePool.TryGetComponent(out HybridListenerRuleActuatorGroup ListenerRuleActuatorGroup))
								{
									//是否有这个监听类型的执行器
									if (ListenerRuleActuatorGroup.actuatorDictionary.TryGetValue(ruleGroup.Key, out HybridListenerRuleActuator listenerRuleActuator))
									{
										//监听器添加到执行器
										listenerRuleActuator.staticListenerRuleActuator?.Remove(listener);
									}
								}
							}
						}
					}
				}
			}
		}


		#endregion

		#region 动态监听器

		#region 添加

		/// <summary>
		/// 监听器根据标记添加目标
		/// </summary>
		public static void TryAddDynamicListener(this ReferencedPoolManager self, IDynamicNodeListener node)
		{
			if (node.listenerTarget != 0)
			{
				if (node.listenerState == ListenerState.Node)
				{
					if (node.listenerTarget == TypeInfo<INode>.TypeCode)
					{
						self.AddAllTarget(node);
					}
					else
					{
						self.AddNodeTarget(node, node.listenerTarget);
					}
				}
				else if (node.listenerState == ListenerState.Rule)
				{
					self.AddRuleTarget(node, node.listenerTarget);
				}
			}

		}
		/// <summary>
		/// 监听器添加 所有节点
		/// </summary>
		private static void AddAllTarget(this ReferencedPoolManager self, IDynamicNodeListener node)
		{
			//获取 INode 动态目标 法则集合集合
			if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(TypeInfo<INode>.TypeCode, out Dictionary<long, RuleGroup> ruleGroupDictionary))
			{
				//遍历获取动态法则集合
				foreach (var ruleGroupPair in ruleGroupDictionary)
				{
					//判断监听法则集合 是否有这个 监听器节点类型
					if (ruleGroupPair.Value.ContainsKey(node.Type))
					{
						//遍历现有池
						foreach (var poolPair in self.pools)
						{
							//尝试获取动态执行器集合组件
							if (poolPair.Value.TryGetComponent(out HybridListenerRuleActuatorGroup ListenerRuleActuatorGroup))
							{
								//从执行器集合 提取这个 监听法则类型 的监听执行器，进行添加
								if (ListenerRuleActuatorGroup.actuatorDictionary.TryGetValue(ruleGroupPair.Key, out var ruleActuator))
								{
									ruleActuator.dynamicListenerRuleActuator?.TryAdd(node);
								}
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// 监听器添加 法则目标
		/// </summary>
		private static void AddRuleTarget(this ReferencedPoolManager self, IDynamicNodeListener node, long targetRuleType)
		{
			//获取法则集合
			if (node.Core.RuleManager.TryGetRuleGroup(targetRuleType, out var targetRuleGroup))
			{
				//遍历法则集合
				foreach (var targetNode_RuleList in targetRuleGroup)
				{
					self.AddNodeTarget(node, targetNode_RuleList.Key);
				}
			}

		}

		/// <summary>
		/// 监听器添加 节点目标
		/// </summary>
		private static void AddNodeTarget(this ReferencedPoolManager self, IDynamicNodeListener node, long listenerTarget)
		{
			if (self.TryGetPool(listenerTarget, out ReferencedPool listenerPool))
			{
				if (listenerPool.TryGetComponent(out HybridListenerRuleActuatorGroup ListenerRuleActuatorGroup))
				{
					//获取 INode 动态目标 法则集合集合
					if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(TypeInfo<INode>.TypeCode, out var ruleGroupDictionary))
					{
						//遍历获取动态法则集合，并添加自己
						foreach (var ruleGroup in ruleGroupDictionary)
						{
							//判断监听法则集合 是否有这个 监听器节点类型
							if (ruleGroup.Value.ContainsKey(node.Type))
							{
								if (ListenerRuleActuatorGroup.actuatorDictionary.TryGetValue(ruleGroup.Key, out HybridListenerRuleActuator ruleActuator))
								{
									ruleActuator.dynamicListenerRuleActuator?.TryAdd(node);
								}
							}
						}
					}
				}
			}
		}


		#endregion

		#region 移除

		/// <summary>
		/// 监听器根据标记移除目标
		/// </summary>
		public static void RemoveDynamicListener(this ReferencedPoolManager self, IDynamicNodeListener node)
		{
			if (node.listenerTarget != 0)
			{
				if (node.listenerState == ListenerState.Node)
				{
					if (node.listenerTarget == TypeInfo<INode>.TypeCode)
					{
						self.RemoveAllTarget(node);
					}
					else
					{
						self.RemoveNodeTarget(node, node.listenerTarget);
					}
				}
				else if (node.listenerState == ListenerState.Rule)
				{
					self.RemoveRuleTarget(node, node.listenerTarget);
				}
				node.listenerTarget = 0;
				node.listenerState = ListenerState.Not;
			}
		}

		/// <summary>
		/// 监听器移除 所有节点
		/// </summary>
		private static void RemoveAllTarget(this ReferencedPoolManager self, IDynamicNodeListener node)
		{
			//获取 INode 动态目标 法则集合集合
			if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(TypeInfo<INode>.TypeCode, out var ruleGroupDictionary))
			{
				//遍历获取动态法则集合
				foreach (var ruleGroupPair in ruleGroupDictionary)
				{
					//判断监听法则集合 是否有这个 监听器节点类型
					if (ruleGroupPair.Value.ContainsKey(node.Type))
					{
						//遍历现有池
						foreach (var poolPair in self.pools)
						{
							//尝试获取动态执行器集合组件
							if (poolPair.Value.TryGetComponent(out HybridListenerRuleActuatorGroup ListenerRuleActuatorGroup))
							{
								//从执行器集合 提取这个 监听法则类型 的监听执行器，进行移除
								if (ListenerRuleActuatorGroup.actuatorDictionary.TryGetValue(ruleGroupPair.Key, out var ruleActuator))
								{
									ruleActuator.dynamicListenerRuleActuator?.Remove(node);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 监听器移除 系统目标
		/// </summary>
		private static void RemoveRuleTarget(this ReferencedPoolManager self, IDynamicNodeListener node, long targetRuleType)
		{
			//获取法则集合
			if (node.Core.RuleManager.TryGetRuleGroup(targetRuleType, out var targetRuleGroup))
			{
				//遍历法则集合
				foreach (var targetNode_RuleList in targetRuleGroup)
				{
					self.RemoveNodeTarget(node, targetNode_RuleList.Key);
				}
			}
		}


		/// <summary>
		/// 监听器移除 节点目标
		/// </summary>
		private static void RemoveNodeTarget(this ReferencedPoolManager self, IDynamicNodeListener node, long listenerTarget)
		{
			if (self.TryGetPool(listenerTarget, out ReferencedPool listenerPool))
			{
				if (listenerPool.TryGetComponent(out HybridListenerRuleActuatorGroup ListenerRuleActuatorGroup))
				{
					//获取 INode 动态目标 法则集合集合
					if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(TypeInfo<INode>.TypeCode, out var ruleGroupDictionary))
					{
						//遍历获取动态法则集合，移除自己
						foreach (var ruleGroup in ruleGroupDictionary)
						{
							//判断监听法则集合 是否有这个 监听器节点类型
							if (ruleGroup.Value.ContainsKey(node.Type))
							{
								if (ListenerRuleActuatorGroup.actuatorDictionary.TryGetValue(ruleGroup.Key, out HybridListenerRuleActuator ruleActuator))
								{
									ruleActuator.dynamicListenerRuleActuator?.Remove(node);
								}
							}
						}
					}
				}
			}
		}


		#endregion

		#endregion




	}



}
