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
		Error = 1,
		/// <summary>
		/// 警告
		/// </summary>
		Warning = 2,
		/// <summary>
		/// 调试
		/// </summary>
		Debug = 4,
		/// <summary>
		/// 信息
		/// </summary>
		Info = 8,
		/// <summary>
		/// 跟踪
		/// </summary>
		Trace = 16,
		/// <summary>
		/// 待办
		/// </summary>
		TODO = 32,

		/// <summary>
		/// 所有日志
		/// </summary>
		All = Error | Warning | Debug | Info | Trace | TODO

	}

	/// <summary>
	/// 日志管理器
	/// </summary>
	public class LogManager : CoreObjectBase, ILog
	{
		/// <summary>
		/// 日志组件
		/// </summary>
		private LogBase log;

		/// <summary>
		/// 初始化
		/// </summary>
		public void Init(string name)
		{
			if (Core.LogType == null) return;
			log = (LogBase)this.CreateCoreObject(Core.LogType);
			log.Init(name);
		}

		public void Trace(string message)
		{
			if ((Core.Options.LogLevel & LogLevel.Trace) != LogLevel.Trace) return;
			log?.Trace(message);
		}

		public void Log(string message)
		{
			if ((Core.Options.LogLevel & LogLevel.Debug) != LogLevel.Debug) return;
			log?.Log(message);
		}

		public void Error(string message)
		{
			if ((Core.Options.LogLevel & LogLevel.Error) != LogLevel.Error) return;
			log?.Error(message);
		}

		public void Error(Exception e)
		{
			if ((Core.Options.LogLevel & LogLevel.Error) != LogLevel.Error) return;
			log?.Error(e);
		}

		public void Info(string message)
		{
			if ((Core.Options.LogLevel & LogLevel.Info) != LogLevel.Info) return;
			log?.Info(message);
		}

		public void Todo(string message)
		{
			if ((Core.Options.LogLevel & LogLevel.TODO) != LogLevel.TODO) return;
			log?.Todo(message);
		}

		public void Warning(string message)
		{
			if ((Core.Options.LogLevel & LogLevel.Warning) != LogLevel.Warning) return;
			log?.Warning(message);
		}
	}
}
