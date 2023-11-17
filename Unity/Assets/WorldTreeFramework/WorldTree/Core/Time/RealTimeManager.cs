﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/31 11:30

* 描述： 真实时间管理器，将根据机器时间计时
* 
* 以本地时间为主，时区时间为辅

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 真实时间管理器
    /// </summary>
    public class RealTimeManager : Node
        , AsRule<IAwakeRule>
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


        public RealTimeManager()
        {
            this.Awake();
        }
    }

    public static partial class TimeManagerRule
    {
        public static void Awake(this RealTimeManager self)
        {
            self.dateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            self.timeZoneDateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            self.FrameTime = self.NowTime();
        }


        /// <summary>
        /// 获取毫秒，从1970年到当前的总毫秒数
        /// </summary>
        public static long NowTime(this RealTimeManager self)
        {
            return (long)(DateTime.UtcNow - self.dateTime1970).TotalMilliseconds;
        }

        /// <summary> 
        /// 时间戳转为时间 
        /// </summary>  
        public static DateTime ToDateTime(this RealTimeManager self, long timeStamp)
        {
            return self.dateTime1970.AddTicks(timeStamp * RealTimeManager.MilliTick);
        }

        #region 计算时间跨度

        /// <summary>
        /// 获取两个时间戳的跨度
        /// </summary>
        public static TimeSpan GetTimeSpan(long startTimer, long endTimer)
        {
            return new TimeSpan(endTimer).Subtract(new TimeSpan(startTimer));
        }

        /// <summary>
        /// 获取两个时间戳跨度多少秒
        /// </summary>
        public static int GetTimeSpanSeconds(long startTimer, long endTimer)
        {
            return (int)(TimeSpan.FromTicks(endTimer) - TimeSpan.FromTicks(startTimer)).TotalSeconds;
        }

        /// <summary>
        /// 获取两个时间跨度多少秒
        /// </summary>
        public static int GetTimeSpanSeconds(DateTime startTimer, DateTime endTimer)
        {
            return (int)(endTimer - startTimer).TotalSeconds;
        }

        /// <summary>
        /// 获取两个时间戳跨度多少分钟
        /// </summary>
        public static int GetTimeSpanMinutes(long startTimer, long endTimer)
        {
            return (int)(TimeSpan.FromTicks(endTimer) - TimeSpan.FromTicks(startTimer)).TotalMinutes;
        }
        /// <summary>
        /// 获取两个时间跨度多少分钟
        /// </summary>
        public static int GetTimeSpanMinutes(DateTime startTimer, DateTime endTimer)
        {
            return (int)(endTimer - startTimer).TotalMinutes;
        }

        /// <summary>
        /// 获取两个时间戳跨度多少小时
        /// </summary>
        public static int GetTimeSpanHours(long startTimer, long endTimer)
        {
            return (int)(TimeSpan.FromTicks(endTimer) - TimeSpan.FromTicks(startTimer)).TotalHours;
        }
        /// <summary>
        /// 获取两个时间跨度多少小时
        /// </summary>
        public static int GetTimeSpanHours(DateTime startTimer, DateTime endTimer)
        {
            return (int)(endTimer - startTimer).TotalHours;
        }

        /// <summary>
        /// 获取两个时间戳跨度多少天
        /// </summary>
        public static int GetTimeSpanDays(long startTimer, long endTimer)
        {
            return (int)(TimeSpan.FromTicks(endTimer) - TimeSpan.FromTicks(startTimer)).TotalDays;
        }

        /// <summary>
        /// 获取两个时间跨度多少天
        /// </summary>
        public static int GetTimeSpanDays(DateTime startTimer, DateTime endTimer)
        {
            return (int)(endTimer - startTimer).TotalDays;
        }

        #endregion

        #region 时间超时检测

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
        public static bool CheckTimeoutBySeconds(long start, int seconds)
        {
            return GetTimeSpanSeconds(start, DateTime.Now.Ticks) > seconds;
        }

        /// <summary>
        /// 检测是否超时：分钟
        /// </summary>
        public static bool CheckTimeoutByMinutes(long start, int minutes)
        {
            return GetTimeSpanMinutes(start, DateTime.Now.Ticks) > minutes;
        }

        /// <summary>
        /// 检测是否超时：分钟
        /// </summary>
        public static bool CheckTimeoutByHours(long start, int hours)
        {
            return GetTimeSpanHours(start, DateTime.Now.Ticks) > hours;
        }

        /// <summary>
        /// 检测是否超时：天
        /// </summary>
        public static bool CheckTimeoutByDays(long start, int days)
        {
            return GetTimeSpanDays(start, DateTime.Now.Ticks) > days;
        }


        #endregion

        #region 时间单位转换

        /// <summary>
        /// 秒转分
        /// </summary>
        public static float ConvertSecondsToMinute(int seconds) => seconds / 60;
        /// <summary>
        /// 分转秒
        /// </summary>
        public static float ConvertMinuteToSeconds(int minute) => minute * 60;

        /// <summary>
        /// 分转时
        /// </summary>
        public static float ConvertMinuteToHour(int minute) => minute / 60;
        /// <summary>
        /// 时转分
        /// </summary>
        public static float ConvertHourToMinute(int hour) => hour * 60;

        /// <summary>
        /// 时转天
        /// </summary>
        public static float ConvertHourToDay(int hour) => hour / 24;

        /// <summary>
        /// 天转时
        /// </summary>
        public static float ConvertDayToHour(int day) => day / 24;





        #endregion

    }
}
