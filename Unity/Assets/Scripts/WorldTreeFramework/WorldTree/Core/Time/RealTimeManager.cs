/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 16:56

* 描述：

*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WorldTree
{
	/// <summary>
	/// 真实时间管理器
	/// </summary>
	public class RealTimeManager : Node
		, AsAwake
		, CoreManagerOf<WorldTreeCore>
	{
		/// <summary>
		/// NTP服务器地址列表
		/// </summary>
		public List<string> ntpServerList = new() { "cn.ntp.org.cn", "pool.ntp.org", "cn.pool.ntp.org", "edu.ntp.org.cn", "time.windows.com", "time.nist.gov" };

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
				return timeZone;
			}
			set
			{
				timeZone = value;
				DateTime1970 = UtcDateTime1970.AddHours(TimeZone);
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
		/// Utc时间获取
		/// </summary>
		public DateTime UtcNow => GetUtcNow();

		/// <summary>
		/// 是否直接使用机器时间
		/// </summary>
		/// <remarks>默认为false, 可内部维护时间，单机可以防止玩家改时间</remarks>
		public bool isLocal = false;

		/// <summary>
		/// 网络请求超时时间 毫秒
		/// </summary>
		public int receiveTimeout = 1000;

		/// <summary>
		/// Utc时间请求更新间隔时间（毫秒）
		/// </summary>
		public long timeRequestTime = 60000;//1分钟

		/// <summary>
		/// 机器时间和网络时间的阈值 毫秒
		/// </summary>
		/// <remarks>机器时间与网络时间差距在阈值内则不校准</remarks>
		public long networkThresholdTime = 2000;//2秒

		/// <summary>
		/// 机器当前时间和累计时间的阈值 毫秒
		/// </summary>
		/// <remarks>机器时间比累计时间快过阈值，则使用累计时间，并尝试请求网络</remarks>
		public long localThresholdTime = 1000;//1秒

		/// <summary>
		/// 机器计时器和累计计时器的阈值 毫秒
		/// </summary>
		/// <remarks>当两个累加计时器差距超过阈值，尝试请求网络</remarks>
		public long clockThresholdTime = 1000;//1秒

		/// <summary>
		/// 网络请求计时器
		/// </summary>
		private long timeRequestClock;

		/// <summary>
		/// 本地Utc时间计时器
		/// </summary>
		private DateTime localUtcNowClock;

		/// <summary>
		/// Utc时间计时器
		/// </summary>
		private Stopwatch stopwatchClock;

		/// <summary>
		/// 内部维护累计的Utc时间
		/// </summary>
		private DateTime cumulativeUtcTime;

		/// <summary>
		/// NTP消息
		/// </summary>
		private byte[] ntpDatas;

		/// <summary>
		/// 是否请求网络时间
		/// </summary>
		private bool isRequest = false;

		public override void OnCreate()
		{
			base.OnCreate();

			// NTP消息大小 - 16字节（RFC 2030）
			ntpDatas = new byte[48];

			// 设置协议版本号为3（RFC 1305）
			ntpDatas[0] = 0x1B;
			cumulativeUtcTime = DateTime.UtcNow;
			stopwatchClock = Stopwatch.StartNew();
			localUtcNowClock = DateTime.UtcNow;
			timeRequestClock = 0;
		}

		/// <summary>
		/// 获取Utc时间
		/// </summary>
		/// <remarks>若检测出时间跳跃，则开始使用内部累计时间，并尝试请求网络</remarks>
		public DateTime GetUtcNow()
		{
			//如果是，直接使用机器时间
			if (isLocal) return cumulativeUtcTime = DateTime.UtcNow;

			//计算时间偏差
			CumulativeTime();

			//如果触发了网络请求，则不用机器时间
			if (!isRequest)
			{
				//计算 机器时间 和 累计时间 的偏差
				//如果时间相差小于0，那么判为时间倒流。如果时间相差大于 阈值，那么判为时间跳跃。
				long offsetTicks = (DateTime.UtcNow - cumulativeUtcTime).Ticks;
				if (offsetTicks >= 0 && offsetTicks <= (localThresholdTime * TimeHelper.MILLI_TICK))
				{
					//如果时间相差在 阈值 以内，那么就使用机器时间。
					return cumulativeUtcTime = DateTime.UtcNow;
				}
			}

			//到了这里 机器时间不可信。假如请求计时器超过了请求时间，那么就尝试异步请求一次网络时间
			if (timeRequestClock > timeRequestTime * TimeHelper.MILLI_TICK)
			{
				timeRequestClock = 0;

				//请求网络时间刷新
				RequestUtcDateTime();
			}
			return cumulativeUtcTime;
		}

		/// <summary>
		/// 获取累计时间
		/// </summary>
		private void CumulativeTime()
		{
			long offsetTicks;
			if (localUtcNowClock <= DateTime.UtcNow)
			{
				//计算 机器时间 和 上次获取 的差值。
				long localOffsetTicks = (DateTime.UtcNow - localUtcNowClock).Ticks;
				long stopwatchOffsetTicks = stopwatchClock.ElapsedTicks;
				offsetTicks = localOffsetTicks - stopwatchOffsetTicks;

				if (offsetTicks > clockThresholdTime * TimeHelper.MILLI_TICK)
				{
					//如果 两个计时器 时间相差大于 阈值，那么判为当前帧发生 机器时间 跳跃。
					isRequest = true;
				}

				//将时间累加到Utc时间 累计时间
				DateTime localUtcTime = cumulativeUtcTime.AddTicks(localOffsetTicks);
				DateTime stopwatchUtcTime = cumulativeUtcTime.AddTicks(stopwatchOffsetTicks);

				//从 机器时间 和 Stopwatch计时器 中，拿到偏差最小的时间，除非两个同时出问题。
				if (Math.Abs((DateTime.UtcNow - localUtcTime).Ticks) < Math.Abs((DateTime.UtcNow - stopwatchUtcTime).Ticks))
				{
					timeRequestClock += localOffsetTicks;
					cumulativeUtcTime = localUtcTime;
				}
				else
				{
					timeRequestClock += stopwatchOffsetTicks;
					cumulativeUtcTime = stopwatchUtcTime;
				}
			}
			else
			{
				//如果 当前的机器时间 大于 上一次机器时间，判为当前帧发生时间倒流。
				isRequest = true;

				//将偏差时间累加到 请求累计时间
				timeRequestClock += stopwatchClock.ElapsedTicks;
				cumulativeUtcTime = cumulativeUtcTime.AddTicks(stopwatchClock.ElapsedTicks);
			}

			//重置Stopwatch计时器
			stopwatchClock.Restart();

			//重置本地Utc计时器
			localUtcNowClock = DateTime.UtcNow;
		}

		/// <summary>
		/// 请求获取网络时间刷新
		/// </summary>
		/// <remarks>
		/// <para>网络时间与机器快慢相差超过 阈值，并且累计时间小于网络时间时，更新为网络时间</para>
		/// <para>相差在 阈值 内 或者 所有请求都失败，那么就继续使用累计时间</para>
		/// <para>如果获得的 网络时间 比 累计时间 慢，则不校准</para>
		/// </remarks>
		public void RequestUtcDateTime() => RequestUtcDateTimeAsync().Coroutine();

		/// <summary>
		/// 异步请求获取网络时间刷新
		/// </summary>
		/// <remarks>
		/// <para>网络时间与机器快慢相差超过 阈值，并且累计时间小于网络时间时，更新为网络时间</para>
		/// <para>相差在 阈值 内 或者 所有请求都失败，那么就继续使用累计时间</para>
		/// <para>如果获得的 网络时间 比 累计时间 慢，则不校准</para>
		/// </remarks>
		private async TreeTask RequestUtcDateTimeAsync()
		{
			DateTime netTime = await GetNetworkUtcDateTimeAsync();
			if (netTime != default)
			{
				//检测网络时间和机器时间相差
				long offsetMilliseconds = (netTime - DateTime.UtcNow).Milliseconds;

				//网络时间快慢相差都在 阈值 内，则不校准
				if (Math.Abs(offsetMilliseconds) > networkThresholdTime)
				{
					//网络时间相差，快则判为机器时间倒流了，慢则判为时间跳跃了。

					//此时机器时间已不可信，检测网络时间和累计时间相差
					offsetMilliseconds = (netTime - cumulativeUtcTime).Milliseconds;

					//如果网络时间比累计时间快，那么就将累计时间校准为网络时间，慢则不校准。
					if (offsetMilliseconds > 0)
					{
						this.Log($"校准为网络时间：{netTime} {cumulativeUtcTime}");
						cumulativeUtcTime = netTime;

						//重置网络请求标记
						isRequest = false;

						//重置Stopwatch计时器
						stopwatchClock.Restart();

						//重置本地Utc计时器
						localUtcNowClock = DateTime.UtcNow;
					}
				}
			}
		}

		/// <summary>
		/// 异步获取网络时间
		/// </summary>
		public async TreeTask<DateTime> GetNetworkUtcDateTimeAsync()
		{
			return await this.TreeTaskLink(Task.Run(GetNetworkUtcDateTime));
		}

		/// <summary>
		/// 同步阻塞 获取网络时间
		/// </summary>
		private DateTime GetNetworkUtcDateTime()
		{
			foreach (string ntpServer in ntpServerList)
			{
				if (TryGetNetworkUtcDateTime(ntpServer, out DateTime networkDateTime)) return networkDateTime;
			}
			return default;
		}

		/// <summary>
		/// 同步阻塞 尝试获取网络时间
		/// </summary>
		/// <param name="ntpServer">ntp服务器地址</param>
		/// <param name="networkDateTime">网络时间</param>
		/// <returns>成功</returns>
		private bool TryGetNetworkUtcDateTime(string ntpServer, out DateTime networkDateTime)
		{
			Array.Clear(ntpDatas, 1, ntpDatas.Length - 1);
			try
			{
				var addresses = Dns.GetHostEntry(ntpServer).AddressList;
				var ipEndPoint = new IPEndPoint(addresses[0], 123); // NTP使用的端口号为123
				using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
				{
					socket.Connect(ipEndPoint);

					// 发送请求
					socket.Send(ntpDatas);
					socket.ReceiveTimeout = receiveTimeout; // 设置接收超时时间（毫秒）

					// 接收来自服务器的响应
					socket.Receive(ntpDatas);
					socket.Close();
				}

				// 从响应中提取传输时间戳
				ulong intPart = BitConverter.ToUInt32(ntpDatas, 40);
				ulong fractPart = BitConverter.ToUInt32(ntpDatas, 44);

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
				// 获取失败就返回默认值
				networkDateTime = default;
				return false;
			}
		}

		/// <summary>
		/// 用于转换大端和小端字节序
		/// </summary>
		private static uint SwapEndianness(ulong x)
		{
			return (uint)(((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) + ((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24));
		}
	}

	public static partial class RealTimeManagerRule
	{
		private class Add : AddRule<RealTimeManager>
		{
			protected override void Execute(RealTimeManager self)
			{
				self.DateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				self.UtcDateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			}
		}

		private class UpdateTime : UpdateTimeRule<RealTimeManager>
		{
			protected override void Execute(RealTimeManager self, TimeSpan arg1)
			{
				self.GetUtcNow();
			}
		}

		/// <summary>
		/// 获取毫秒时间戳，从1970年到当前的总毫秒数
		/// </summary>
		public static long GetUtcTimeSeconds(this RealTimeManager self)
		{
			return (long)(self.UtcNow - self.UtcDateTime1970).TotalSeconds;
		}

		/// <summary>
		/// 获取毫秒时间戳，从1970年到当前的总毫秒数
		/// </summary>
		public static long GetTimeSeconds(this RealTimeManager self)
		{
			return (long)(self.UtcNow - self.DateTime1970).TotalSeconds;
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
		/// 检查是否超时：时间刻度
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
		/// 检测是否超时：小时
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