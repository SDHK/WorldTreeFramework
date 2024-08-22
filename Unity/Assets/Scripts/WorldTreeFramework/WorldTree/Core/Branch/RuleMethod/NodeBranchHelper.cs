/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：节点分支处理帮助类

*/

namespace WorldTree
{
	/// <summary>
	/// 节点分支处理帮助类
	/// </summary>
	/// <remarks>无约束不安全的用法</remarks>
	public static partial class NodeBranchHelper
	{
		#region 添加

		/// <summary>
		/// 添加分支
		/// </summary>
		public static B AddBranch<B>(INode self) where B : class, IBranch => AddBranch(self, self.TypeToCode<B>()) as B;

		/// <summary>
		/// 添加分支
		/// </summary>
		public static IBranch AddBranch(INode self, long type)
		{
			// 拿到分支字典
			var branchDict = self.GetBranchDict;
			if (!branchDict.TryGetValue(type, out IBranch iBranch))
			{
				branchDict.Add(type, iBranch = self.Core.PoolGetUnit(type) as IBranch);
			}
			return iBranch;
		}

		#endregion

		#region 移除

		/// <summary>
		/// 移除分支中的节点
		/// </summary>
		public static void RemoveBranchNode<B>(INode self, INode node) where B : class, IBranch => RemoveBranchNode(self, self.TypeToCode<B>(), node);

		/// <summary>
		/// 移除分支中的节点
		/// </summary>
		public static void RemoveBranchNode(INode self, long branchType, INode node)
		{
			if (self == null) return;
			if (TryGetBranch(self, branchType, out IBranch branch))
			{
				branch.RemoveNode(node.Id);
				if (branch.Count == 0)
				{
					//移除分支
					self.BranchDict.Remove(branchType);
					if (self.BranchDict.Count == 0)
					{
						self.BranchDict.Dispose();
						self.BranchDict = null;
					}

					//释放分支
					branch.Dispose();
				}
			}
		}

		/// <summary>
		/// 释放分支的所有节点
		/// </summary>
		public static void RemoveAllNode<B>(INode self) where B : class, IBranch
			=> self.RemoveAllNode(self.TypeToCode<B>());

		#endregion

		#region 获取

		/// <summary>
		/// 尝试获取分支
		/// </summary>
		public static bool TryGetBranch(INode self, long branchType, out IBranch branch) => (branch = (self.BranchDict != null && self.BranchDict.TryGetValue(branchType, out branch)) ? branch : null) != null;

		/// <summary>
		/// 获取分支
		/// </summary>
		public static IBranch GetBranch(INode self, long branchType) => (self.BranchDict != null && self.BranchDict.TryGetValue(branchType, out IBranch iBranch)) ? iBranch : null;

		#endregion
	}
}