/****************************************

* 作者： 闪电黑客
* 日期： 2025/3/31 20:02

* 描述： 

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 日志等级
	/// </summary>
	[Flags]
	public enum LogLevel
	{
		/// <summary>
		/// 异常
		/// </summary>
		Error,
		/// <summary>
		/// 警告
		/// </summary>
		Warning,
		/// <summary>
		/// 调试
		/// </summary>
		Debug,
		/// <summary>
		/// 信息
		/// </summary>
		Info,
		/// <summary>
		/// 待办
		/// </summary>
		TODO
	}



	/// <summary>
	/// 日志类
	/// </summary>
	public interface ILog
	{
		/// <summary>
		/// 记录日志
		/// </summary>
		void Debug(string message);
		/// <summary>
		/// 警告
		/// </summary>
		/// <param name="message"></param>
		void Warning(string message);
		/// <summary>
		/// 异常
		/// </summary>
		void Error(string message);
		/// <summary>
		/// 异常
		/// </summary>
		void Error(Exception e);

		/// <summary>
		/// 信息
		/// </summary>
		void Info(string message);
		/// <summary>
		/// 待办
		/// </summary>
		void TODO(string message);

	}

	/// <summary>
	/// 日志类型
	/// </summary>
	public class NLogger : ILog
	{
		/// <summary>
		/// a
		/// </summary>
		private readonly NLog.Logger logger;

		public void Debug(string message)
		{
		}

		public void Error(string message)
		{
		}

		public void Error(Exception e)
		{
		}

		public void Info(string message)
		{
		}

		public void TODO(string message)
		{
		}

		public void Warning(string message)
		{
		}
	}
}
