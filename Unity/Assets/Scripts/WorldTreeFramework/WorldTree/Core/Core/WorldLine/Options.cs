/****************************************

* 作者： 闪电黑客
* 日期： 2025/4/17 14:45

* 描述： 启动选项

*/
using CommandLine;

namespace WorldTree
{
	/// <summary>
	/// 启动选项
	/// </summary>
	public class Options
	{
		/// <summary>
		/// 配置路径
		/// </summary>
		[Option("ConfigPath", Required = false, Default = "StartConfig/Localhost")]
		public string ConfigPath { get; set; }

		/// <summary>
		/// 启动进程
		/// </summary>
		[Option("Process", Required = false, Default = 1)]
		public int Process { get; set; } = 1;

		/// <summary>
		/// 日志等级
		/// </summary>
		[Option("LogLevel", Required = false, Default = LogLevel.All)]
		public LogLevel LogLevel { get; set; } = LogLevel.All;
	}
}