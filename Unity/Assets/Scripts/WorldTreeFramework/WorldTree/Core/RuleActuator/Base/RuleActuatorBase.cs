/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/11 11:52:09

* 描述： 法则执行器基类
*
* 这是一个可以在遍历时，增加和移除节点的队列遍历器。
* 在遍历时，如果增加了节点，那么会在下一次遍历时执行。
*
* 如果移除了节点，那么就会被抵消跳过，不会执行。
* 如果节点被意外回收了，那么也会被跳过，不会执行。
* 

*/

using System.Collections.Generic;

namespace WorldTree
{

	/// <summary>
	/// 法则执行器基类
	/// </summary>
	public abstract class RuleActuatorBase : Node, IRuleListActuator, IRuleActuatorEnumerable
		, AsChildBranch
	{

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


		public bool TryAdd(INode node, RuleList func)
		{
			if (IdIndexDict != null && IdIndexDict.ContainsKey(node.InstanceId)) return false;
			if (IdIndexNextDict != null && IdIndexNextDict.ContainsKey(node.InstanceId)) return false;
			nodeNextList.Add(new NodeRef<INode>(node, false));
			delegateNextList.Add(func);
			IdIndexNextDict.Add(node.InstanceId, nodeNextList.Count - 1);
			return true;
		}

		public bool TryDequeue(out INode node, out RuleList func)
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
					func = delegateList[nodeIndex];
					nodeIndex++;
					nodeNextList.Add(nodeRef); // 塞回队列用于下次遍历
					delegateNextList.Add(func);
					IdIndexDict.Remove(node.InstanceId);//索引字典移动
					IdIndexNextDict.Add(node.InstanceId, nodeNextList.Count - 1);
					return true;
				}
			}
			node = null;
			func = null;
			return false;
		}

		public bool TryPeek(out INode node, out RuleList func)
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
					func = delegateList[nodeIndex];
					return true;
				}
			}
			node = null;
			func = null;
			return false;
		}


		public void Remove(INode node) => Remove(node.InstanceId);



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
	}

	public static class RuleActuatorBaseRule
	{
		private class Add : AddRule<RuleActuatorBase>
		{
			protected override void Execute(RuleActuatorBase self)
			{
				self.Core.PoolGetUnit(out self.nodeList);
				self.Core.PoolGetUnit(out self.nodeNextList);
				self.Core.PoolGetUnit(out self.delegateList);
				self.Core.PoolGetUnit(out self.delegateNextList);
				self.Core.PoolGetUnit(out self.IdIndexDict);
				self.Core.PoolGetUnit(out self.IdIndexNextDict);
			}
		}


		private class RemoveRule : RemoveRule<RuleActuatorBase>
		{
			protected override void Execute(RuleActuatorBase self)
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