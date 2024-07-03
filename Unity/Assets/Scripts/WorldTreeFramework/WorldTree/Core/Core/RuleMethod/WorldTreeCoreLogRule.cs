/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/21 04:28:30

* 描述： 日志打印
* 

*/

namespace WorldTree
{
	public static partial class WorldTreeCoreRule
	{
		/// <summary>
		/// 打印日志
		/// </summary>
		public static void Log(this IWorldTreeBasic self, object message) => self.Core.Log?.Invoke(message);
		/// <summary>
		/// 打印警告日志
		/// </summary>
		public static void LogWarning(this IWorldTreeBasic self, object message) => self.Core.LogWarning?.Invoke(message);
		/// <summary>
		/// 打印错误日志
		/// </summary>
		public static void LogError(this IWorldTreeBasic self, object message) => self.Core.LogError?.Invoke(message);
	}
}
