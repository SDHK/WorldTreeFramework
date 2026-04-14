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
	public class WorldLog : LogBase
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

		public override void Init(string name)
		{
			if (System.IO.Directory.Exists("../ProjectConfig/NLog"))
			{
				logger = NLog.LogManager.GetLogger(name);
			}
		}

		public override void Trace(string message)
		{
			logger.Trace(message);
			Debug.WriteLine(message);
		}

		public override void Log(string message)
		{
			logger.Debug(message);
			Debug.WriteLine(message);
		}

		public override void Error(string message)
		{
			logger.Error(message);
			Debug.WriteLine(message);
		}

		public override void Error(Exception e)
		{
			logger.Error(e);
			Debug.WriteLine(e);
		}

		public override void Info(string message)
		{
			logger.Info(message);
			Debug.WriteLine(message);
		}

		public override void Todo(string message)
		{
			logger.Warn("[TODO]" + message);
			Debug.WriteLine("[TODO]" + message);
		}

		public override void Warning(string message)
		{
			logger.Warn(message);
			Debug.WriteLine(message);
		}
	}
}
