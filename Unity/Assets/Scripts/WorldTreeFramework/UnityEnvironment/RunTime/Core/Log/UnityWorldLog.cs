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
	public class UnityWorldLog : LogBase
	{
		/// <summary>
		/// 日志名称 
		/// </summary>
		private string name;
		public override void Init(string name)
		{
			this.name = name;
		}

		public override void Trace(string msg) => UnityEngine.Debug.Log($"[{name}]: {msg}");

		public override void Log(string msg) => UnityEngine.Debug.Log($"[{name}]: {msg}");

		public override void Todo(string msg) => UnityEngine.Debug.LogWarning($"[{name}]: [TODO] {msg}");
		public override void Info(string msg) => UnityEngine.Debug.Log($"[{name}]: {msg}");

		public override void Warning(string msg) => UnityEngine.Debug.LogWarning($"[{name}]: {msg}");

		public override void Error(string msg) => UnityEngine.Debug.LogError($"[{name}]: {msg}");

		public override void Error(Exception e) => UnityEngine.Debug.LogException(e);
	}
}
