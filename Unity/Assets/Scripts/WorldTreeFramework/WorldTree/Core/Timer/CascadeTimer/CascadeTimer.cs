/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/19 15:59

* 描述： 级联定时器
* 
* Number是long，它可以是DateTime.Ticks，也可以是自定义的时钟时间戳,也可以是帧。
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
		/// 上次序号  
		/// </summary>
		public long LastNumber;

		/// <summary>
		/// 当前序号
		/// </summary>
		public long CurrentNumber;

		/// <summary>
		/// 推进序号(内部) 
		/// </summary>
		public long AdvanceNumber;

		/// <summary>
		/// 追赶精度:用于限制每次追赶的最小单位，0表示不限制
		/// </summary>
		public long Precision;

		/// <summary>
		/// 已占用槽位掩码 
		/// </summary>
		public long OccupiedSlotMask;

		/// <summary>
		/// 最小序号记录
		/// </summary>
		public long minNumber;

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

		/// <summary>
		/// 更新 
		/// </summary>
		public static void Update(this CascadeTimer self, long currentTimeStamp)
		{
			self.CurrentNumber = currentTimeStamp;

			//没有占用槽位则直接返回
			if (self.OccupiedSlotMask == 0) return;
			//回退不处理，等待CurrentNumber追上LastNumber后自然恢复
			if (self.CurrentNumber < self.LastNumber) return;
			//获取序号变化差异
			var numberDiff = self.CurrentNumber ^ self.LastNumber;
			//没有变化就直接返回
			if (numberDiff == 0) return;
			//获取占用槽位的最低位置
			var minSlot = MathBit.GetLowestBitIndex(self.OccupiedSlotMask);
			//如果最低槽位比当前变化大，说明没有要处理的，直接返回。
			if (minSlot == -1 || (1L << minSlot) > numberDiff) return;

			//前进追赶
			do
			{
				// 按精度递增序号追赶
				if (self.Precision != 0 && self.AdvanceNumber < self.minNumber)
				{
					self.AdvanceNumber += self.Precision;
				}
				//按记录的最小序号进行自适应追赶
				else if (self.minNumber != long.MaxValue)
				{
					self.AdvanceNumber = self.minNumber;
				}
				//直接追赶到当前序号
				else if (self.AdvanceNumber < self.CurrentNumber)
				{
					self.AdvanceNumber = self.CurrentNumber;
				}
				//跳出循环
				else
				{
					break;
				}
				//重置最小序号记录
				self.minNumber = long.MaxValue;
				//追赶序号
				self.Advance(self.AdvanceNumber);

				//没有占用槽位则直接返回
				if (self.OccupiedSlotMask == 0) return;
			} while (true);
		}

		/// <summary>
		/// 推进追赶
		/// </summary>
		private static void Advance(this CascadeTimer self, long adviceNumber)
		{
			//获取序号变化差异
			var numberDiff = adviceNumber ^ self.LastNumber;
			//没有变化就直接返回
			if (numberDiff == 0) return;
			//获取占用槽位的最低位置
			var minSlot = MathBit.GetLowestBitIndex(self.OccupiedSlotMask);
			//如果最低槽位比当前变化大，说明没有要处理的，直接返回。
			if (minSlot == -1 || (1L << minSlot) > numberDiff) return;
			//如果变化的序号大于已占用槽位，直接遍历所有槽位，否则只遍历本次序号变化内涉及的槽位。
			var clampDiffSlot = numberDiff > self.OccupiedSlotMask ? self.OccupiedSlotMask : numberDiff;
			var max = MathBit.GetHighestBitIndex(clampDiffSlot);
			var min = minSlot;
			//序号推进正序遍历，避免重复处理下移的定序器
			for (int i = min; i <= max; i++) self.SlotUpdate(i);
			//序号更新
			self.LastNumber = adviceNumber;
			//执行定, 然后清空执行器
			self.RuleMulticast.Send();
			self.RuleMulticast.Clear();
		}

		/// <summary>
		/// 槽位更新
		/// </summary>
		private static void SlotUpdate(this CascadeTimer self, int i)
		{
			//槽位未占用则跳过
			if ((self.OccupiedSlotMask & (1L << i)) == 0) return;
			var slot = self.SlotList[i];

			//遍历槽位内的定序器
			slot.NumberIterator.RefreshTraversalCount();
			for (int j = 0; j < slot.NumberIterator.TraversalCount; j++)
			{
				if (!slot.NumberIterator.TryGetNext(out var nodeRef)) continue;
				var timerData = nodeRef.Value;
				//移除空定序器
				if (timerData == null || timerData.IsNull)
				{
					slot.NumberIterator.DequeueCurrent();
					timerData?.Dispose();
					continue;
				}
				//序号到达，执行定序器
				if (self.AdvanceNumber >= timerData.TimeStamp)
				{
					if (self.Core.RuleManager.TryGetRuleList(timerData.Node.Value.Id, timerData.RuleType, out var ruleList))
					{
						//添加到执行器
						self.RuleMulticast.TryAdd(timerData.Node.Value, ruleList);
					}
					//从槽位移除
					timerData.Dispose();
					slot.NumberIterator.DequeueCurrent();

				}
				//序号未到达，重新计算槽位位置
				else
				{
					//重新计算槽位位置
					int slotIndex = MathBit.GetHighestBitIndex(self.AdvanceNumber ^ timerData.TimeStamp);
					//槽位未变更则跳过
					if (slotIndex == i) continue;
					//移到新的槽位
					self.SlotList[slotIndex].Add(timerData);
					//从当前槽位移除
					slot.NumberIterator.DequeueCurrent();
					//标记槽位已占用
					self.OccupiedSlotMask |= 1L << slotIndex;
				}
			}
			//槽位已空，清除占用标记
			if (slot.NumberIterator.TraversalCount == 0) self.OccupiedSlotMask &= ~(1L << i);
		}


		/// <summary>
		/// 添加定时器 
		/// </summary>
		public static long AddTimer(this CascadeTimer self, long clockTimeStamp, INode node, long ruleType)
		{
			if (!self.Core.RuleManager.TryGetRuleList(node.Id, ruleType, out var ruleList)) return -1;

			//大小比较，过时了就直接执行
			if (self.AdvanceNumber >= clockTimeStamp)
			{
				self.RuleMulticast.TryAdd(node, ruleList);
				return -1;
			}

			//记录添加的最小时间戳,用于自适应追赶
			if (self.minNumber == 0 || clockTimeStamp < self.minNumber || self.AdvanceNumber >= self.minNumber)
			{
				self.minNumber = clockTimeStamp;
			}

			//算出槽位位置，TimeStamp 与 clockTimeStamp 的异或后的最高位1所在的位置
			int slotIndex = MathBit.GetHighestBitIndex(self.AdvanceNumber ^ clockTimeStamp);
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
