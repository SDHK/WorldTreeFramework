/****************************************

* 作者：闪电黑客
* 日期：2024/7/2 16:57

* 描述：

*/
using System.Threading;

namespace WorldTree
{

	/// <summary>
	/// id管理器
	/// </summary>
	public class IdManager : Node, IListenerIgnorer
		, CoreManagerOf<WorldLine>
	{
		/// <summary>
		/// 14位为16384个进程
		/// </summary>
		public const int MASK_BIT14 = 0x3fff;
		/// <summary>
		/// 30位时间为34年
		/// </summary>
		public const int MASK_BIT30 = 0x3fffffff;
		/// <summary>
		/// 20位偏移支持每秒并发1048576个UID（100万）
		/// </summary>
		public const int MASK_BIT20 = 0xfffff;

		/// <summary>
		/// UID获取锁
		/// </summary>
		private readonly object uIDLock = new object();

		/// <summary>
		/// 进程Id
		/// </summary>
		public long ProcessId = 1;

		/// <summary>
		/// 当前递增的id值
		/// </summary>
		private long currentId = 0;

		/// <summary>
		/// 当前数据id值时间值
		/// </summary>
		private long currentUIDTime = 0;

		/// <summary>
		/// 数据id时间偏移
		/// </summary>
		private uint uIDTimeOffset;


		/// <summary>
		/// 获取id后递增
		/// </summary>
		public long GetId()
		{
			//因为Id从0开始，所以当currentId等于1<<50时，下一个Id就是1<<50，刚好是UID的起始值，不会冲突
			//1<<50大概是1千万亿，按理说UID会比实例Id更早用完，但都能用到34年后了，应该够用了。
			if (currentId == 1 << 50) this.LogError($"实例Id溢出: {currentId}");
			return Interlocked.Increment(ref currentId);
		}

		/// <summary>
		/// 获取UID
		/// </summary>
		public long GetUID()
		{
			lock (uIDLock)
			{
				var time = Core.RealTimeManager.GetUtcTimeSeconds();
				if (time > currentUIDTime)
				{
					currentUIDTime = time;
					uIDTimeOffset = 0;
				}
				else
				{
					uIDTimeOffset++;
					if (uIDTimeOffset > MASK_BIT20 - 1)//超过最大值
					{
						uIDTimeOffset = 0;
						currentUIDTime++; // 借用下一秒
						this.LogError($"溢出的UID计数: {time} {currentUIDTime}");
					}
				}

				//14位为16384个进程
				//30位时间为34年,20位偏移支持每秒并发1048576个UID（100万）

				//雪花算法 生成UID ：14位进程ID  30位秒级时间戳 20位时间偏移 三部分组成
				var uid = (ProcessId << 50) | (currentUIDTime << 20) | uIDTimeOffset;

				return uid;
			}
		}

		//31位时间为68年,19位偏移支持每秒并发524288个UID（50万）
		//32位时间为136年,18位偏移支持每秒并发262144个UID（20万）

		public override void OnCreate()
		{
			InstanceId = GetId();
			Id = InstanceId;
		}

		/// <summary>
		/// 释放后
		/// </summary>
		public override void OnDispose()
		{
			NodeBranchHelper.RemoveNode(this);//从父节点分支移除
			IsDisposed = true;
		}
	}
}
