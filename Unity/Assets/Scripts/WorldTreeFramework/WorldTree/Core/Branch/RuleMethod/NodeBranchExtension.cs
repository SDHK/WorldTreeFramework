/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：节点分支处理扩展类

*/

namespace WorldTree
{
	/// <summary>
	/// 节点分支处理扩展类
	/// </summary>
	public static partial class NodeBranchExtension
	{
		#region 获取

		/// <summary>
		/// 尝试获取分支
		/// </summary>
		public static bool TryGetBranch<B>(this INode self, out B branch) where B : class, IBranch
			=> (branch = (self.BranchDict != null && self.BranchDict.TryGetValue(self.TypeToCode<B>(), out IBranch Ibranch)) ? Ibranch as B : null) != null;

		/// <summary>
		/// 获取分支
		/// </summary>
		public static B GetBranch<B>(this INode self) where B : class, IBranch
			=> (self.BranchDict != null && self.BranchDict.TryGetValue(self.TypeToCode<B>(), out IBranch iBranch)) ? iBranch as B : null;

		#endregion
	}
}