/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 一个编号分发的管理器
* 
* 后续需要改为根据时间生成
*/

using WorldTree.Internal;
using static PlasticPipe.Server.MonitorStats;

namespace WorldTree
{

	/// <summary>
	/// id管理器
	/// </summary>
	public class IdManager : Node, IListenerIgnorer, ComponentOf<WorldTreeCore>
	{
		/// <summary>
		/// 进程Id
		/// </summary>
		public long ProcessId;

		/// <summary>
		/// 线程Id
		/// </summary>
		public long WorldId;


		public IdManager()
		{
			Type = TypeInfo<IdManager>.TypeCode;
		}

		/// <summary>
		/// 当前递增的id值
		/// </summary>
		public long currentId = 0;

		/// <summary>
		/// 当前数据id值时间值
		/// </summary>
		public long currentUIDTime = 0;

		/// <summary>
		/// 数据id时间偏移
		/// </summary>
		public uint UIDTimeOffset;


		/// <summary>
		/// 释放后
		/// </summary>
		public override void OnDispose()
		{
			NodeBranchHelper.RemoveBranchNode(this.Parent, this.BranchType, this);//从父节点分支移除
			this.IsRecycle = true;
			this.IsDisposed = true;
		}
	}

	public static class IdManagerRule
	{

		/// <summary>
		/// 获取id后递增
		/// </summary>
		public static long GetId(this IdManager self)
		{
			return self.currentId++;
		}

		/// <summary>
		/// 获取UID
		/// </summary>
		public static long GetUID(this IdManager self)
		{
			var time = self.Core.RealTimeManager.NowUtcTime();
			if (time > self.currentUIDTime)
			{
				self.currentUIDTime = time;
				self.UIDTimeOffset = 0;
			}
			else
			{
				self.UIDTimeOffset++;
				if (self.UIDTimeOffset > ushort.MaxValue - 1)//超过最大值
				{
					self.UIDTimeOffset = 0;
					self.currentUIDTime++; // 借用下一秒
					self.LogError($"溢出的UID计数: {time} {self.currentUIDTime}");
				}
			}

			//14位为16384个进程
			//30位时间为34年,20位偏移支持每秒并发1048576个UID（100万）
			//31位时间为68年,19位偏移支持每秒并发524288个UID（50万）
			//32位时间为136年,18位偏移支持每秒并发262144个UID（20万）

			//雪花算法 生成UID ：14位进程ID  30位秒级时间戳 20位时间偏移 三部分组成
			var uid = (self.ProcessId << 50)  | (self.currentUIDTime << 20) | self.UIDTimeOffset;

			return uid;
		}
	}
}
