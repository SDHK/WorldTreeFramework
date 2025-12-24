/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/19 15:59

* 描述： 级联定序器
* 
* Tick是long。
* 它可以是DateTime.Ticks,也可以是帧,也可以是自定义的时钟时间戳。
* 本身会自动根据Update频率适应精度，Precision 可以用来限制每次追赶的最小单位。
* 通过二分降级均摊传统降级排序开销，避免雪崩遍历。
* 

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 级联定序器
	/// </summary>
	public class CascadeTicker : Node
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 上次时序  
		/// </summary>
		public long LastTick;

		/// <summary>
		/// 当前时序
		/// </summary>
		public long CurrentTick;

		/// <summary>
		/// 推进时序(内部) 
		/// </summary>
		public long advanceTick;

		/// <summary>
		/// 指针刻度掩码:用于自适应追赶时序的刻度。  
		/// </summary>
		public long precisionMask;

		/// <summary>
		/// 指针刻度掩码:用于自适应追赶时序的刻度。  
		/// </summary>
		public long PrecisionMask => precisionMask;

		/// <summary>
		/// 最小时序记录
		/// </summary>
		public long minTick = long.MaxValue;

		/// <summary>
		/// 已占用槽位掩码 
		/// </summary>
		public long OccupiedSlotMask;

		/// <summary>
		/// 槽位数组 
		/// </summary>
		public CascadeTickerSlot[] Slots;

		/// <summary>
		/// 执行器 
		/// </summary>
		public RuleMulticast<ISendRule> RuleMulticast;
	}


	public static class CascadeTickerRule
	{
		class AddRule : AddRule<CascadeTicker>
		{
			protected override void Execute(CascadeTicker self)
			{
				self.Core.PoolGetArray(out self.Slots, 64);
			}
		}
		class RemoveRule : RemoveRule<CascadeTicker>
		{
			protected override void Execute(CascadeTicker self)
			{
				self.Core.PoolRecycle(self.Slots, true);
				self.LastTick = 0;
				self.CurrentTick = 0;
				self.advanceTick = 0;
				self.OccupiedSlotMask = 0;
				self.minTick = long.MaxValue;
				self.OccupiedSlotMask = 0;
				self.Slots = null;
				self.RuleMulticast = null;
			}
		}

		/// <summary>
		/// 设置指针每帧最大跨越刻度：0表示不限制。1~64（也就是2的幂）
		/// </summary>
		public static void SetPrecision(this CascadeTicker self, int precision)
		{
			if (precision <= 0)
			{
				self.precisionMask = 0;
				return;
			}
			else if (precision > 64)
			{
				self.precisionMask = 0;
				return;
			}
			self.precisionMask = (1L << (precision - 1));
		}

		/// <summary>
		/// 添加定序器 
		/// </summary>
		public static long AddTimer(this CascadeTicker self, long clockTick, INode node, long ruleType)
		{
			if (!self.Core.RuleManager.TryGetRuleList(node.Id, ruleType, out var ruleList)) return -1;

			//大小比较，过时了就直接执行
			if (self.advanceTick >= clockTick)
			{
				self.RuleMulticast.TryAdd(node, ruleList);
				return -1;
			}

			//记录添加的最小时序,用于自适应追赶
			if (self.minTick == 0 || clockTick < self.minTick || self.advanceTick >= self.minTick)
			{
				self.minTick = clockTick;
			}

			//算出槽位位置，AdvanceTick 与 clockTick 的异或后的最高位1所在的位置
			int slotIndex = MathBit.GetHighestBitIndex(self.advanceTick ^ clockTick);
			self.AddChild(out CascadeTickerData tickerData, clockTick, node, ruleList);
			self.GetOrNewSlot(slotIndex).Add(tickerData);
			//标记槽位已占用
			self.OccupiedSlotMask |= 1L << slotIndex;
			return tickerData.Id;
		}

		/// <summary>
		/// 移除定序器 
		/// </summary>
		public static void RemoveTimer(this CascadeTicker self, long tickerId) => self.RemoveChild(tickerId);

		/// <summary>
		/// 更新 
		/// </summary>
		public static void Update(this CascadeTicker self, long currentTick)
		{
			self.CurrentTick = currentTick;
			//没有占用槽位则直接返回
			if (self.OccupiedSlotMask == 0) return;
			//回退不处理，等待CurrentTick追上LastTick后自然恢复
			if (self.CurrentTick < self.LastTick) return;
			//获取时序变化差异
			var numberDiff = self.CurrentTick ^ self.LastTick;
			//没有变化就直接返回
			if (numberDiff == 0) return;
			//获取占用槽位的最低位置
			var minSlot = MathBit.GetLowestBitIndex(self.OccupiedSlotMask);
			//如果最低槽位比当前变化大，说明没有要处理的，直接返回。
			if (minSlot == -1 || (1L << minSlot) > numberDiff) return;

			//前进追赶
			do
			{
				//选择较小的推进,精度刻度推进
				self.advanceTick = (self.precisionMask == 0) ? self.CurrentTick : Math.Min((self.advanceTick + self.precisionMask) & ~(self.precisionMask - 1), self.CurrentTick);
				//按记录的最小时序进行自适应追赶
				self.advanceTick = Math.Min(self.advanceTick, self.minTick);
				//重置最小时序记录
				self.minTick = long.MaxValue;
				//追赶时序
				self.Advance(self.advanceTick);
				//没有占用槽位则直接返回
				if (self.OccupiedSlotMask == 0) return;
			} while (self.advanceTick < self.CurrentTick);
			self.advanceTick = self.CurrentTick;
		}

		/// <summary>
		/// 推进追赶
		/// </summary>
		private static void Advance(this CascadeTicker self, long adviceTick)
		{
			//获取时序变化差异
			var numberDiff = adviceTick ^ self.LastTick;
			//没有变化就直接返回
			if (numberDiff == 0) return;
			//获取占用槽位的最低位置
			var minSlot = MathBit.GetLowestBitIndex(self.OccupiedSlotMask);
			//如果最低槽位比当前变化大，说明没有要处理的，直接返回。
			if (minSlot == -1 || (1L << minSlot) > numberDiff) return;
			//如果变化的时序大于已占用槽位，直接遍历所有槽位，否则只遍历本次时序变化内涉及的槽位。
			var clampDiffSlot = numberDiff > self.OccupiedSlotMask ? self.OccupiedSlotMask : numberDiff;
			var max = MathBit.GetHighestBitIndex(clampDiffSlot);
			var min = minSlot;
			//时序推进正序遍历，避免重复处理下移的定序器
			for (int i = min; i <= max; i++) self.SlotUpdate(i);
			//时序更新
			self.LastTick = adviceTick;
			//执行定, 然后清空执行器
			self.RuleMulticast.Send();
			self.RuleMulticast.Clear();
		}

		/// <summary>
		/// 槽位更新
		/// </summary>
		private static void SlotUpdate(this CascadeTicker self, int i)
		{
			//槽位未占用则跳过
			if ((self.OccupiedSlotMask & (1L << i)) == 0) return;
			var slot = self.GetOrNewSlot(i);

			//遍历槽位内的定序器
			slot.TickIterator.RefreshTraversalCount();
			for (int j = 0; j < slot.TickIterator.TraversalCount; j++)
			{
				if (!slot.TickIterator.TryGetNext(out var nodeRef)) continue;
				var tickerData = nodeRef.Value;
				//移除空定序器
				if (tickerData == null || tickerData.IsNull)
				{
					slot.TickIterator.DequeueCurrent();
					tickerData?.Dispose();
					continue;
				}
				//时序到达，执行定序器
				if (self.advanceTick >= tickerData.Tick)
				{

					//添加到执行器
					self.RuleMulticast.TryAdd(tickerData.Node.Value, tickerData.RuleList);
					//从槽位移除
					tickerData.Dispose();
					slot.TickIterator.DequeueCurrent();

				}
				//时序未到达，重新计算槽位位置
				else
				{
					//重新计算槽位位置
					int slotIndex = MathBit.GetHighestBitIndex(self.advanceTick ^ tickerData.Tick);
					//槽位未变更则跳过
					if (slotIndex == i) continue;
					//移到新的槽位
					self.GetOrNewSlot(slotIndex).Add(tickerData);
					//从当前槽位移除
					slot.TickIterator.DequeueCurrent();
					//标记槽位已占用
					self.OccupiedSlotMask |= 1L << slotIndex;
				}
			}
			//槽位已空，清除占用标记
			if (slot.TickIterator.TraversalCount == 0) self.OccupiedSlotMask &= ~(1L << i);
		}

		/// <summary>
		/// 获取槽位 
		/// </summary>
		private static CascadeTickerSlot GetOrNewSlot(this CascadeTicker self, int slotIndex) => self.Slots[slotIndex] ??= self.AddChild(out CascadeTickerSlot slot);
	}
}
