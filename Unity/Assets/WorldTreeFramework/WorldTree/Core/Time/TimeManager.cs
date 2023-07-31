
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/31 11:30

* 描述： 机器时间管理器

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 机器时间管理器
    /// </summary>
    public class TimeManager : Node
    {
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


        /// <summary>
        /// 时区
        /// </summary>
        private int timeZone;
        /// <summary>
        /// 时区
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
                this.timeZoneDateTime1970 = dateTime1970.AddHours(TimeZone);
            }
        }

        /// <summary>
        /// 时间起点1970
        /// </summary>
        public DateTime dateTime1970;

        /// <summary>
        /// 当前时区时间起点1970
        /// </summary>
        public DateTime timeZoneDateTime1970;

        /// <summary>
        /// 帧时间
        /// </summary>
        public long FrameTime;


        public TimeManager()
        {
            this.Awake();
        }
    }

    public static partial class TimeManagerRule
    {
        public static void Awake(this TimeManager self)
        {
            self.dateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            self.timeZoneDateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            self.FrameTime = self.ClientNow();
        }


        /// <summary>
        /// 获取毫秒，从1970年到当前的总毫秒数
        /// </summary>
        public static long ClientNow(this TimeManager self)
        {
            //每一万Tick就是一毫秒
            return (DateTime.UtcNow.Ticks - self.dateTime1970.Ticks) / TimeManager.MilliTick;
        }

        /// <summary> 
        /// 毫秒转为时间 
        /// </summary>  
        public static DateTime ToDateTime(this TimeManager self, long timeStamp)
        {
            return self.timeZoneDateTime1970.AddTicks(timeStamp * TimeManager.MilliTick);
        }

        /// <summary>
        /// 获取两个时间戳跨度多少秒
        /// </summary>
        public static int GetTimeSpanSeconds(long startTimer, long endTimer)
        {
            return (int)GetTimeSpan(startTimer, endTimer).TotalSeconds;
        }

        /// <summary>
        /// 获取两个时间跨度多少天
        /// </summary>
        public static int GetTimeSpanSeconds(DateTime startTimer, DateTime endTimer)
        {
            return (int)GetTimeSpan(startTimer.Ticks, endTimer.Ticks).TotalSeconds;
        }

        /// <summary>
        /// 获取两个时间戳跨度多少天
        /// </summary>
        public static int GetTimeSpanDays(long startTimer, long endTimer)
        {
            return (int)GetTimeSpan(startTimer, endTimer).TotalDays;
        }

        /// <summary>
        /// 获取两个时间戳跨度
        /// </summary>
        public static TimeSpan GetTimeSpan(long startTimer, long endTimer)
        {
            return new TimeSpan(endTimer).Subtract(new TimeSpan(startTimer)).Duration();
        }

        /// <summary>
        /// 检查是否超时
        /// </summary>
        public static bool CheckTimeout(long start, long range)
        {
            return DateTime.Now.Ticks - range > start;
        }

        /// <summary>
        /// 检测是否超时：秒
        /// </summary>
        public static bool CheckTimeoutBySeconds(long start, int Seconds)
        {
            return GetTimeSpanSeconds(start, DateTime.Now.Ticks) > Seconds;
        }

    }
}
