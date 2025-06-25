/****************************************

* 作者： 闪电黑客
* 日期： 2025/3/31 20:02

* 描述： 

*/
using NLog;
using System;
using System.Diagnostics;

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

		static WorldLog()
		{
			//判断路径是否存在
			if (System.IO.Directory.Exists("../ProjectConfig/NLog"))
			{
				//获得配置
				NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("../ProjectConfig/NLog/NLog.config");
				//获得当前目录
				NLog.LogManager.Configuration.Variables["currentDir"] = Environment.CurrentDirectory;
			}
		}

		public override void OnCreate()
		{
			if (System.IO.Directory.Exists("../ProjectConfig/NLog"))
			{
				this.logger = NLog.LogManager.GetLogger($"{(uint)this.Core.Id:000000}.{this.GetType()}");
			}
		}

		public void Trace(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Trace) != LogLevel.Trace) return;
			logger?.Trace(message);
			Debug.WriteLine(message);
		}

		public void Log(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Debug) != LogLevel.Debug) return;
			logger?.Debug(message);
			Debug.WriteLine(message);
		}

		public void Error(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Error) != LogLevel.Error) return;
			logger?.Error(message);
			Debug.WriteLine(message);
		}

		public void Error(Exception e)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Error) != LogLevel.Error) return;
			logger?.Error(e);
			Debug.WriteLine(e);
		}

		public void Info(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Info) != LogLevel.Info) return;
			logger?.Info(message);
			Debug.WriteLine(message);
		}

		public void Todo(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.TODO) != LogLevel.TODO) return;
			logger?.Warn(message);
			Debug.WriteLine(message);
		}

		public void Warning(string message)
		{
			if ((Core.WorldLineManager.Options.LogLevel & LogLevel.Warning) != LogLevel.Warning) return;
			logger?.Warn(message);
			Debug.WriteLine(message);
		}
	}
}
