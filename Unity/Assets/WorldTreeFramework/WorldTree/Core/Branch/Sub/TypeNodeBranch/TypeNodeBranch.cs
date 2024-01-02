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
	public interface TypeNodeOf<in P> : NodeOf<P, TypeNodeBranch> where P : class, INode { }

	/// <summary>
	/// 类型节点分支
	/// </summary>
	public class TypeNodeBranch : Branch<long> { }

	public static class NodeTypeNodeBranchRule
	{
		#region 获取

		/// <summary>
		/// 尝试获取类型节点
		/// </summary>
		public static bool TryGetTypeNode(this INode self, long type, out INode node)
		{
			node = null;
			return self.TryGetBranch(out TypeNodeBranch branch) && branch.TryGetNode(type, out node);
		}

		/// <summary>
		/// 尝试获取类型节点
		/// </summary>
		public static bool TryGetTypeNode<N, T>(this N self, long type, out T typeNode)
			where N : class, INode
			where T : class, INode, NodeOf<N, TypeNodeBranch>
		{
			return (typeNode = self.TryGetBranch(out TypeNodeBranch branch) && branch.TryGetNode(type, out INode node) ? node as T : null) != null;
		}

		#endregion

		#region 裁剪

		/// <summary>
		/// 裁剪类型节点
		/// </summary>
		public static bool TryCutTypeNode(this INode self, long type, out INode node) => self.TryCutNode<TypeNodeBranch, long>(type, out node);

		#endregion

		#region 嫁接

		/// <summary>
		/// 嫁接类型节点
		/// </summary>
		public static void GraftTypeNode<N, T>(this N self, long type, T node)
			where N : class, INode
			where T : class, INode, NodeOf<N, TypeNodeBranch>
			=> node.GraftSelfToTree<TypeNodeBranch, long>(type, self);

		#endregion

		#region 移除

		/// <summary>
		/// 移除类型节点
		/// </summary>
		public static void RemoveTypeNode(this INode self, long type) => self.RemoveNode<TypeNodeBranch, long>(type);

		/// <summary>
		/// 移除所有类型节点
		/// </summary>
		public static void RemoveAllTypeNode(this INode self) => self.RemoveAllNode<TypeNodeBranch>();

		#endregion

		#region 添加

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T>(this N self, long type, out T node, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<IAwakeRule>
		=> self.AddNode<N, TypeNodeBranch, long, T>(type, out node, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T, T1>(this N self, long type, out T node, T1 arg1, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<IAwakeRule<T1>>
		=> self.AddNode<N, TypeNodeBranch, long, T, T1>(type, out node, arg1, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T, T1, T2>(this N self, long type, out T node, T1 arg1, T2 arg2, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<IAwakeRule<T1, T2>>
		=> self.AddNode<N, TypeNodeBranch, long, T, T1, T2>(type, out node, arg1, arg2, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T, T1, T2, T3>(this N self, long type, out T node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<IAwakeRule<T1, T2, T3>>
		=> self.AddNode<N, TypeNodeBranch, long, T, T1, T2, T3>(type, out node, arg1, arg2, arg3, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T, T1, T2, T3, T4>(this N self, long type, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<IAwakeRule<T1, T2, T3, T4>>
		=> self.AddNode<N, TypeNodeBranch, long, T, T1, T2, T3, T4>(type, out node, arg1, arg2, arg3, arg4, isPool);

		/// <summary>
		/// type为键值，添加节点
		/// </summary>
		public static T AddTypeNode<N, T, T1, T2, T3, T4, T5>(this N self, long type, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TypeNodeBranch>, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
		=> self.AddNode<N, TypeNodeBranch, long, T, T1, T2, T3, T4, T5>(type, out node, arg1, arg2, arg3, arg4, arg5, isPool);


		#endregion

	}

}
