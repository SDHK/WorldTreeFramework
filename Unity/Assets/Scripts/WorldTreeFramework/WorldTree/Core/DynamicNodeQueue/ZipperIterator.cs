/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 16:21

* 描述： 拉链迭代器
* 
* 支持动态添加和删除数据的迭代器
*
* 核心思想：
* 对稳定数据使用+1间隔的连续访问获得极致性能，
* 对新增数据使用+2间隔的奇偶交替访问，保证性能同时腾出空间，
* 在遍历过程中进行渐进式空洞压缩，实现零开销的内存整理。
* 保证新增的数据不会打乱原有数据的访问顺序，时序绝对稳定。
*
* 特点：
* 1. 双模式遍历：前半段+1连续，后半段+2奇偶。
* 2. 奇偶时空切换点：switchPoint = traversalCount - lastVacuityCount
* 3. 空洞填充点：addStartIndex = switchPoint + nowNewCount
* 4. 双模式压缩：边遍历边整理，遍历和添加同时在压缩空洞。
* 5. 最好情况：数据没有增删时，只有连续访问单个操作，没有写入操作
* 6. 遍历时删除和新增数据，顺序稳定不变，这是其它方案绝对做不到的，难得的时序稳定能力。
* 
* Queue循环数组的缺点：
* - 每次都有读和写两个操作
* - 指针跳跃访问模式
* - 遍历时添加数据会立即插入，打乱原有顺序时序混乱
*  
* 双List轮换方式的缺点：
* - 每次都有读和写两个操作
* - 指针跳跃访问模式
* - 需要维护两套完整的数组，内存占用翻倍
* - 遍历时添加数据会立即插入，打乱原有顺序时序混乱
* 
* 时间，空间，时序，全都要！
* 
*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 节点迭代器:支持动态添加和删除数据的迭代器。 
	/// </summary>
	public class IteratorNodeRef<T> : ZipperIterator<NodeRef<T>>
		where T : class, INode
	{
		public override bool CheckNull(NodeRef<T> data) => data == default || data.IsNull;
	}


	/// <summary>
	/// 拉链迭代器:支持动态添加和删除数据的迭代器。
	/// </summary>
	public abstract class ZipperIterator<T> : Node
		, ComponentOf<INode>, ChildOf<INode>
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 数据列表
		/// </summary>
		public T[] nodes;

		/// <summary>
		/// 出队指针
		/// </summary>
		[TreeDataIgnore]
		public int readPoint;

		/// <summary>
		/// 入队指针
		/// </summary>
		[TreeDataIgnore]
		public int writePoint;

		/// <summary>
		/// 下一次遍历数量
		/// </summary>
		[TreeDataIgnore]
		public int nextTraversalCount;

		/// <summary>
		/// 每次遍历数量
		/// </summary>
		[TreeDataIgnore]
		public int traversalCount;

		/// <summary>
		/// 新增数据奇偶标记
		/// </summary>
		[TreeDataIgnore]
		public bool isAddOdd;

		/// <summary>
		/// 当前新数据数量
		/// </summary>
		[TreeDataIgnore]
		public int nowNewCount;

		/// <summary>
		/// 奇偶切换点：在这点后的空间是二维的，新增和旧数据将会交替叠加在一起。
		/// </summary>
		[TreeDataIgnore]
		public int switchPoint;

		/// <summary>
		/// 当前起始添加位置：这是切换点之后，理论上绝对安全的添加位置。
		/// </summary>
		[TreeDataIgnore]
		public int addStartIndex;

		/// <summary>
		/// 初始化状态
		/// </summary>
		[TreeDataIgnore]
		public bool isInit = false;

		/// <summary>
		/// 动态遍历数量 
		/// </summary>
		public int TraversalCount => traversalCount;

		/// <summary>
		/// 遍历后的剩余数量 
		/// </summary>
		public int RemainCount => writePoint + nowNewCount;

		/// <summary>
		/// 尝试添加数据
		/// </summary>
		public virtual bool TryAdd(T data)
		{
			if (CheckNull(data)) return false;

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
				//计算添加位置 = 当前添加起始点 + 新数据数量 * 2 
				addIndex = addStartIndex + (nowNewCount << 1);
				nowNewCount++;
			}

			// 判断自动扩容
			if (addIndex + 1 >= nodes.Length) Capacity();
			nodes[addIndex] = data;
			nextTraversalCount = addIndex + 1;
			return true;
		}

		/// <summary>
		/// 扩容
		/// </summary>
		protected void Capacity()
		{
			// 如果目标容量小于当前数据数量，则设置为当前数据数量的两倍,为0则设置为4
			int num = this.nodes.Length != 0 ? this.nodes.Length * 2 : 4;
			if (num == this.nodes.Length) return;
			if (num < this.nextTraversalCount) this.LogError("下标小于当前大小");
			if (num > 0)
			{
				var newNodes = this.Core.PoolGetArray<T>(num);
				if (this.nodes.Length != 0)
				{
					Array.Copy(this.nodes, 0, newNodes, 0, this.nodes.Length);
				}
				this.Core.PoolRecycle(this.nodes, true);
				this.nodes = newNodes;
			}
			else
			{
				if (this.nodes != null)
				{
					this.Core.PoolRecycle(this.nodes, true);
				}
				this.nodes = this.Core.PoolGetArray<T>(4);
			}
		}

		/// <summary>
		/// 刷新遍历数量 
		/// </summary>
		public int RefreshTraversalCount()
		{
			//判断如果下一次遍历数量为0，说明没有可遍历的数据，当前数组为空，直接恢复为初始化状态
			isInit = nextTraversalCount == 0;
			//奇偶切换点是读取指针最后停下的位置
			switchPoint = writePoint;
			//新增位置是切换点 + 上次新数据数量
			addStartIndex = switchPoint + nowNewCount;
			//切换奇偶添加标记
			isAddOdd = !isAddOdd;
			//如果是奇数位置，矫正为奇数
			if (isAddOdd)
			{
				addStartIndex |= 1;
			}
			// 如果是偶数位置，矫正为偶数
			else if ((addStartIndex & 1) == 1)
			{
				addStartIndex++;
			}
			// 清空新数据数量
			nowNewCount = 0;
			// 清空读指针
			readPoint = 0;
			// 清空写指针
			writePoint = 0;
			traversalCount = nextTraversalCount;
			return traversalCount;
		}

		/// <summary>
		/// 获取下一个
		/// </summary>
		public virtual bool TryGetNext(out T pair)
		{
			//遍历间隔
			int indexInterval = 1;

			// 如果遍历数量小于读指针，说明没有可遍历的数据
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

				pair = nodes[readPoint];

				if (CheckNull(pair)) // 数据为空判断
				{
					readPoint += indexInterval;
					continue; // 继续下一个数据
				}

				// 空洞压缩：如果读写指针不相等，说明有数据被移除，这时需要将当前数据移到写入点。
				if (readPoint != writePoint)
				{
					// 将当前数据放到写入点
					nodes[writePoint] = pair;
					// 清除当前读取点
					nodes[readPoint] = default;
				}

				writePoint++;
				readPoint += indexInterval;
				return true;
			}

			if (nowNewCount == 0)
			{
				// 直接裁剪掉所有的空洞
				nextTraversalCount = writePoint;
				// 判断如果遍历数量为0，说明没有可遍历的数据，当前数组为空，直接恢复为初始化状态
				if (nextTraversalCount == 0) isInit = true;
			}
			pair = default;
			return false;
		}


		/// <summary>
		/// 检查数据是否为空 
		/// </summary>
		public abstract bool CheckNull(T data);

		/// <summary>
		/// 出队当前数据 
		/// </summary>
		public void DequeueCurrent()
		{
			if (writePoint == 0) return;
			writePoint--;
			nodes[writePoint] = default;
		}


		/// <summary>
		/// 清空队列 
		/// </summary>
		public virtual void Clear()
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
		}
	}

	public static class FreeIteratorRule
	{
		class Remove<T> : RemoveRule<ZipperIterator<T>>
		{
			protected override void Execute(ZipperIterator<T> self)
			{
				self.Clear();
				self.Core.PoolRecycle(self.nodes, true);
				self.nodes = null;
			}
		}
	}
}
