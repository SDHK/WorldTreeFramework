/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 一个编号分发的管理器
* 
*/

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

		public IdManager()
		{
			Type = TypeInfo<IdManager>.TypeCode;
		}

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
			return currentId++;
		}

		/// <summary>
		/// 获取UID
		/// </summary>
		public long GetUID()
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
				if (uIDTimeOffset > ushort.MaxValue - 1)//超过最大值
				{
					uIDTimeOffset = 0;
					currentUIDTime++; // 借用下一秒
					this.LogError($"溢出的UID计数: {time} {currentUIDTime}");
				}
			}

			//14位为16384个进程
			//30位时间为34年,20位偏移支持每秒并发1048576个UID（100万）
			//31位时间为68年,19位偏移支持每秒并发524288个UID（50万）
			//32位时间为136年,18位偏移支持每秒并发262144个UID（20万）

			//雪花算法 生成UID ：14位进程ID  30位秒级时间戳 20位时间偏移 三部分组成
			var uid = (ProcessId << 50) | (currentUIDTime << 20) | uIDTimeOffset;

			return uid;
		}

		public override void OnCreate()
		{
			Id = GetId();
			Core.RuleManager?.SupportNodeRule(Type);
		}

		/// <summary>
		/// 释放后
		/// </summary>
		public override void OnDispose()
		{
			NodeBranchHelper.RemoveBranchNode(Parent, BranchType, this);//从父节点分支移除
			IsDisposed = true;
		}
	}
}
