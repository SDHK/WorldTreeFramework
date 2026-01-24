/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/19 15:59

* 描述： 级联定序器
* 
* 在级联定序器中，时间是以一个递增的long来表示的。
* 默认会自动根据Update频率适应精度。
* 而 Precision 可以用来限制每次追赶的最小单位。
* 
* 时间单位可以自定义：
* 它可以表示 DateTime.Ticks、帧数，或者任何自定义的时间戳。
* 
* 核心设计原则：
* 1. 事件的执行是由时间差异的位级结构变化所触发的，
*    而非通过计数节拍或旋转轮盘来实现。
*    通过位运算 GetHighestBitIndex(advanceTick ^ clockTick) 直接定位槽位，无需分层查找或递归。
*    
* 2. 进程能够安全地跳过空闲时间。
*    没有待处理事件的长时间段会在 O(1) 时间内被跳过，
*    不会出现空循环或逐帧推进的情况。
*    通过位运算计算变化范围，只处理实际变化的槽位，避免全部遍历。
*    
* 3. 位级结构。
*    事件按照层级进行组织和降级处理，这些层级由当前时间与目标时间之间差异的最高位所决定。
*    64个槽位按位划分（槽位i对应2^i到2^(i+1)的范围），单层覆盖从 2^0 到 2^63 的无限时间范围。
*    相比传统时间轮的分层递归，本设计通过位运算实现单层无限范围覆盖。
*	 
* 4. Precision精度（0, 1~64）。
*    启用时（precision > 0），通过位对齐操作将时间推进与 2^(precision-1) 的倍数对齐，
*    强制执行执行顺序和帧级别的确定性。
*    禁用为0时，直接追赶到当前时间，只保证事件不会被跳过，但不保证事件的执行顺序。
*    示例： 使用C#的DateTime.Ticks作为时间单位，那么：
*    		precision = 0 时，直接追赶到当前时间，只保证事件不会被跳过，但不保证事件的执行顺序。
*			precision = 13 对应 2^14 = 8192 Ticks ≈ 1 毫秒精度。
*          	precision = 18 对应 2^19 = 262114 Ticks ≈ 25 毫秒精度（游戏引擎帧级）。
*			precision = 23 对应 2^23 = 8388608 Ticks ≈ 1 秒精度。
*    
* 5. 该程序的设计中运算操作极为精简。
*    除了用于边界移动的基本加法和减法运算外，
*    所有结构方面的决策都依赖于位运算。
*    不存在除法、取模运算或循环索引计算。
*    相比传统时间轮的除法、取模、堆操作，本设计通过位运算实现 O(1) 定位。
*    
* 性能特点：
* - 添加：O(1) 位运算定位，无需分层查找。
* - 槽位定位：O(1) 位运算，无需递归或堆操作。
* - 任务降级：O(1) 位运算重新定位，无需重新排序。
* - 更新处理：O(1) 位运算定位变化的槽位，避免全部遍历。
* - 相比传统时间轮：避免除法、取模、堆操作的开销。

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 级联定序器
	/// </summary>
	public class CascadeTicker : Node
		, ChildOf<RealTimeManager>
		, ChildOf<GameTimeManager>
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
		/// 最小时序记录：保证事件不会被跳过
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
				self.Core.PoolGetArray(out self.Slots, 2);
				self.AddChild(out self.RuleMulticast);
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
				self.precisionMask = 0;
				self.minTick = long.MaxValue;
				self.OccupiedSlotMask = 0;
				self.Slots = null;
				self.RuleMulticast = null;
			}
		}

		/// <summary>
		/// 确保槽位数组容量足够（动态扩容）
		/// </summary>
		private static void EnsureSlotsCapacity(this CascadeTicker self, int index)
		{
			if (index >= self.Slots.Length)
			{
				// 计算新的容量（按2的幂次扩容）
				int newCapacity = self.Slots.Length;
				while (newCapacity <= index)
				{
					newCapacity *= 2;
				}

				// 扩容数组
				var newSlots = self.Core.PoolGetArray<CascadeTickerSlot>(newCapacity);

				// 复制旧数据
				Array.Copy(self.Slots, 0, newSlots, 0, self.Slots.Length);
				self.Core.PoolRecycle(self.Slots, true);
				self.Slots = newSlots;
			}
		}

		/// <summary>
		/// 获取槽位（不存在则创建）
		/// </summary>
		private static CascadeTickerSlot GetOrNewSlot(this CascadeTicker self, int slotIndex)
		{
			// 防御性检查：理论上不应该出现负数，但为了安全起见
			if (slotIndex < 0) slotIndex = 0;
			self.EnsureSlotsCapacity(slotIndex);
			return self.Slots[slotIndex] ??= self.AddChild(out CascadeTickerSlot slot);
		}

		/// <summary>
		/// 设置指针每帧最大跨越刻度：0表示不限制。1~64（也就是2的幂）
		/// </summary>
		public static void SetPrecision(this CascadeTicker self, int precision)
		{
			if (precision <= 0 || precision > 64)
			{
				self.precisionMask = 0;
				return;
			}
			self.precisionMask = (1L << (precision - 1));
		}

		/// <summary>
		/// 添加定序器 
		/// </summary>
		public static long AddTicker(this CascadeTicker self, long clockTick, INode node, long ruleType)
		{
			if (!self.Core.RuleManager.TryGetRuleList(node.Id, ruleType, out var ruleList)) return -1;

			//大小比较，过时了就直接执行
			if (self.advanceTick >= clockTick)
			{
				self.RuleMulticast.TryAdd(node, ruleList);
				return -1;
			}

			//记录添加的最小时序,用于自适应追赶
			if (self.minTick == long.MaxValue || clockTick < self.minTick || self.advanceTick >= self.minTick)
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
		public static void RemoveTicker(this CascadeTicker self, long tickerId) => self.RemoveChild(tickerId);

		/// <summary>
		/// 更新 
		/// </summary>
		public static void Update(this CascadeTicker self, long currentTick)
		{
			self.CurrentTick = currentTick;
			//没有占用槽位则直接返回
			if (self.OccupiedSlotMask == 0)
			{
				self.advanceTick = self.CurrentTick;
				self.LastTick = self.advanceTick;
				return;
			}
			//回退不处理，等待CurrentTick追上LastTick后自然恢复
			if (self.CurrentTick < self.LastTick) return;
			//获取时序变化差异
			var numberDiff = self.CurrentTick ^ self.LastTick;
			//没有变化就直接返回
			if (numberDiff == 0)
			{
				self.advanceTick = self.CurrentTick;
				self.LastTick = self.advanceTick;
				return;
			}
			//获取占用槽位的最低位置
			var minSlot = MathBit.GetLowestBitIndex(self.OccupiedSlotMask);
			//如果最低槽位比当前变化大，说明没有要处理的，直接返回。
			if (minSlot == -1 || (1L << minSlot) > numberDiff)
			{
				//同步推进时序，越过空闲时序段，避免无效追赶。
				self.advanceTick = self.CurrentTick;
				self.LastTick = self.advanceTick;
				return;
			}

			//MinTick记录了每帧添加的最小时序，所以这一次推进是绝对安全的，越过空闲时序段，避免无效追赶。
			else if (self.minTick <= self.CurrentTick)
			{
				self.advanceTick = self.minTick;
				//重置最小时序记录
				self.minTick = long.MaxValue;
				//追赶时序
				self.Advance(self.advanceTick);
				//时序更新
				self.LastTick = self.advanceTick;
			}

			//前进追赶，原因是MinTick不知道第二个及以后的时序，所以只能逐步追赶。
			while (self.advanceTick < self.CurrentTick && self.OccupiedSlotMask != 0)
			{
				//选择较小的推进,刻度推进
				self.advanceTick = (self.precisionMask == 0) ? self.CurrentTick : Math.Min((self.advanceTick + self.precisionMask) & ~(self.precisionMask - 1), self.CurrentTick);
				//按记录的最小时序进行自适应追赶
				self.advanceTick = Math.Min(self.advanceTick, self.minTick);
				//重置最小时序记录
				self.minTick = long.MaxValue;
				//追赶时序
				long safeDiff = self.Advance(self.advanceTick);
				//安全推进
				if (safeDiff != -1) self.advanceTick += safeDiff;
				//时序更新
				self.LastTick = self.advanceTick;
			}
			self.advanceTick = self.CurrentTick;
			self.LastTick = self.advanceTick;
		}

		/// <summary>
		/// 推进追赶
		/// </summary>
		private static long Advance(this CascadeTicker self, long advanceTick)
		{
			//获取时序变化差异
			var numberDiff = advanceTick ^ self.LastTick;
			//没有变化就直接返回
			if (numberDiff == 0) return -1;
			//获取占用槽位的最低位置
			var minSlot = MathBit.GetLowestBitIndex(self.OccupiedSlotMask);
			//如果最低槽位比当前变化大，说明没有要处理的，返回一个安全推进值。
			if ((1L << minSlot) > numberDiff)
			{
				return (1L << minSlot) - (advanceTick & ((1L << minSlot) - 1)) - 1;
			}
			//如果变化的时序大于已占用槽位，直接遍历所有槽位，否则只遍历本次时序变化内涉及的槽位。
			var clampDiffSlot = numberDiff > self.OccupiedSlotMask ? self.OccupiedSlotMask : numberDiff;
			var max = MathBit.GetHighestBitIndex(clampDiffSlot);
			var min = minSlot;
			//时序推进正序遍历，避免重复处理下移的定序器
			for (int i = min; i <= max; i++) self.SlotUpdate(i);
			//执行, 然后清空执行器
			self.RuleMulticast.Send();
			self.RuleMulticast.Clear();
			return -1;
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
			if (slot.TickIterator.RemainCount == 0) self.OccupiedSlotMask &= ~(1L << i);
		}
	}
}
