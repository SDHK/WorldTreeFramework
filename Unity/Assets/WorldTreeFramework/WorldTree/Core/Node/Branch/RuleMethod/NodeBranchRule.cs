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
