
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/31 11:30

* 描述： 真实时间管理器，将根据机器时间计时
* 
* 带Utc的是世界标准时间，是0时区时间，
* 无论你在哪个时区，Utc时间都是一样的。
* 
* 无Utc是时区时间，经过了时区偏差的时间。
* 

*/

using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Codice.Client.Common;

namespace WorldTree
{
	/// <summary>
	/// 真实时间管理器
	/// </summary>
	public class RealTimeManager : Node
		, AsAwake
		, ComponentOf<WorldTreeCore>
	{


		/// <summary>
		/// NTP服务器地址列表
		/// </summary>
		public List<string> ntpServers = new() { "cn.ntp.org.cn", "cn.pool.ntp.org", "pool.ntp.org", "edu.ntp.org.cn", "time.windows.com", "time.nist.gov" };

		/// <summary>
		/// 时区偏差
		/// </summary>
		private int timeZone;

		/// <summary>
		/// 时区偏差设置
		/// </summary>
		public int TimeZone
		{
			get
			{
				return this.timeZone;
			}
			set
			{
				this.timeZone = value;
				this.DateTime1970 = UtcDateTime1970.AddHours(TimeZone);
			}
		}

		/// <summary>
		/// 当前时区时间起点1970
		/// </summary>
		public DateTime DateTime1970;

		/// <summary>
		/// 1970年时间
		/// </summary>
		public DateTime UtcDateTime1970;


		/// <summary>
		/// 是否直接使用机器时间
		/// </summary>
		/// <remarks>默认为false, 使用的是内部维护时间，可以防止玩家改时间</remarks>
		public bool isLocal = false;

		/// <summary>
		/// Utc时间请求更新间隔时间（秒）
		/// </summary>
		public float UtcTimeRequestSeconds = 10;

		/// <summary>
		/// Update计时器
		/// </summary>
		public TimeSpan UtcTimeRequestClock;



		/// <summary>
		/// 本地Utc时间
		/// </summary>
		private DateTime localUtcNow;

		/// <summary>
		/// Utc时间计时器
		/// </summary>
		public Stopwatch stopwatchClock;


		/// <summary>
		/// 内部维护累计的Utc时间
		/// </summary>
		public DateTime m_UtcNow;

		/// <summary>
		/// 时间偏差
		/// </summary>
		private long offsetTicks;


		///// <summary>
		///// 启动以来累计的时间
		///// </summary>
		//public TimeSpan UpdateTimeSinceStartup;

		/// <summary>
		/// 请求超时时间
		/// </summary>
		public int ReceiveTimeout = 1000;


		/// <summary>
		/// Utc时间
		/// </summary>
		public DateTime UtcNow => GetUtcNow();


		/// <summary>
		/// 获取Utc时间
		/// </summary>
		/// <remarks>以累计时间为准，机器时间比累计时间快，并小于1分钟，则使用机器时间 </remarks>
		public DateTime GetUtcNow()
		{
			//如果是，直接使用机器时间
			if (isLocal) return DateTime.UtcNow;

			//以累计时间为准，如果时间相差1分钟以内，那么就使用机器时间
			//如果时间相差大于1分钟，那么判为时间跳跃。
			//如果时间相差小于0分钟，那么判为时间倒流。
			offsetTicks = (DateTime.UtcNow - m_UtcNow).Ticks;
			if (offsetTicks >= 0 && offsetTicks <= 600000000)
			{
				RestartTimer();
				return m_UtcNow = DateTime.UtcNow;
			}
			//差值不在这1分钟内，那么就使用累计时间

			//计算 机器时间 和上次获取的偏差。假如 机器时间 小于上次获取的时间，就是时间倒流，那么就将偏差值设置到最大
			offsetTicks = (localUtcNow < DateTime.UtcNow) ? (DateTime.UtcNow - localUtcNow).Ticks : long.MaxValue;

			//从 机器时间 和 Stopwatch计时器 中，拿到偏差最小的时间，除非两个同时出问题。
			offsetTicks = Math.Min(offsetTicks, stopwatchClock.ElapsedTicks);
			RestartTimer();

			//将时间累加到Utc时间
			return m_UtcNow = m_UtcNow.AddTicks(offsetTicks);
		}


		/// <summary>
		/// 一帧时间
		/// </summary>
		public long FrameTime;


		/// <summary>
		/// 一毫秒，10000Tick
		/// </summary>
		public const int MilliTick = 10000;

		/// <summary>
		/// 一秒，1000毫秒
		/// </summary>
		public const int Second = 1000;
		/// <summary>
		/// 一分钟，60000毫秒
		/// </summary>
		public const long Minute = 60000;
		/// <summary>
		/// 一小时，3600000毫秒
		/// </summary>
		public const long Hour = 3600000;
		/// <summary>
		/// 一天，86400000毫秒
		/// </summary>
		public const long Day = 86400000;


		public RealTimeManager()
		{
			m_UtcNow = DateTime.UtcNow;
			//m_UtcNow = this.GetNetworkUtcDateTime();
		}


		/// <summary>
		/// 重启计时器
		/// </summary>
		private void RestartTimer()
		{
			//重置Stopwatch计时器
			stopwatchClock.Restart();
			//重置本地Utc时间
			localUtcNow = DateTime.UtcNow;
		}

	}


