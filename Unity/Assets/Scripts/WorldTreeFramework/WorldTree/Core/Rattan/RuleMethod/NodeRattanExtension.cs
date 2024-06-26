/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/12 08:25:17

* 描述： 未完成...

*/

namespace WorldTree.Internal
{
	/// <summary>
	/// 节点藤分支处理扩展类
	/// </summary>
	public static class NodeRattanExtension
	{
		#region 获取

		/// <summary>
		/// 尝试获取藤分支
		/// </summary>
		public static bool TryGetRattan<R>(this INode self, out R rattan) where R : class, IRattan => (rattan = (self.RattanDict != null && self.RattanDict.TryGetValue(TypeInfo<R>.TypeCode, out IRattan IRattan)) ? IRattan as R : null) != null;

		/// <summary>
		/// 获取藤分支
		/// </summary>
		public static R GetRattan<R>(this INode self) where R : class, IRattan => (self.RattanDict != null && self.RattanDict.TryGetValue(TypeInfo<R>.TypeCode, out IRattan iRattan)) ? iRattan as R : null;

		#endregion
	}
}