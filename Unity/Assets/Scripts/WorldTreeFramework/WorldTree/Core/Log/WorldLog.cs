/****************************************

* 作者： 闪电黑客
* 日期： 2025/3/31 20:02

* 描述： 

*/
using NLog;
using System;

namespace WorldTree
{
	/// <summary>
	/// 世界日志 （NLog插件）
	/// </summary>
	public class WorldLog : Unit, ILog
	{
		/// <summary>
		/// 日志插件
		/// </summary>
		private Logger logger;

		public override void OnCreate()
		{
			this.logger = NLog.LogManager.GetLogger($"{(uint)this.Core.Id:000000}.{this.GetType()}");
		}

		public void Trace(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Trace) != LogLevel.Trace) return;
			logger.Trace(message);
		}

		public void Debug(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Debug) != LogLevel.Debug) return;
			logger.Debug(message);
		}

		public void Error(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Error) != LogLevel.Error) return;
			logger.Error(message);
		}

		public void Error(Exception e)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Error) != LogLevel.Error) return;
			logger.Error(e);
		}

		public void Info(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Info) != LogLevel.Info) return;
			logger.Info(message);
		}

		public void Todo(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.TODO) != LogLevel.TODO) return;
			logger.Warn(message);
		}

		public void Warning(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Warning) != LogLevel.Warning) return;
			logger.Warn(message);
		}
	}
}
