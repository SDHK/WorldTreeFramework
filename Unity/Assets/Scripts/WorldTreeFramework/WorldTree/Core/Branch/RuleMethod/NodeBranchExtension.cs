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


		/// <summary>
		/// 获取键值
		/// </summary>
		public static long GetNumberKey<N>(this N self)
			where N : class, AsNumberNodeBranch
		{

			if (!NodeBranchHelper.TryGetKey(self, out NumberNodeBranch branch)) return default;
			if (!branch.TryGetNodeKey(self.Id, out long key)) return default;
			return key;
		}
	}
}