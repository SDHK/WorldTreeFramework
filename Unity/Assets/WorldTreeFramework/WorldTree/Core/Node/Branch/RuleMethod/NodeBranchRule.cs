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
				Branchs.Add(Type, iBranch = self.Core.GetUnit(Type) as IBranch);
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

		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T>(this N self, K key, out T node, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule>
		=> node = (T)(self.Contains<B, K>(key) ? self.GetNode<B, K>(key) : self.GetOrNewNode<T>(isPool).AddSelfToTree<B, K>(key, self));

		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T, T1>(this N self, K key, out T node, T1 arg1, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule<T1>>
		=> node = (T)(self.Contains<B, K>(key) ? self.GetNode<B, K>(key) as T : self.GetOrNewNode<T>(isPool).AddSelfToTree<B, K, T1>(key, self, arg1));

		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T, T1, T2>(this N self, K key, out T node, T1 arg1, T2 arg2, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule<T1, T2>>
		=> node = (T)(self.Contains<B, K>(key) ? self.GetNode<B, K>(key) as T : self.GetOrNewNode<T>(isPool).AddSelfToTree<B, K, T1, T2>(key, self, arg1, arg2));

		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T, T1, T2, T3>(this N self, K key, out T node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule<T1, T2, T3>>
		=> node = (T)(self.Contains<B, K>(key) ? self.GetNode<B, K>(key) as T : self.GetOrNewNode<T>(isPool).AddSelfToTree<B, K, T1, T2, T3>(key, self, arg1, arg2, arg3));

		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T, T1, T2, T3, T4>(this N self, K key, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule<T1, T2, T3, T4>>
		=> node = (T)(self.Contains<B, K>(key) ? self.GetNode<B, K>(key) as T : self.GetOrNewNode<T>(isPool).AddSelfToTree<B, K, T1, T2, T3, T4>(key, self, arg1, arg2, arg3, arg4));

		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T, T1, T2, T3, T4, T5>(this N self, K key, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
		=> node = (T)(self.Contains<B, K>(key) ? self.GetNode<B, K>(key) as T : self.GetOrNewNode<T>(isPool).AddSelfToTree<B, K, T1, T2, T3, T4, T5>(key, self, arg1, arg2, arg3, arg4, arg5));

	}
}
