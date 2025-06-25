/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/21 04:28:30

* 描述： 日志打印
* 

*/

using System;

namespace WorldTree
{
	public static partial class WorldTreeCoreRule
	{
		/// <summary>
		/// 打印跟踪日志
		/// </summary>
		public static void LogTrace(this IWorldTreeBasic self, string message) => self.Core.LogManager?.Trace(message);

		/// <summary>
		/// 打印日志
		/// </summary>
		public static void Log(this IWorldTreeBasic self, string message) => self.Core.LogManager?.Log(message);

		/// <summary>
		/// 打印警告日志
		/// </summary>
		public static void LogWarning(this IWorldTreeBasic self, string message) => self.Core.LogManager?.Warning(message);
		/// <summary>
		/// 打印异常
		/// </summary>
		public static void LogError(this IWorldTreeBasic self, string message) => self.Core.LogManager?.Error(message);

		/// <summary>
		/// 打印异常
		/// </summary>
		public static void LogError(this IWorldTreeBasic self, Exception e) => self.Core.LogManager?.Error(e);

		/// <summary>
		/// 打印信息日志
		/// </summary>
		public static void LogInfo(this IWorldTreeBasic self, string message) => self.Core.LogManager?.Info(message);

		/// <summary>
		/// 打印待办日志
		/// </summary>
		public static void LogTodo(this IWorldTreeBasic self, string message) => self.Core.LogManager?.Todo(message);

	}
}
