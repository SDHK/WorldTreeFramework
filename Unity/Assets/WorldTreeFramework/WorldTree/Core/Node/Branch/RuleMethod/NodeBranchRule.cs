namespace WorldTree
{
	public static class NodeBranchRule
	{
		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T>(this N self, K key, out T node, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule>
		=> node = self.Contains<B, K>(key) ? self.TreeAddNode<B, K, T>(key, self.GetOrNewNode<T>(isPool)) : null;

		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T, T1>(this N self, K key, out T node, T1 arg1, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule<T1>>
		=> node = self.Contains<B, K>(key) ? self.TreeAddNode<B, K, T, T1>(key, self.GetOrNewNode<T>(isPool), arg1) : null;

		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T, T1, T2>(this N self, K key, out T node, T1 arg1, T2 arg2, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule<T1, T2>>
		=> node = self.Contains<B, K>(key) ? self.TreeAddNode<B, K, T, T1, T2>(key, self.GetOrNewNode<T>(isPool), arg1, arg2) : null;

		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T, T1, T2, T3>(this N self, K key, out T node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule<T1, T2, T3>>
		=> node = self.Contains<B, K>(key) ? self.TreeAddNode<B, K, T, T1, T2, T3>(key, self.GetOrNewNode<T>(isPool), arg1, arg2, arg3) : null;

		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T, T1, T2, T3, T4>(this N self, K key, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule<T1, T2, T3, T4>>
		=> node = self.Contains<B, K>(key) ? self.TreeAddNode<B, K, T, T1, T2, T3, T4>(key, self.GetOrNewNode<T>(isPool), arg1, arg2, arg3, arg4) : null;

		/// <summary>
		/// 添加节点
		/// </summary>
		public static T AddNode<N, B, K, T, T1, T2, T3, T4, T5>(this N self, K key, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
		=> node = self.Contains<B, K>(key) ? self.TreeAddNode<B, K, T, T1, T2, T3, T4, T5>(key, self.GetOrNewNode<T>(isPool), arg1, arg2, arg3, arg4, arg5) : null;

	}
}
