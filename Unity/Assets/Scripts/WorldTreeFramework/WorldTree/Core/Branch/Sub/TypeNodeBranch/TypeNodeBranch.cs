/****************************************

* 作者： 闪电黑客
* 日期： 2023/11/21 04:58:09

* 描述： 子节点分支
*
* 主要分支之一
*
* 以类型码为键值的分支。
*

*/

namespace WorldTree
{
	/// <summary>
	/// 类型节点约束
	/// </summary>
	/// <typeparam name="P">父节点类型</typeparam>
	public interface TypeNodeOf<in P> : NodeOf<P, TypeNodeBranch> where P : class, INode
	{ }

	/// <summary>
	/// 类型节点分支
	/// </summary>
	public class TypeNodeBranch : Branch<long>
	{ }

	public static class NodeTypeNodeBranchRule
	{
		#region 获取

		/// <summary>
		/// 尝试获取类型节点
		/// </summary>
		public static bool TryGetTypeNode(this INode self, long type, out INode typeNode)
			=> (typeNode = NodeBranchHelper.GetBranch<TypeNodeBranch>(self)?.GetNode(type)) != null;

		/// <summary>
		/// 尝试获取类型节点
		/// </summary>
		public static bool TryGetTypeNode<N, T>(this N self, long type, out T typeNode)
			where N : class, INode
			where T : class, INode, NodeOf<N, TypeNodeBranch>
		=> (typeNode = (NodeBranchHelper.GetBranch<TypeNodeBranch>(self)?.GetNode(type)) as T) != null;

		#endregion

		#region 裁剪

		/// <summary>
		/// 裁剪类型节点
		/// </summary>
		public static bool TryCutTypeNode(this INode self, long type, out INode node)
			=> (node = NodeBranchHelper.GetBranch<TypeNodeBranch>(self)?.GetNode(type)?.CutSelf()) != null;

		#endregion

		#region 嫁接

		/// <summary>
		/// 尝试嫁接类型节点
		/// </summary>
		public static void TryGraftTypeNode<N, T>(this N self, long type, T node)
			where N : class, INode
			where T : class, INode, NodeOf<N, TypeNodeBranch>
			=> node.TryGraftSelfToTree<TypeNodeBranch, long>(type, self);

		/// <summary>
		/// 尝试嫁接类型节点
		/// </summary>
		public static void TryGraftTypeNode(this INode self, long type, INode node)
			=> node.TryGraftSelfToTree<TypeNodeBranch, long>(type, self);

		#endregion

		#region 移除

		/// <summary>
		/// 移除类型节点
		/// </summary>
		public static void RemoveTypeNode(this INode self, long type)
			=> NodeBranchHelper.GetBranch<TypeNodeBranch>(self)?.GetNode(type)?.Dispose();

		/// <summary>
		/// 移除所有类型节点
		/// </summary>
		public static void RemoveAllTypeNode(this INode self)
			=> self.RemoveAllNode(TypeInfo<TypeNodeBranch>.TypeCode);

		#endregion

		#region 添加

		#region 类型

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static INode AddTypeNode(this INode self, long typeKey, long type, out INode node, bool isPool = true)
			=> self.AddNode<TypeNodeBranch, long>(typeKey, type, out node, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static INode AddTypeNode<T1>(this INode self, long typeKey, long type, out INode node, T1 arg1, bool isPool = true)
			=> self.AddNode<TypeNodeBranch, long, T1>(typeKey, type, out node, arg1, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static INode AddTypeNode<T1, T2>(this INode self, long typeKey, long type, out INode node, T1 arg1, T2 arg2, bool isPool = true)
			=> self.AddNode<TypeNodeBranch, long, T1, T2>(typeKey, type, out node, arg1, arg2, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static INode AddTypeNode<T1, T2, T3>(this INode self, long typeKey, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			=> self.AddNode<TypeNodeBranch, long, T1, T2, T3>(typeKey, type, out node, arg1, arg2, arg3, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static INode AddTypeNode<T1, T2, T3, T4>(this INode self, long typeKey, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			=> self.AddNode<TypeNodeBranch, long, T1, T2, T3, T4>(typeKey, type, out node, arg1, arg2, arg3, arg4, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static INode AddTypeNode<T1, T2, T3, T4, T5>(this INode self, long typeKey, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			=> self.AddNode<TypeNodeBranch, long, T1, T2, T3, T4, T5>(typeKey, type, out node, arg1, arg2, arg3, arg4, arg5, isPool);

		#endregion

		#region 泛型

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T>(this N self, long typeKey, out T node, bool isPool = true)
			where N : class, INode, AsBranch<TypeNodeBranch>
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<Awake>
		=> self.AddNode<N, TypeNodeBranch, long, T>(typeKey, out node, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T, T1>(this N self, long typeKey, out T node, T1 arg1, bool isPool = true)
			where N : class, INode, AsBranch<TypeNodeBranch>
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<Awake<T1>>
		=> self.AddNode<N, TypeNodeBranch, long, T, T1>(typeKey, out node, arg1, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T, T1, T2>(this N self, long typeKey, out T node, T1 arg1, T2 arg2, bool isPool = true)
			where N : class, INode, AsBranch<TypeNodeBranch>
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<Awake<T1, T2>>
		=> self.AddNode<N, TypeNodeBranch, long, T, T1, T2>(typeKey, out node, arg1, arg2, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T, T1, T2, T3>(this N self, long typeKey, out T node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			where N : class, INode, AsBranch<TypeNodeBranch>
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<Awake<T1, T2, T3>>
		=> self.AddNode<N, TypeNodeBranch, long, T, T1, T2, T3>(typeKey, out node, arg1, arg2, arg3, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T, T1, T2, T3, T4>(this N self, long typeKey, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			where N : class, INode, AsBranch<TypeNodeBranch>
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<Awake<T1, T2, T3, T4>>
		=> self.AddNode<N, TypeNodeBranch, long, T, T1, T2, T3, T4>(typeKey, out node, arg1, arg2, arg3, arg4, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T, T1, T2, T3, T4, T5>(this N self, long typeKey, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			where N : class, INode, AsBranch<TypeNodeBranch>
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<Awake<T1, T2, T3, T4, T5>>
		=> self.AddNode<N, TypeNodeBranch, long, T, T1, T2, T3, T4, T5>(typeKey, out node, arg1, arg2, arg3, arg4, arg5, isPool);

		#endregion

		#endregion
	}
}