	public static partial class RealTimeManagerRule
	{

		class Add : AddRule<RealTimeManager>
		{
			protected override void Execute(RealTimeManager self)
			{
				//获取网络时间
				//开始计时
				self.stopwatchClock = Stopwatch.StartNew();

				self.DateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				self.UtcDateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				self.FrameTime = self.NowUtcTime();
			}
		}

		class UpdateTime : UpdateTimeRule<RealTimeManager>
		{
			protected override void Execute(RealTimeManager self, TimeSpan arg1)
			{
				self.UtcTimeRequestClock += arg1;

				//假如Update计时器超过了请求时间，那么就请求一次网络时间
				if (self.UtcTimeRequestClock.TotalSeconds > self.UtcTimeRequestSeconds)
				{
					self.UtcTimeRequestClock = TimeSpan.Zero;
					//请求网络时间
					self.Log($"请求网络时间!!!");
					self.GetNetworkUtcDateTimeAsync().Coroutine();
				}
			}
		}


		/// <summary>
		/// 请求获取网络时间
		/// </summary>
		/// <remarks>
		/// <para>网络时间与本地快慢相差超过10秒，并且累计时间小于网络时间时，更新为网络时间 </para>
		/// <para>相差在10秒内 或者 所有请求都失败，那么就继续使用累计时间</para>
		/// </remarks>
		public static async TreeTask GetNetworkUtcDateTimeAsync(this RealTimeManager self)
		{
			await self.TreeTaskCompleted();

			foreach (string ntpServer in self.ntpServers)
			{
				DateTime NetTime = await self.GetNetworkUtcDateTimeAsync(ntpServer);
				if (NetTime != default)
				{
					//检测网络时间和本地时间相差
					var offsetTicks = (NetTime - DateTime.UtcNow).Ticks;
					//网络时间快慢相差都在10秒内，则不校准
					if (Math.Abs(offsetTicks) >= 100000000) break;

					//网络时间相差快10秒，判为本地时间倒流了，慢10秒，判为时间跳跃了。

					//此时本地时间已不可信，检测网络时间和累计时间相差
					offsetTicks = (NetTime - self.m_UtcNow).Ticks;
					//如果网络时间比累计时间快，那么就将累计时间校准为网络时间
					if (offsetTicks > 0)
					{
						self.Log($"校准为网络时间：{NetTime}");
						self.m_UtcNow = NetTime;
					}
					break;
				}
			}

			//刷新时间
			self.GetUtcNow();
		}


		public static async TreeTask<DateTime> GetNetworkUtcDateTimeAsync(this RealTimeManager self, string ntpServer)
		{
			var ntpData = new byte[48];
			ntpData[0] = 0x1B; // 设置协议版本号为3（RFC 1305）

			try
			{
				var addresses = await self.GetAwaiter(Dns.GetHostEntryAsync(ntpServer));
				var ipEndPoint = new IPEndPoint(addresses.AddressList[0], 123); // NTP使用的端口号为123

				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				await self.GetAwaiter(socket.ConnectAsync(ipEndPoint));

				// 发送请求
				await self.GetAwaiter(socket.SendAsync(new ArraySegment<byte>(ntpData), SocketFlags.None));
				socket.ReceiveTimeout = self.ReceiveTimeout; // 设置接收超时时间（毫秒）

				// 接收来自服务器的响应
				await self.GetAwaiter(socket.ReceiveAsync(new ArraySegment<byte>(ntpData), SocketFlags.None));
				socket.Dispose();

				// 从响应中提取传输时间戳
				ulong intPart = BitConverter.ToUInt32(ntpData, 40);
				ulong fractPart = BitConverter.ToUInt32(ntpData, 44);

				// 转换为网络字节序
				intPart = SwapEndianness(intPart);
				fractPart = SwapEndianness(fractPart);

				var milliseconds = intPart * 1000 + fractPart * 1000 / 0x100000000L;
				// NTP时间戳是从1900年开始的，将其转换为从1970年开始的Unix时间戳
				var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

				// 成功获取时间
				self.Log($"获取网络时间成功：{networkDateTime}");
				return networkDateTime;
			}
			catch (Exception)
			{
				await self.TreeTaskCompleted();
				self.Log($"获取网络时间失败：！！！！！！");
				// 获取网络时间失败返回默认值
				return default;
			}
		}

