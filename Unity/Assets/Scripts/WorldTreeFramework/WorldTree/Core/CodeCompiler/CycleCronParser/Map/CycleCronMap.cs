namespace WorldTree
{

	/// <summary>
	/// 周期Cron表达式位图 (36字节，288位)
	/// 高性能调度规则存储结构，通过位图实现极致的内存占用和匹配速度
	/// 
	/// 设计理念：
	/// - 模式与表达式分离：Mode作为独立字段，表达式更接近传统Cron
	/// - 职责分离：只存储调度规则，时区由外部DateTime处理
	/// - 位图编码：所有时间字段用位图存储，实现O(1)匹配性能
	/// </summary>
	public class CycleCronMap
	{
		/// <summary>
		/// 模式标识 (1-4，预留5-255扩展)
		/// 决定了表达式的解析方式和字段含义
		/// 注意：Mode不在表达式字符串中，而是作为独立参数传入
		/// </summary>
		public CycleCronMode Mode;

		/// <summary>
		/// 秒位图 (0-59)
		/// 64位可表示60个值，每个bit对应一个秒数
		/// </summary>
		public ulong Seconds;

		/// <summary>
		/// 分钟位图 (0-59)
		/// 64位可表示60个值，每个bit对应一个分钟数
		/// </summary>
		public ulong Minutes;

		/// <summary>
		/// 小时位图 (0-23)
		/// 32位中使用低24位，每个bit对应一个小时
		/// </summary>
		public uint Hours;

		/// <summary>
		/// 日期/星期位图 (根据模式复用不同含义)
		/// - 模式1 (DateMonthYear): 位1-31表示日历日期 (1号、2号...31号)
		/// - 模式2/3 (WeekMonthYear/Week): 位1-7表示星期几 (1=周一, 2=周二...7=周日)
		///   注：星期复用此字段，虽然不是"日期"，但都是时间点的定位
		/// - 模式4 (Day): 不使用此字段 (Day模式的天数存储在Cycles字段中)
		/// </summary>
		public uint Dates;

		/// <summary>
		/// 月位图 (1-12)
		/// 16位中使用低13位，bit1-12对应月份1-12
		/// </summary>
		public ushort Months;

		/// <summary>
		/// 轮次位图 + 轮次范围 (64位)
		/// 位63-56: 轮次范围 (8位, 1-56, 0表示默认56)
		/// 位55-0:  轮次位图 (56位, 每位对应一个轮次)
		/// </summary> 
		public ulong Cycles;

		/// <summary>
		/// 日期/星期特殊符号编码 (8位, 配合Dates字段使用)
		/// 
		/// === 模式1 (DateMonthYear): 日期特殊符号 ===
		/// bit 7-6: 符号类型 (2位, 4种状态)
		///   00 (0) = 普通日期（不使用此字段）
		///   01 (1) = L (Last, 月末倒数)
		///   10 (2) = W (Weekday, 工作日调整)
		///   11 (3) = LW (月末最后一个工作日)
		/// bit 5-0: 日期值/偏移量 (6位, 0-63)
		/// 示例: 0x4F=15W(15号最近的工作日), 0x40=L(月末), 0x41=L-1(月末倒数第2天), 0xC0=LW(月末工作日)
		/// 
		/// === 模式2/3 (WeekMonthYear/Week): 星期特殊符号 ===
		/// bit 7-6: 符号类型 (2位, 统一编码)
		///   01 (1) = L (最后一个星期N)
		///   10 (2) = # (第N个星期X)
		/// bit 5-3: 第N个 (1-5, #时使用, L时为0)
		/// bit 2-0: 星期几 (1-7, 1=周一...7=周日)
		/// 示例: 0x9D=5#3(第3个周五), 0x45=5L(最后一个周五)
		/// 
		/// === 模式4 (Day): 不使用 ===
		/// 0x00
		/// </summary>
		public byte DateOffset;

		// 总计: Mode(8) + Seconds(64) + Minutes(64) + Hours(32) + Dates(32) + Months(16) + Cycles(64) + DateOffset(8) = 288位 = 36字节
	}

}
