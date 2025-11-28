/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/19 15:59

* 描述： 级联定时器
* 
* 时间戳是long，它可以是DateTime.Ticks，也可以是自定义的时钟时间戳,也可以是帧。
* 本身会自动根据Update频率适应精度，而PrecisionMask可以用来固定精度。
* 通过二分降级均摊传统降级排序开销，避免雪崩消耗。

*/
namespace WorldTree
{
	/// <summary>
	/// 级联定时器
	/// </summary>
	public class CascadeTimer : Node
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 上次时间戳  
		/// </summary>
		public long LastTimeStamp;

		/// <summary>
		/// 当前时间戳 
		/// </summary>
		public long TimeStamp;

		/// <summary>
		/// 精度掩码：用来强制对齐的，一般不需要设置，0为最高精度。
		/// </summary>
		public long PrecisionMask;

		/// <summary>
		/// 已占用槽位掩码 
		/// </summary>
		public long OccupiedSlotMask;

		/// <summary>
		/// 槽位数组 
		/// </summary>
		public TreeList<CascadeTimerSlot> SlotList;

		/// <summary>
		/// 执行器 
		/// </summary>
		public RuleMulticast<ISendRule> RuleMulticast;
	}


	public static class CascadeTimerRule
	{
		class AddRule : AddRule<CascadeTimer>
		{
			protected override void Execute(CascadeTimer self)
			{
				self.AddTemp(out self.SlotList);
				for (int i = 0; i < 64; i++)
				{
					self.AddChild(out CascadeTimerSlot slot);
					self.SlotList.Add(slot);
				}
			}
		}

		class UpdateRule : UpdateRule<CascadeTimer>
		{
			protected override void Execute(CascadeTimer self)
			{
				//没有占用槽位则直接返回
				if (self.OccupiedSlotMask == 0) return;
				//去掉精度位,只遍历高于精度的槽位
				if (self.PrecisionMask != 0) self.TimeStamp &= (~(self.PrecisionMask));
				//时间回退不处理，等待时间追上LastTimeStamp后自然恢复
				if (self.TimeStamp < self.LastTimeStamp) return;
				//获取时间戳变化差异
				var timeDiff = self.TimeStamp ^ self.LastTimeStamp;
				//没有变化就直接返回
				if (timeDiff == 0) return;
				//获取占用槽位的最低位置
				var minSlot = MathBit.GetLowestBitIndex(self.OccupiedSlotMask);
				//如果最低槽位比时间变化大，说明没有到时的要处理，直接返回。
				if (minSlot == -1 || (1L << minSlot) > timeDiff) return;
				//如果变化的时间大于已占用槽位，直接遍历所有槽位，否则只遍历本次时间变化内涉及的槽位。
				var clampDiffSlot = timeDiff > self.OccupiedSlotMask ? self.OccupiedSlotMask : timeDiff;
				var max = MathBit.GetHighestBitIndex(clampDiffSlot);
				var min = minSlot;
				//时间前进时正序遍历，避免重复处理下移的定时器
				for (int i = min; i <= max; i++) self.SlotUpdate(i);
				//时间戳更新
				self.LastTimeStamp = self.TimeStamp;
				//执行定时器, 然后清空执行器
				self.RuleMulticast.Send();
				self.RuleMulticast.Clear();
			}
		}

		/// <summary>
		/// 槽位更新
		/// </summary>
		private static void SlotUpdate(this CascadeTimer self, int i)
		{
			//槽位未占用则跳过
			if ((self.OccupiedSlotMask & (1L << i)) == 0) return;
			var slot = self.SlotList[i];

			//遍历槽位内的定时器
			slot.TimerIterator.RefreshTraversalCount();
			for (int j = 0; j < slot.TimerIterator.TraversalCount; j++)
			{
				if (!slot.TimerIterator.TryGetNext(out var nodeRef)) continue;
				var timerData = nodeRef.Value;
				//移除空定时器
				if (timerData == null || timerData.IsNull)
				{
					slot.TimerIterator.DequeueCurrent();
					timerData?.Dispose();
					continue;
				}
				//时间到达，执行定时器
				if (self.TimeStamp >= timerData.TimeStamp)
				{
					if (self.Core.RuleManager.TryGetRuleList(timerData.Node.Value.Id, timerData.RuleType, out var ruleList))
					{
						//添加到执行器
						self.RuleMulticast.TryAdd(timerData.Node.Value, ruleList);
					}
					//从槽位移除
					timerData.Dispose();
					slot.TimerIterator.DequeueCurrent();

				}
				//时间未到达，重新计算槽位位置
				else
				{
					//重新计算槽位位置
					int slotIndex = MathBit.GetHighestBitIndex(self.TimeStamp ^ timerData.TimeStamp);
					//槽位未变更则跳过
					if (slotIndex == i) continue;
					//移到新的槽位
					self.SlotList[slotIndex].Add(timerData);
					//从当前槽位移除
					slot.TimerIterator.DequeueCurrent();
					//标记槽位已占用
					self.OccupiedSlotMask |= 1L << slotIndex;
				}
			}
			//槽位已空，清除占用标记
			if (slot.TimerIterator.TraversalCount == 0) self.OccupiedSlotMask &= ~(1L << i);
		}


		/// <summary>
		/// 添加定时器 
		/// </summary>
		public static long AddTimer(this CascadeTimer self, long clockTimeStamp, INode node, long ruleType)
		{
			if (!self.Core.RuleManager.TryGetRuleList(node.Id, ruleType, out var ruleList)) return -1;

			//大小比较，过时了就直接执行
			if (self.TimeStamp >= clockTimeStamp)
			{
				self.RuleMulticast.TryAdd(node, ruleList);
				return -1;
			}

			//算出槽位位置，TimeStamp 与 clockTimeStamp 的异或后的最高位1所在的位置
			int slotIndex = MathBit.GetHighestBitIndex(self.TimeStamp ^ clockTimeStamp);
			self.AddChild(out CascadeTimerData timerData, clockTimeStamp, node, ruleType);
			self.SlotList[slotIndex].Add(timerData);
			//标记槽位已占用
			self.OccupiedSlotMask |= 1L << slotIndex;
			return timerData.Id;
		}

		/// <summary>
		/// 移除定时器 
		/// </summary>
		public static void RemoveTimer(this CascadeTimer self, long timerId)
		{
			self.RemoveChild(timerId);
		}
	}
}