		/// <summary>
		/// 获取网络时间
		/// </summary>
		public static DateTime GetNetworkUtcDateTime(this RealTimeManager self)
		{
			foreach (string ntpServer in self.ntpServers)
			{
				if (self.TryGetNetworkUtcDateTime(ntpServer, out DateTime networkDateTime)) return networkDateTime;
			}
			self.Core.Log($"获取网络时间失败，使用本地时间!!!");
			return DateTime.UtcNow;
		}

		/// <summary>
		/// 尝试获取网络时间
		/// </summary>
		/// <param name="ntpServer">ntp服务器地址</param>
		/// <param name="networkDateTime">网络时间</param>
		/// <returns>成功</returns>
		private static bool TryGetNetworkUtcDateTime(this RealTimeManager self, string ntpServer, out DateTime networkDateTime)
		{

			//================================
			// NTP消息大小 - 16字节（RFC 2030）
			var ntpData = new byte[48];

			// 设置协议版本号为3（RFC 1305）
			ntpData[0] = 0x1B;

			try
			{
				var addresses = Dns.GetHostEntry(ntpServer).AddressList;
				var ipEndPoint = new IPEndPoint(addresses[0], 123); // NTP使用的端口号为123
				using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
				{
					socket.Connect(ipEndPoint);

					// 发送请求
					socket.Send(ntpData);
					socket.ReceiveTimeout = self.ReceiveTimeout; // 设置接收超时时间（毫秒）

					// 接收来自服务器的响应
					socket.Receive(ntpData);
					socket.Close();
				}

				// 从响应中提取传输时间戳
				ulong intPart = BitConverter.ToUInt32(ntpData, 40);
				ulong fractPart = BitConverter.ToUInt32(ntpData, 44);

				// 转换为网络字节序
				intPart = SwapEndianness(intPart);
				fractPart = SwapEndianness(fractPart);

				var milliseconds = intPart * 1000 + fractPart * 1000 / 0x100000000L;
				// NTP时间戳是从1900年开始的，将其转换为从1970年开始的Unix时间戳
				networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

				return true; // 成功获取时间
			}
			catch (Exception)
			{
				networkDateTime = DateTime.UtcNow; // 如果失败了，设置机器时间
				return false; // 表示获取网络时间失败
			}
		}

		/// <summary>
		/// 用于转换大端和小端字节序
		/// </summary>
		private static uint SwapEndianness(ulong x)
		{
			return (uint)(((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) + ((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24));
		}




		/// <summary>
		/// 获取毫秒时间戳，从1970年到当前的总毫秒数
		/// </summary>
		public static long NowUtcTime(this RealTimeManager self)
		{
			return (long)(self.UtcNow - self.UtcDateTime1970).TotalMilliseconds;
		}

		/// <summary>
		/// 获取毫秒时间戳，从1970年到当前的总毫秒数
		/// </summary>
		public static long NowTime(this RealTimeManager self)
		{
			return (long)(self.UtcNow - self.DateTime1970).TotalMilliseconds;
		}

		/// <summary> 
		/// 毫秒时间戳转为时间 
		/// </summary>  
		public static DateTime ToUtcDateTime(this RealTimeManager self, long timeMilliSecond)
		{
			return self.UtcDateTime1970.AddMilliseconds(timeMilliSecond);
		}

		/// <summary> 
		/// 毫秒时间戳转为时间 
		/// </summary>  
		public static DateTime ToDateTime(this RealTimeManager self, long timeMilliSecond)
		{
			return self.DateTime1970.AddMilliseconds(timeMilliSecond);
		}

		#region 时间超时检测

		/// <summary>
		/// 检查是否超时
		/// </summary>
		public static bool CheckTimeout(this RealTimeManager self, long startTicks, long rangeTicks)
		{
			return self.UtcNow.Ticks - rangeTicks > startTicks;
		}

		/// <summary>
		/// 检测是否超时：秒
		/// </summary>
		public static bool CheckTimeoutBySeconds(this RealTimeManager self, long startTicks, int seconds)
		{
			return TimeHelper.GetTimeSpanSeconds(startTicks, self.UtcNow.Ticks) > seconds;
		}

		/// <summary>
		/// 检测是否超时：分钟
		/// </summary>
		public static bool CheckTimeoutByMinutes(this RealTimeManager self, long startTicks, int minutes)
		{
			return TimeHelper.GetTimeSpanMinutes(startTicks, self.UtcNow.Ticks) > minutes;
		}

		/// <summary>
		/// 检测是否超时：分钟
		/// </summary>
		public static bool CheckTimeoutByHours(this RealTimeManager self, long startTicks, int hours)
		{
			return TimeHelper.GetTimeSpanHours(startTicks, self.UtcNow.Ticks) > hours;
		}

		/// <summary>
		/// 检测是否超时：天
		/// </summary>
		public static bool CheckTimeoutByDays(this RealTimeManager self, long startTicks, int days)
		{
			return TimeHelper.GetTimeSpanDays(startTicks, self.UtcNow.Ticks) > days;
		}


		#endregion

	}
}
