/****************************************

* 作者： 闪电黑客
* 日期： 2025/3/31 20:02

* 描述： 

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 日志类
	/// </summary>
	public interface ILog
	{
		/// <summary>
		/// 初始化
		/// </summary>
		public void Init(string name);

		/// <summary>
		/// 跟踪
		/// </summary>
		public void Trace(string message);
		/// <summary>
		/// 记录日志
		/// </summary>
		void Log(string message);
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
		void Todo(string message);
	}

	/// <summary>
	/// 日志基类 
	/// </summary>
	public abstract class LogBase : CoreObject, ILog
	{
		public abstract void Init(string name);
		public abstract void Trace(string message);
		public abstract void Log(string message);
		public abstract void Warning(string message);
		public abstract void Error(string message);
		public abstract void Error(Exception e);
		public abstract void Info(string message);
		public abstract void Todo(string message);
	}
}
