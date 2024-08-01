/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/1 19:44

* 描述： 法则集合执行器基类
*
* 执行拥有指定法则的节点

 */

using System;

namespace WorldTree
{
	/// <summary>
	/// 法则集合执行器基类
	/// </summary>
	public abstract class RuleGroupActuatorBase : Node, IRuleGroupActuator, IRuleActuatorEnumerable
		, AsChildBranch
	{
		/// <summary>
		/// 单法则集合
		/// </summary>
		[Protected] public RuleGroup ruleGroupDict;

		/// <summary>
		/// 节点法则队列
		/// </summary>
		public TreeQueue<ValueTuple<NodeRef<INode>, RuleList>> nodeRuleQueue;

		/// <summary>
		/// 节点Id字典
		/// </summary>
		/// <remarks>确保同一时刻不允许有两个相同的节点</remarks>
		public TreeHashSet<long> nodeIdHash;

		/// <summary>
		/// 节点id被移除的次数
		/// </summary>
		/// <remarks>队列是无法移除任意位置的，所以需要记录，并在获取时抵消</remarks>
		public TreeDictionary<long, int> removeIdDict;

		/// <summary>
		/// 动态的遍历数量
		/// </summary>
		/// <remarks>当遍历时移除后，在发生抵消的时候减少数量</remarks>
		private int traversalCount;

		public int TraversalCount => traversalCount;

		public int RefreshTraversalCount() => traversalCount = nodeRuleQueue is null ? 0 : nodeRuleQueue.Count;

		public override string ToString()
		{
			return $"RuleGroupActuator : {ruleGroupDict?.RuleType.CodeToType()}";
		}

		public bool TryAdd(INode node)
		{
			//节点存在则不允许重复添加。
			//如果节点是意外回收了，那么Id是递增的不再出现，也就是那个回收的Id已经被永久销毁了，同样禁止添加。
			if (nodeIdHash != null && nodeIdHash.Contains(node.Id)) return false;
			if (ruleGroupDict == null || !ruleGroupDict.TryGetValue(node.Type, out RuleList ruleList)) return false;
			nodeIdHash ??= this.AddChild(out nodeIdHash);
			nodeRuleQueue ??= this.AddChild(out nodeRuleQueue);
			NodeRef<INode> nodeRef = new(node);
			nodeRuleQueue.Enqueue((nodeRef, ruleList));
			nodeIdHash.Add(node.Id);
			return true;
		}

		public void Clear()
		{
			nodeRuleQueue?.Clear();
			nodeIdHash?.Clear();
			removeIdDict?.Clear();
			traversalCount = 0;
		}

		public void Remove(long id)
		{
			if (nodeIdHash != null && nodeIdHash.Contains(id))
			{
				nodeIdHash.Remove(id);

				//累计强制移除的节点id
				removeIdDict ??= this.AddChild(out removeIdDict);
				if (removeIdDict.TryGetValue(id, out var count))
				{
					removeIdDict[id] = count + 1;
				}
				else
				{
					removeIdDict.Add(id, 1);
				}
			}
		}

		public void Remove(INode node) => Remove(node.Id);

	

		/// <summary>
		/// 尝试获取队顶
		/// </summary>
		public bool TryPeek(out INode node, out RuleList ruleList)
		{
			do
			{
				if (nodeRuleQueue != null && nodeRuleQueue.TryPeek(out (NodeRef<INode>, RuleList) valueRef))
				{
					long id = valueRef.Item1.NodeId;

					//假如id被回收了
					if (removeIdDict != null && removeIdDict.TryGetValue(id, out int count))
					{
						//回收次数抵消
						removeIdDict[id] = --count;
						if (count == 0) removeIdDict.Remove(id);// 次数为0时删除id
						if (removeIdDict.Count == 0)//假如字典空了,则释放
						{
							removeIdDict.Dispose();
							removeIdDict = null;
						}

						if (traversalCount > 0) traversalCount--;
						nodeRuleQueue.Dequeue();//移除
					}
					else
					{
						node = valueRef.Item1.Value;
						if (node == null)//节点意外回收
						{
							//字典移除节点Id，节点回收后id改变了，而id是递增，绝对不会再出现的。
							nodeIdHash.Remove(id);
							if (traversalCount != 0) traversalCount--; //遍历数抵消
							nodeRuleQueue.Dequeue();//移除
						}
						else
						{
							ruleList = valueRef.Item2;
							return true;
						}
					}
				}
				else
				{
					node = null;
					ruleList = null;
					return false;
				}
			} while (true);
		}

		/// <summary>
		/// 尝试出列
		/// </summary>
		public bool TryDequeue(out INode node, out RuleList ruleList)
		{
			//从队列里拿到id
			if (nodeRuleQueue.TryDequeue(out (NodeRef<INode>, RuleList) nodeRuleTuple))
			{
				while (true)
				{
					long id = nodeRuleTuple.Item1.NodeId;

					//假如id被主动移除了
					if (removeIdDict != null && removeIdDict.TryGetValue(id, out int count))
					{
						removeIdDict[id] = --count;//回收次数抵消
						if (count == 0) removeIdDict.Remove(id);//次数为0时删除id
						if (removeIdDict.Count == 0)//假如字典空了,则释放
						{
							removeIdDict.Dispose();
							removeIdDict = null;
						}

						if (traversalCount != 0) traversalCount--; //遍历数抵消

						//获取下一个id,假如队列空了,则直接返回退出
						if (!nodeRuleQueue.TryDequeue(out nodeRuleTuple))
						{
							node = null;
							ruleList = null;
							return false;
						}
					}
					else
					{
						node = nodeRuleTuple.Item1.Value;

						if (node == null)//节点意外回收
						{
							//字典移除节点Id，节点回收后id改变了，而id是递增，绝对不会再出现的。
							nodeIdHash.Remove(id);

							if (traversalCount != 0) traversalCount--; //遍历数抵消

							//获取下一个id,假如队列空了,则直接返回退出
							if (!nodeRuleQueue.TryDequeue(out nodeRuleTuple))
							{
								ruleList = null;
								return false;
							}
						}
						else//节点存在
						{
							nodeRuleQueue.Enqueue(nodeRuleTuple);//塞回队列用于下次遍历
							ruleList = nodeRuleTuple.Item2;
							return true;
						}
					}
				}
			}
			node = null;
			ruleList = null;
			return false;
		}



	}

	public static class RuleGroupActuatorBaseRule
	{
		class RemoveRule : RemoveRule<RuleGroupActuatorBase>
		{
			protected override void Execute(RuleGroupActuatorBase self)
			{
				self.ruleGroupDict = null;
				self.Clear();
				self.nodeRuleQueue = null;
				self.removeIdDict = null;
				self.nodeIdHash = null;
			}
		}
	}
}
