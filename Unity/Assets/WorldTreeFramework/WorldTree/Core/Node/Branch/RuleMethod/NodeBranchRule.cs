namespace WorldTree
{
	public static class NodeBranchRule
	{
		/// <summary>
		/// 分支集合
		/// </summary>
		public static UnitDictionary<long, IBranch> BranchsDictionary(this INode self)
		{
			self.m_Branchs ??= self.PoolGet<UnitDictionary<long, IBranch>>();
			return self.m_Branchs;
		}

		/// <summary>
		/// 添加分支
		/// </summary>
		public static B AddBranch<B>(this INode self)
			where B : class, IBranch
		{
			var Branchs = self.BranchsDictionary();
			if (!Branchs.TryGetValue(TypeInfo<B>.HashCode64, out IBranch iBranch))
			{
				Branchs.Add(TypeInfo<B>.HashCode64, iBranch = self.PoolGet<B>());
				iBranch.Self = self;
			}
			return iBranch as B;
		}

		/// <summary>
		/// 添加分支
		/// </summary>
		public static B AddBranch<B>(this INode self, out B branch)
			where B : class, IBranch
		{
			var Branchs = self.BranchsDictionary();
			if (!Branchs.TryGetValue(TypeInfo<B>.HashCode64, out IBranch iBranch))
			{
				Branchs.Add(TypeInfo<B>.HashCode64, iBranch = self.PoolGet<B>());
				iBranch.Self = self;
			}
			return branch = iBranch as B;
		}

		/// <summary>
		/// 移除分支
		/// </summary>
		public static void RemoveBranch<B>(this INode self) where B : class, IBranch => self.RemoveBranch(TypeInfo<B>.HashCode64);

		/// <summary>
		/// 移除分支
		/// </summary>
		public static void RemoveBranch(this INode self, long branchType)
		{
			if (self.m_Branchs != null && self.m_Branchs.TryGetValue(branchType, out IBranch iBranch))
			{
				self.m_Branchs.Remove(branchType);
				iBranch.Dispose();
				iBranch.Self = null;

				if (self.m_Branchs.Count == 0)
				{
					self.m_Branchs.Dispose();
					self.m_Branchs = null;
				}
			}
		}

		/// <summary>
		/// 尝试获取分支
		/// </summary>
		public static bool TryGetBranch<B>(this INode self, out B branch)
			where B : class, IBranch
		{
			if (self.m_Branchs != null && self.m_Branchs.TryGetValue(TypeInfo<B>.HashCode64, out IBranch iBranch))
			{
				branch = iBranch as B;
				return true;
			}
			else
			{
				branch = null;
				return false;
			}
		}
		/// <summary>
		/// 获取分支
		/// </summary>
		public static B GetBranch<B>(this INode self)
			where B : class, IBranch
		{
			if (self.m_Branchs != null && self.m_Branchs.TryGetValue(TypeInfo<B>.HashCode64, out IBranch iBranch))
			{
				return iBranch as B;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 从父节点分支移除
		/// </summary>
		public static void RemoveInParentBranch(this INode self)
		{
			if (self.Parent == null) return;
			if (self.Parent.m_Branchs == null) return;

			if (self.Parent.m_Branchs.TryGetValue(self.BranchType, out IBranch branch))
			{
				branch.RemoveNode(self);
				if (branch.Count == 0)
				{
					self.Parent.RemoveBranch(self.BranchType);
				}
			}
		}
	}
}
