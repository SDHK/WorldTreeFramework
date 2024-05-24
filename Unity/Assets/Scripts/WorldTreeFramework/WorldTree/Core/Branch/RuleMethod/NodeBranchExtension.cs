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
	public static partial class NodeBranchExtension
	{
		#region 分支处理

		#region 添加

		/// <summary>
		/// 添加分支
		/// </summary>
		public static B AddBranch<B>(this INode self) where B : class, IBranch => AddBranch(self, TypeInfo<B>.TypeCode) as B;

		/// <summary>
		/// 添加分支
		/// </summary>
		public static IBranch AddBranch(this INode self, long Type)
		{
			var Branchs = self.Branchs;
			if (!Branchs.TryGetValue(Type, out IBranch iBranch))
			{
				Branchs.Add(Type, iBranch = self.PoolGetUnit(Type) as IBranch);
			}
			return iBranch;
		}

		#endregion

		#region 移除

		/// <summary>
		/// 移除分支中的节点
		/// </summary>
		public static void RemoveBranchNode<B>(this INode self, INode node) where B : class, IBranch => RemoveBranchNode(self, TypeInfo<B>.TypeCode, node);

		/// <summary>
		/// 移除分支中的节点
		/// </summary>
		public static void RemoveBranchNode(this INode self, long branchType, INode node)
		{
			if (self == null) return;
			if (TryGetBranch(self, branchType, out IBranch branch))
			{
				branch.RemoveNode(node.Id);
				if (branch.Count == 0)
				{
					//移除分支
					self.m_Branchs.Remove(branchType);
					if (self.m_Branchs.Count == 0)
					{
						self.m_Branchs.Dispose();
						self.m_Branchs = null;
					}

					//释放分支
					branch.Dispose();
				}
			}
		}

		/// <summary>
		/// 释放分支的所有节点
		/// </summary>
		public static void RemoveAllNode<B>(this INode self) where B : class, IBranch
			=> self.RemoveAllNode(TypeInfo<B>.TypeCode);

		#endregion

		#region 获取

		/// <summary>
		/// 尝试获取分支
		/// </summary>
		public static bool TryGetBranch<B>(this INode self, out B branch) where B : class, IBranch => (branch = (self.m_Branchs != null && self.m_Branchs.TryGetValue(TypeInfo<B>.TypeCode, out IBranch Ibranch)) ? Ibranch as B : null) != null;

		/// <summary>
		/// 尝试获取分支
		/// </summary>
		public static bool TryGetBranch(this INode self, long branchType, out IBranch branch) => (branch = (self.m_Branchs != null && self.m_Branchs.TryGetValue(branchType, out branch)) ? branch : null) != null;

		/// <summary>
		/// 获取分支
		/// </summary>
		public static B GetBranch<B>(this INode self) where B : class, IBranch => (self.m_Branchs != null && self.m_Branchs.TryGetValue(TypeInfo<B>.TypeCode, out IBranch iBranch)) ? iBranch as B : null;

		/// <summary>
		/// 获取分支
		/// </summary>
		public static IBranch GetBranch(this INode self, long branchType) => (self.m_Branchs != null && self.m_Branchs.TryGetValue(branchType, out IBranch iBranch)) ? iBranch : null;

		#endregion

		#endregion
	}
}