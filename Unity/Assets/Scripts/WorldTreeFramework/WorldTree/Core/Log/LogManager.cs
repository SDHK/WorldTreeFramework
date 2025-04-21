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

		static LogManager()
		{
			//获得配置
			NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("../Config/NLog/NLog.config");
			//获得当前目录
			NLog.LogManager.Configuration.Variables["currentDir"] = Environment.CurrentDirectory;
		}

		public override void OnCreate()
		{
			log = (ILog)Core.PoolGetUnit(this.TypeToCode(Core.WorldLineManager.LogType));
		}

		public void Trace(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Trace) != LogLevel.Trace) return;
			log.Trace(message);
		}

		public void Debug(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Debug) != LogLevel.Debug) return;
			log.Debug(message);
		}

		public void Error(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Error) != LogLevel.Error) return;
			log.Error(message);
		}

		public void Error(Exception e)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Error) != LogLevel.Error) return;
			log.Error(e);
		}

		public void Info(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Info) != LogLevel.Info) return;
			log.Info(message);
		}

		public void Todo(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.TODO) != LogLevel.TODO) return;
			log.Todo(message);
		}

		public void Warning(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Warning) != LogLevel.Warning) return;
			log.Warning(message);
		}
	}
}
