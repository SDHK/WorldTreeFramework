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
	public enum LogLevel : byte
	{
		/// <summary>
		/// 无日志
		/// </summary>
		None,
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
		/// 跟踪
		/// </summary>
		Trace,
		/// <summary>
		/// 待办
		/// </summary>
		TODO,

		/// <summary>
		/// 所有日志
		/// </summary>
		All = Error | Warning | Debug | Info | Trace | TODO

	}

	/// <summary>
	/// 日志管理器
	/// </summary>
	public class LogManager : Unit, ILog
	{
		/// <summary>
		/// 日志组件
		/// </summary>
		private ILog log;

		/// <summary>
		/// 日志等级
		/// </summary>
		public LogLevel LogLevel;


		static LogManager()
		{
			//获得配置
			NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("../Config/NLog/NLog.config");
			//获得当前目录
			NLog.LogManager.Configuration.Variables["currentDir"] = Environment.CurrentDirectory;
		}

		/// <summary>
		/// 初始化
		/// </summary>
		public void Init(Type logType, LogLevel logLevel)
		{
			//设置日志等级
			LogLevel = logLevel;
			log = (ILog)Core.PoolGetUnit(this.TypeToCode(logType));
		}

		public void Trace(string message)
		{
			if ((LogLevel & LogLevel.Trace) != LogLevel.Trace) return;
			log.Trace(message);
		}

		public void Debug(string message)
		{
			if ((LogLevel & LogLevel.Debug) != LogLevel.Debug) return;
			log.Debug(message);
		}

		public void Error(string message)
		{
			if ((LogLevel & LogLevel.Error) != LogLevel.Error) return;
			log.Error(message);
		}

		public void Error(Exception e)
		{
			if ((LogLevel & LogLevel.Error) != LogLevel.Error) return;
			log.Error(e);
		}

		public void Info(string message)
		{
			if ((LogLevel & LogLevel.Info) != LogLevel.Info) return;
			log.Info(message);
		}

		public void Todo(string message)
		{
			if ((LogLevel & LogLevel.TODO) != LogLevel.TODO) return;
			log.Todo(message);
		}

		public void Warning(string message)
		{
			if ((LogLevel & LogLevel.Warning) != LogLevel.Warning) return;
			log.Warning(message);
		}
	}
}
