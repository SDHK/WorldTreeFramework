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
}
