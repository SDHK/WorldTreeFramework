/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/11 11:52:09

* 描述： 法则列表执行器基类
* 
* 增加了Id索引字典，用于快速定位节点位置，提升删除效率
*

*/

namespace WorldTree
{
	/// <summary>
	/// 法则列表执行器抽象基类
	/// </summary>
	public abstract class RuleListExecutor : RuleExecutor, IRuleExecutorEnumerable
		, AsChildBranch
	{
		/// <summary>
		/// id索引字典
		/// </summary>
		public UnitDictionary<long, int> IdIndexDict;

		/// <summary>
		/// 尝试添加节点和法则
		/// </summary>
		public override bool TryAdd(INode node, RuleList rule)
		{
			if (node == null || rule == null) return false;
			if (IdIndexDict != null && IdIndexDict.ContainsKey(node.InstanceId)) return false;

			int addIndex = 0;

			// 如果是初始化状态，直接从0开始添加，模拟已经遍历完毕
			if (isInit)
			{
				addIndex = nowNewCount;
				nowNewCount++;
				switchPoint = nowNewCount;
				traversalCount = nowNewCount;
				readPoint = nowNewCount;
				writePoint = nowNewCount;
				nextTraversalCount = nowNewCount;
				isAddOdd = false;
				addStartIndex = 0;
			}
			else
			{
				//计算添加位置 = 当前添加起始点 + 新节点数量 * 2 
				addIndex = addStartIndex + (nowNewCount << 1);
				nowNewCount++;
			}

			// 判断扩容
			if (addIndex == nodes.Length) Capacity();
			nodes[addIndex] = new NodeRuleRef(node, rule);
			IdIndexDict.Add(node.InstanceId, addIndex);
			nextTraversalCount = addIndex + 1;
			return true;
		}

		public override bool TryGetNext(out INode node, out RuleList ruleList)
		{
			//遍历间隔
			int indexInterval = 1;

			// 如果遍历数量小于读指针，说明没有可遍历的节点
			while (traversalCount > readPoint)
			{
				// 切换到奇偶遍历状态，读取上一轮添加的新数据
				if (indexInterval == 1 && readPoint == switchPoint)
				{
					//切换点是偶数
					if (readPoint == 0 || (readPoint & 1) == 0)
					{
						//判断如果当前添加标记是偶数，那么上次添加位置就是奇数，读取指针偏移到奇数位置
						if (!isAddOdd) readPoint++;
					}
					//切换点是奇数
					else
					{
						//判断当前添加标记是奇数,那么上次添加位置就是偶数，读取指针偏移到偶数位置
						if (isAddOdd) readPoint++;
					}
					// 接下来读取间隔为2
					indexInterval = 2;
				}

				// 使用引用类型来避免结构体复制
				ref NodeRuleRef pair = ref nodes[readPoint];

				node = pair.Node;
				if (node == null) // 节点意外回收
				{
					readPoint += indexInterval;
					IdIndexDict.Remove(pair.InstanceId);
					continue; // 继续下一个节点
				}
				ruleList = pair.Rule;

				// 空洞压缩：如果读写指针不相等，说明有节点被移除，这时需要将当前节点移到写入点。
				if (readPoint != writePoint)
				{
					// 将当前节点放到写入点
					nodes[writePoint] = pair;
					// 更新索引字典
					IdIndexDict[node.InstanceId] = writePoint;
					// 为了安全，清除引用，因为节点已经移动到写入点
					pair.Clear();
				}
				writePoint++;
				readPoint += indexInterval;
				return true;
			}

			if (nowNewCount == 0)
			{
				// 直接裁剪掉所有的空洞
				nextTraversalCount = writePoint;
				// 判断如果遍历数量为0，说明没有可遍历的节点，当前数组为空，直接恢复为初始化状态
				if (nextTraversalCount == 0) isInit = true;
			}
			node = null;
			ruleList = null;
			return false;
		}

		public override void Clear()
		{
			readPoint = 0;
			writePoint = 0;
			nextTraversalCount = 0;
			traversalCount = 0;
			isAddOdd = false;
			nowNewCount = 0;
			switchPoint = 0;
			addStartIndex = 0;
			isInit = true;
			IdIndexDict.Clear();
		}

		/// <summary>
		/// 移除
		/// </summary>
		public virtual void Remove(long id)
		{
			if (IdIndexDict.TryGetValue(id, out int index))
			{
				IdIndexDict.Remove(id);
				if (index < nodes.Length && nodes[index].InstanceId == id)
				{
					//清除引用，让其变成意外回收
					nodes[index].Clear();
				}
			}
		}

		/// <summary>
		/// 移除
		/// </summary>
		public virtual void Remove(INode node)
		{
			if (node == null) return;
			Remove(node.InstanceId);
		}
	}


	public static class RuleListExecutorRule
	{
		private class AddRule : AddRule<RuleListExecutor>
		{
			protected override void Execute(RuleListExecutor self)
			{
				self.Core.PoolGetUnit(out self.IdIndexDict);
				self.GetBaseRule<RuleListExecutor, RuleExecutor, Add>().Send(self);
			}
		}

		private class RemoveRule : RemoveRule<RuleListExecutor>
		{
			protected override void Execute(RuleListExecutor self)
			{
				self.GetBaseRule<RuleListExecutor, RuleExecutor, Remove>().Send(self);
				self.IdIndexDict.Dispose();
				self.IdIndexDict = null;
			}
		}
	}
}