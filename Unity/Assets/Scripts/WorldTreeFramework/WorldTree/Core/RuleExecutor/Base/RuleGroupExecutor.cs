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
* 技术特点：
* 1. 双模式遍历：前半段+1连续，后半段+2奇偶
* 2. 奇偶切换点：switchPoint = traversalCount - lastVacuityCount
* 3. 奇偶轮换：当前轮避开新增数据，遍历上轮的奇偶数据
* 4. 渐进式压缩：边遍历边整理，无突发性能开销
* 5. 最好情况：数据没有增删时，只有连续访问单个操作，没有写入操作
* 6. 遍历时添加新增数据完全隔离在后面的奇偶区域，不会打乱原有遍历顺序。
* 
* 
* Queue循环数组的缺点：
* - 每次都有读和写两个操作
* - 指针跳跃访问模式
* - 无法利用数组连续访问的硬件优化
* - 遍历时添加节点会立即插入，打乱原有顺序
*  
* 双List轮换方式的缺点：
* - 每次都有读和写两个操作
* - 始终使用固定间隔访问，无法享受连续访问的极致性能
* - 需要维护两套完整的数组，内存占用翻倍
* - 遍历时添加节点会立即插入，打乱原有顺序

 */

using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 法则集合执行器抽象基类
	/// </summary>
	public abstract class RuleGroupExecutor1 : Node, IRuleExecutorOperate, IRuleExecutorEnumerable
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
		public RuleExecutorPair[] nodes;

		/// <summary>
		/// 出队指针
		/// </summary>
		private int readPoint;

		/// <summary>
		/// 入队指针
		/// </summary>
		private int writePoint;

		/// <summary>
		/// 节点总数
		/// </summary>
		private int size;

		/// <summary>
		/// 每次遍历数量
		/// </summary>
		private int traversalCount;


		public int TraversalCount => traversalCount;


		/// <summary>
		/// 奇偶新增标记
		/// </summary>
		private bool isOdd;

		/// <summary>
		/// 奇偶切换点
		/// </summary>
		private int switchPoint;

		/// <summary>
		/// 上次新节点数量
		/// </summary>
		private int lastNewCount;

		/// <summary>
		/// 当前新节点数量
		/// </summary>
		private int nowNewCount;

		/// <summary>
		/// 当前起始添加位置
		/// </summary>
		private int addStartIndex;


		/// <summary>
		/// 上次空节点数量
		/// </summary>
		private int lastVacuityCount;

		/// <summary>
		/// 尝试添加节点和法则
		/// </summary>
		public bool TryAdd(INode node, RuleList rule)
		{
			if (node == null || rule == null) return false;
			//计算添加位置= 当前添加起始点 + 奇偶校正 + 新节点数量 * 2 
			int addIndex = addStartIndex + (isOdd ? 1 : 0) + nowNewCount * 2;
			// 判断扩容
			if (addIndex == nodes.Length) Capacity();
			nodes[addIndex] = new RuleExecutorPair(node, rule);
			nowNewCount++;
			size = Math.Max(size, addIndex + 1);
			return true;
		}

		/// <summary>
		/// 扩容
		/// </summary>
		private void Capacity()
		{
			// 如果目标容量小于当前节点数量，则设置为当前节点数量的两倍,为0则设置为4
			int num = this.nodes.Length != 0 ? this.nodes.Length * 2 : 4;
			if (num < this.size) this.LogError("下标小于当前大小");
			if (num == this.nodes.Length) return;
			if (num > 0)
			{
				var newNodes = this.Core.PoolGetArray<RuleExecutorPair>(num);
				if (this.size > 0)
				{
					Array.Copy(this.nodes, 0, newNodes, 0, this.size);
				}
				this.Core.PoolRecycle(this.nodes);
				this.nodes = newNodes;
			}
			else
			{
				if (this.nodes != null)
				{
					this.Core.PoolRecycle(this.nodes);
				}
				this.nodes = this.Core.PoolGetArray<RuleExecutorPair>(1);
			}
		}


		public int RefreshTraversalCount()
		{
			//空洞数量是上次读取位置 - 写入位置
			lastVacuityCount = readPoint - writePoint;
			//奇偶切换点是上次遍历的数量- 上次空洞数量
			switchPoint = traversalCount - lastVacuityCount;
			//上次新节点数量
			lastNewCount = nowNewCount;

			addStartIndex = switchPoint + lastNewCount;
			//如果添加位置是偶数
			if (addStartIndex != 0 && addStartIndex % 2 == 1)
			{
				addStartIndex++;//校正为偶数
			}

			// 清空当前新节点数量
			nowNewCount = 0;

			// 清空读指针
			readPoint = 0;

			// 清空写指针
			writePoint = 0;

			traversalCount = size;

			//切换奇偶添加标记
			isOdd = !isOdd;
			return traversalCount;
		}


		public bool TryDequeue(out INode node, out RuleList ruleList)
		{
			//遍历间隔
			int indexInterval = 1;

			// 如果遍历数量小于读指针，说明没有可遍历的节点
			while (traversalCount > readPoint)
			{
				// 切换到奇偶遍历状态
				if (indexInterval == 1 && readPoint == switchPoint)
				{
					//切换点是偶数
					if (readPoint == 0 || readPoint % 2 == 0)
					{
						//当前标记是偶数,那么上次就是奇数，读取指针偏移到正确位置
						if (!isOdd) readPoint++;
					}
					//切换点是奇数
					else
					{
						//当前标记是奇数,那么上次就是偶数，读取指针偏移到正确位置
						if (isOdd) readPoint++;
					}
					// 接下来读取间隔为2
					indexInterval = 2;
				}

				// 使用引用类型来避免结构体复制
				ref RuleExecutorPair pair = ref nodes[readPoint];

				node = pair.Node;

				if (node == null) // 节点意外回收
				{
					readPoint += indexInterval;
					continue; // 继续下一个节点
				}

				ruleList = pair.Rule;

				// 空洞压缩：如果读写指针不相等，说明有节点被移除，这时需要将当前节点移到写入点。
				if (readPoint != writePoint)
				{
					// 将当前节点放到写入点
					nodes[writePoint] = pair;

					// 为了安全，清除引用，因为节点已经移动到写入点
					pair.Clear();
				}
				writePoint++;

				readPoint += indexInterval;
				return true;
			}

			node = null;
			ruleList = null;
			return false;
		}

		public bool TryPeek(out INode node, out RuleList ruleList)
		{
			// 如果遍历数量小于读指针，说明没有可遍历的节点
			while (traversalCount > readPoint)
			{
				// 使用引用类型来避免结构体复制
				ref RuleExecutorPair pair = ref nodes[readPoint];
				node = pair.Node;
				ruleList = pair.Rule;
				if (node != null) return true; // 找到有效节点
				readPoint++;
			}
			node = null;
			ruleList = null;
			return false; // 没有有效节点
		}

		public void Clear()
		{
			readPoint = 0;
			writePoint = 0;
			size = 0;
			traversalCount = 0;
		}


		public void Remove(long id)
		{
			//生命周期走意外丢失移除，不能手动移除
		}

		public void Remove(INode node)
		{
			//生命周期走意外丢失移除，不能手动移除
		}

	}

	/// <summary>
	/// 法则集合执行器抽象基类
	/// </summary>
	public abstract class RuleGroupExecutor : Node, IRuleExecutorOperate, IRuleExecutorEnumerable
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

	public static class RuleGroupExecutorBaseRule
	{
		private class Add : AddRule<RuleGroupExecutor>
		{
			protected override void Execute(RuleGroupExecutor self)
			{
				self.Core.PoolGetUnit(out self.nodeList);
				self.Core.PoolGetUnit(out self.nodeNextList);
				self.Core.PoolGetUnit(out self.delegateList);
				self.Core.PoolGetUnit(out self.delegateNextList);
				self.Core.PoolGetUnit(out self.IdIndexDict);
			}
		}

		class RemoveRule : RemoveRule<RuleGroupExecutor>
		{
			protected override void Execute(RuleGroupExecutor self)
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


		class TreeDataSerialize : TreeDataSerializeRule<RuleGroupExecutor>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out RuleGroupExecutor obj, false)) return;
				self.WriteDynamic(1);
				self.WriteValue(obj.RuleType);
			}
		}
		class TreeDataDeserialize : TreeDataDeserializeRule<RuleGroupExecutor>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(RuleGroupExecutor), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				long ruleTypeCode = self.ReadValue<long>();
				value = self.Core.GetRuleBroadcast(ruleTypeCode);
			}
		}
	}
}
