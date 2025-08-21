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

using System;

namespace WorldTree
{
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
		public RuleExecutorPair[] nodes;

		/// <summary>
		/// 出队指针
		/// </summary>
		public int readPoint;

		/// <summary>
		/// 入队指针
		/// </summary>
		public int writePoint;

		/// <summary>
		/// 下一次遍历数量
		/// </summary>
		public int nextTraversalCount;

		/// <summary>
		/// 每次遍历数量
		/// </summary>
		public int traversalCount;

		/// <summary>
		/// 新增数据奇偶标记
		/// </summary>
		public bool isAddOdd;

		/// <summary>
		/// 当前新节点数量
		/// </summary>
		public int nowNewCount;

		/// <summary>
		/// 奇偶切换点
		/// </summary>
		public int switchPoint;

		/// <summary>
		/// 当前起始添加位置
		/// </summary>
		public int addStartIndex;

		/// <summary>
		/// 初始化状态
		/// </summary>
		public bool isInit = true;

		public int TraversalCount => traversalCount;



		/// <summary>
		/// 尝试添加节点
		/// </summary>
		public bool TryAdd(INode node)
		{
			if (node == null) return false;
			if (ruleGroupDict == null || !ruleGroupDict.TryGetValue(node.Type, out RuleList rule)) return false;

			int addIndex = 0;

			// 如果是初始化状态，直接从0开始添加，模拟已经遍历完毕
			if (isInit)
			{
				addIndex = nowNewCount;
				switchPoint = nowNewCount + 1;
				traversalCount = switchPoint;
				readPoint = switchPoint;

				writePoint = switchPoint;
				nextTraversalCount = switchPoint;
				isAddOdd = false;
				addStartIndex = 0;
			}
			else
			{
				//计算添加位置 = 当前添加起始点 + 奇偶校正 + 新节点数量 * 2 
				addIndex = addStartIndex + (isAddOdd ? 1 : 0) + nowNewCount * 2;
			}

			// 判断自动扩容
			if (addIndex == nodes.Length) Capacity();
			nodes[addIndex] = new RuleExecutorPair(node, rule);
			nowNewCount++;
			nextTraversalCount = Math.Max(nextTraversalCount, addIndex + 1);
			return true;
		}

		/// <summary>
		/// 扩容
		/// </summary>
		private void Capacity()
		{
			// 如果目标容量小于当前节点数量，则设置为当前节点数量的两倍,为0则设置为4
			int num = this.nodes.Length != 0 ? this.nodes.Length * 2 : 4;
			if (num < this.nextTraversalCount) this.LogError("下标小于当前大小");
			if (num == this.nodes.Length) return;
			if (num > 0)
			{
				var newNodes = this.Core.PoolGetArray<RuleExecutorPair>(num);
				if (this.nextTraversalCount > 0)
				{
					Array.Copy(this.nodes, 0, newNodes, 0, this.nextTraversalCount);
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
			//标记为非初始化状态
			isInit = false;

			//奇偶切换点是上次遍历的数量 - 上次空洞数量
			switchPoint = traversalCount - (readPoint - writePoint);

			//新增位置是切换点 + 上次新节点数量
			addStartIndex = switchPoint + nowNewCount;

			//如果添加位置是奇数
			if (addStartIndex != 0 && (addStartIndex & 1) == 1)
			{
				addStartIndex++;//校正为偶数
			}

			// 清空新节点数量
			nowNewCount = 0;

			// 清空读指针
			readPoint = 0;

			// 清空写指针
			writePoint = 0;

			traversalCount = nextTraversalCount;

			//切换奇偶添加标记
			isAddOdd = !isAddOdd;
			return traversalCount;
		}


		public bool TryDequeue(out INode node, out RuleList ruleList)
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

		public bool TryPeek(out INode node, out RuleList ruleList)
		{
			// 如果遍历数量小于读指针，说明没有可遍历的节点，需要奇偶切换！！！！？？？
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
			nextTraversalCount = 0;
			traversalCount = 0;
			isAddOdd = false;
			nowNewCount = 0;
			switchPoint = 0;
			addStartIndex = 0;
			isInit = true;
			if (nodes != null)
			{
				this.Core.PoolRecycle(nodes);
				nodes = null;
			}
		}

		public void Remove(long id)
		{
			//全局事件的对象是释放移除，不能手动移除
		}

		public void Remove(INode node)
		{
			//全局事件的对象是释放移除，不能手动移除
		}
	}

	public static class RuleGroupExecutorRule
	{
		class RemoveRule : RemoveRule<RuleGroupExecutor>
		{
			protected override void Execute(RuleGroupExecutor self)
			{
				self.Clear();
			}
		}
	}
}
