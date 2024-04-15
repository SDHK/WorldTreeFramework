namespace WorldTree
{
	public static class NodeBranchRule
	{
		#region 分支处理

		#region 添加

		/// <summary>
		/// 添加分支
		/// </summary>
		public static B AddBranch<B>(this INode self) where B : class, IBranch => self.AddBranch(TypeInfo<B>.TypeCode) as B;

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
		public static void RemoveBranchNode<B>(this INode self, INode node) where B : class, IBranch => self.RemoveBranchNode(TypeInfo<B>.TypeCode, node);

		/// <summary>
		/// 移除分支中的节点
		/// </summary>
		public static void RemoveBranchNode(this INode self, long branchType, INode node)
		{
			if (self.TryGetBranch(branchType, out IBranch branch))
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