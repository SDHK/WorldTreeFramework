/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/1 19:44

* 描述： 法则集合执行器基类
*
* 执行拥有指定法则的节点
*
* 核心思想：
* 对稳定数据使用+1间隔的连续访问获得极致性能，
* 对动态数据使用+2间隔的奇偶交替访问保证稳定性，
* 同时在遍历过程中进行渐进式空洞压缩，实现零开销的内存整理。
*
* 特点：
* 1. 双模式遍历：前半段+1连续，后半段+2奇偶
* 2. 奇偶时空切换点：switchPoint = traversalCount - lastVacuityCount
* 3. 空洞填充点：addStartIndex = switchPoint + nowNewCount
* 4. 奇偶轮换：当前轮避开新增数据，遍历上轮的奇偶数据
* 5. 渐进式压缩：边遍历边整理，遍历和添加同时在压缩空洞，无突发性能开销。
* 6. 最好情况：数据没有增删时，只有连续访问单个操作，没有写入操作
* 7. 遍历时添加新增数据完全隔离在后面的奇偶区域，不会打乱原有遍历顺序。
* 
* Queue循环数组的缺点：
* - 每次都有读和写两个操作
* - 指针跳跃访问模式
* - 遍历时添加节点会立即插入，打乱原有顺序
*  
* 双List轮换方式的缺点：
* - 每次都有读和写两个操作
* - 始终使用固定间隔访问，无法享受连续访问的极致性能
* - 需要维护两套完整的数组，内存占用翻倍
* - 遍历时添加节点会立即插入，打乱原有顺序
* 
* 时间，空间全都要！
* 我找到了免费的午餐！
* 

 */

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 法则集合执行器抽象基类
	/// </summary>
	public abstract class RuleGroupExecutorOld : Node, IRuleExecutorOperate, IRuleExecutorEnumerable
		, AsChildBranch
	{
		/// <summary>
		/// 法则类型
		/// </summary>
		public long RuleType => ruleGroupDict.RuleType;

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
			nodeNextList.Clear();
			delegateNextList.Clear();
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
			nodeIndex = 0;
		}

		/// <summary>
		/// 尝试添加节点到执行器
		/// </summary>
		public bool TryAdd(INode node)
		{
			//节点不允许重复添加。
			if (IdIndexDict != null && IdIndexDict.ContainsKey(node.InstanceId)) return false;
			if (ruleGroupDict == null || !ruleGroupDict.TryGetValue(node.Type, out RuleList ruleList)) return false;
			nodeNextList.Add(new NodeRef<INode>(node, false));
			delegateNextList.Add(ruleList);
			IdIndexDict.Add(node.InstanceId, nodeNextList.Count - 1);
			return true;
		}

		public void Remove(INode node) => Remove(node.InstanceId);


		public void Remove(long id)
		{
			if (IdIndexDict.TryGetValue(id, out int index))
			{
				IdIndexDict.Remove(id);
				// 优先检查 nodeNextList
				if (nodeNextList.Count > index)
				{
					if (nodeNextList[index].InstanceId == id)
					{
						//清除引用，让其变成意外回收
						nodeNextList[index].Clear();
						return;
					}
				}

				// 再检查 nodeList
				if (nodeList.Count > index)
				{
					if (nodeList[index].InstanceId == id)
					{
						//清除引用，让其变成意外回收
						nodeList[index].Clear();
					}
				}
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
					// 值已经拿到，列表内部作废，清除引用
					nodeList[nodeIndex].Clear();
					ruleList = delegateList[nodeIndex];
					nodeIndex++;
					// 更新索引字典
					IdIndexDict[nodeRef.InstanceId] = nodeNextList.Count - 1;
					nodeNextList.Add(nodeRef); // 塞回队列用于下次遍历
					delegateNextList.Add(ruleList);
					return true;
				}
			}
			node = null;
			ruleList = null;
			return false;
		}

	}
	public static class RuleGroupExecutorOldRule
	{
		private class Add : AddRule<RuleGroupExecutorOld>
		{
			protected override void Execute(RuleGroupExecutorOld self)
			{
				self.Core.PoolGetUnit(out self.nodeList);
				self.Core.PoolGetUnit(out self.nodeNextList);
				self.Core.PoolGetUnit(out self.delegateList);
				self.Core.PoolGetUnit(out self.delegateNextList);
				self.Core.PoolGetUnit(out self.IdIndexDict);
			}
		}

		class RemoveRule : RemoveRule<RuleGroupExecutorOld>
		{
			protected override void Execute(RuleGroupExecutorOld self)
			{
				self.nodeIndex = 0;
				self.nodeList.Dispose();
				self.nodeNextList.Dispose();
				self.delegateList.Dispose();
				self.delegateNextList.Dispose();
				self.IdIndexDict.Dispose();
				self.nodeList = null;
				self.nodeNextList = null;
				self.delegateList = null;
				self.delegateNextList = null;
				self.IdIndexDict = null;
			}
		}
	}
}
