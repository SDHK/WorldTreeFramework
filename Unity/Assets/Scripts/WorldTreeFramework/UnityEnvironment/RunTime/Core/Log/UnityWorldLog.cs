/****************************************

* 作者： 闪电黑客
* 日期： 2025/4/7 20:39

* 描述： 

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// Unity世界日志
	/// </summary>
	public class UnityWorldLog : Unit, ILog
	{
		public void Trace(string msg) => UnityEngine.Debug.Log(msg);

		public void Log(string msg) => UnityEngine.Debug.Log(msg);

		public void Todo(string msg) => UnityEngine.Debug.LogWarning("TODO: " + msg);

		public void Info(string msg) => UnityEngine.Debug.Log(msg);

		public void Warning(string msg) => UnityEngine.Debug.LogWarning(msg);

		public void Error(string msg) => UnityEngine.Debug.LogError(msg);

		public void Error(Exception e) => UnityEngine.Debug.LogException(e);
	}
}
