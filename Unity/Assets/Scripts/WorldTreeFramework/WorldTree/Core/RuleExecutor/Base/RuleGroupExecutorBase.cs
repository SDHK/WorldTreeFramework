/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/1 19:44

* 描述： 法则集合执行器基类
*
* 执行拥有指定法则的节点

 */

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 法则集合执行器基类
	/// </summary>
	public abstract class RuleGroupExecutorBase : Node, IGlobalRuleExecutor, IRuleExecutorEnumerable
		, AsChildBranch
	{
		/// <summary>
		/// 单法则集合
		/// </summary>
		[Protected] public RuleGroup ruleGroupDict;

		/// <summary>
		/// 节点列表
		/// </summary>
		public UnitList<NodeRef<INode>> nodeList;

		/// <summary>
		/// 节点下一个列表
		/// </summary>
		public UnitList<NodeRef<INode>> nodeNextList;

		/// <summary>
		/// 委托列表
		/// </summary>
		public UnitList<RuleList> delegateList;

		/// <summary>
		/// 委托下一个列表
		/// </summary>
		public UnitList<RuleList> delegateNextList;

		/// <summary>
		/// 节点Id索引字典
		/// </summary>
		public UnitDictionary<long, int> IdIndexDict;

		/// <summary>
		/// 节点Id下一个索引字典
		/// </summary>
		public UnitDictionary<long, int> IdIndexNextDict;

		/// <summary>
		/// 遍历下标
		/// </summary>
		public int nodeIndex;

		public int TraversalCount => nodeList.Count;

		public int RefreshTraversalCount()
		{
			//交换列表
			nodeIndex = 0;
			(nodeList, nodeNextList) = (nodeNextList, nodeList);
			(delegateList, delegateNextList) = (delegateNextList, delegateList);
			(IdIndexDict, IdIndexNextDict) = (IdIndexNextDict, IdIndexDict);
			nodeNextList.Clear();
			delegateNextList.Clear();
			IdIndexNextDict.Clear();
			return nodeList.Count;
		}


		public override string ToString()
		{
			return $"RuleGroupExecutor : {(ruleGroupDict == null ? null : Core.CodeToType(ruleGroupDict.RuleType))}";
		}

		public void Clear()
		{
			nodeList?.Clear();
			nodeNextList?.Clear();
			delegateList?.Clear();
			delegateNextList?.Clear();
			IdIndexDict?.Clear();
			IdIndexNextDict?.Clear();
			nodeIndex = 0;
		}

		public bool TryAdd(INode node)
		{
			//节点存在则不允许重复添加。
			//如果节点是意外回收了，那么Id是递增的不再出现，也就是那个回收的Id已经被永久销毁了，同样禁止添加。
			if (IdIndexDict != null && IdIndexDict.ContainsKey(node.InstanceId)) return false;
			if (IdIndexNextDict != null && IdIndexNextDict.ContainsKey(node.InstanceId)) return false;
			if (ruleGroupDict == null || !ruleGroupDict.TryGetValue(node.Type, out RuleList ruleList)) return false;
			nodeNextList.Add(new NodeRef<INode>(node, false));
			delegateNextList.Add(ruleList);
			IdIndexNextDict.Add(node.InstanceId, nodeNextList.Count - 1);
			return true;
		}



		public void Remove(long id)
		{
			//下一次遍历列表
			if (IdIndexNextDict.TryGetValue(id, out int index))
			{
				//不是遍历列表，直接快速移除
				RemoveAtSwapBack(nodeNextList, index);
				RemoveAtSwapBack(delegateNextList, index);
				IdIndexNextDict.Remove(id);
			}
			//正在遍历的列表
			else if (IdIndexDict.TryGetValue(id, out index))
			{
				IdIndexDict.Remove(id);
				nodeList[index].Clear();//清除引用，让其变成意外回收
			}
		}

		public void Remove(INode node) => Remove(node.Id);
		/// <summary>
		/// 移动到最后并删除
		/// </summary>
		public void RemoveAtSwapBack<T>(List<T> list, int index)
		{
			int tail = list.Count - 1;
			if (index != tail)
				list[index] = list[tail];
			list.RemoveAt(tail);
		}


		/// <summary>
		/// 尝试获取队顶
		/// </summary>
		public bool TryPeek(out INode node, out RuleList ruleList)
		{
			while (nodeList.Count > nodeIndex)
			{
				node = nodeList[nodeIndex].Value;
				if (node == null) // 节点意外回收
				{
					nodeIndex++;
				}
				else // 节点存在
				{
					ruleList = delegateList[nodeIndex];
					return true;
				}
			}
			node = null;
			ruleList = null;
			return false;
		}

		/// <summary>
		/// 尝试出列
		/// </summary>
		public bool TryDequeue(out INode node, out RuleList ruleList)
		{
			NodeRef<INode> nodeRef;
			while (nodeList.Count > nodeIndex)
			{
				nodeRef = nodeList[nodeIndex];
				node = nodeRef.Value;
				if (node == null) // 节点意外回收
				{
					nodeIndex++;
				}
				else // 节点存在
				{
					ruleList = delegateList[nodeIndex];
					nodeIndex++;
					nodeNextList.Add(nodeRef); // 塞回队列用于下次遍历
					delegateNextList.Add(ruleList);
					IdIndexDict.Remove(node.InstanceId);//索引字典移动
					IdIndexNextDict.Add(node.InstanceId, nodeNextList.Count - 1);
					return true;
				}
			}
			node = null;
			ruleList = null;
			return false;
		}

	}

	public static class RuleGroupExecutorBaseRule
	{
		private class Add : AddRule<RuleGroupExecutorBase>
		{
			protected override void Execute(RuleGroupExecutorBase self)
			{
				self.Core.PoolGetUnit(out self.nodeList);
				self.Core.PoolGetUnit(out self.nodeNextList);
				self.Core.PoolGetUnit(out self.delegateList);
				self.Core.PoolGetUnit(out self.delegateNextList);
				self.Core.PoolGetUnit(out self.IdIndexDict);
				self.Core.PoolGetUnit(out self.IdIndexNextDict);
			}
		}

		class RemoveRule : RemoveRule<RuleGroupExecutorBase>
		{
			protected override void Execute(RuleGroupExecutorBase self)
			{
				self.nodeIndex = 0;
				self.nodeList.Dispose();
				self.nodeNextList.Dispose();
				self.delegateList.Dispose();
				self.delegateNextList.Dispose();
				self.IdIndexDict.Dispose();
				self.IdIndexNextDict.Dispose();
				self.nodeList = null;
				self.nodeNextList = null;
				self.delegateList = null;
				self.delegateNextList = null;
				self.IdIndexDict = null;
				self.IdIndexNextDict = null;
			}
		}
	}
}
