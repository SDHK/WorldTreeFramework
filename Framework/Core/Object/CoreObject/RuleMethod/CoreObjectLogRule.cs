using System;

namespace WorldTree
{
	public static class CoreObjectLogRule
	{
		/// <summary>
		/// 打印跟踪日志
		/// </summary>
		public static void LogTrace(this ICoreObject self, string message) => self.Core.LogManager?.Trace(message);
		/// <summary>
		/// 打印日志
		/// </summary>
		public static void Log(this ICoreObject self, string message) => self.Core.LogManager?.Log(message);
		/// <summary>
		/// 打印警告日志
		/// </summary>
		public static void LogWarning(this ICoreObject self, string message) => self.Core.LogManager?.Warning(message);
		/// <summary>
		/// 打印异常
		/// </summary>
		public static void LogError(this ICoreObject self, string message) => self.Core.LogManager?.Error(message);
		/// <summary>
		/// 打印异常
		/// </summary>
		public static void LogError(this ICoreObject self, Exception e) => self.Core.LogManager?.Error(e);
		/// <summary>
		/// 打印信息日志
		/// </summary>
		public static void LogInfo(this ICoreObject self, string message) => self.Core.LogManager?.Info(message);
		/// <summary>
		/// 打印待办日志
		/// </summary>
		public static void LogTodo(this ICoreObject self, string message) => self.Core.LogManager?.Todo(message);
	}
}
