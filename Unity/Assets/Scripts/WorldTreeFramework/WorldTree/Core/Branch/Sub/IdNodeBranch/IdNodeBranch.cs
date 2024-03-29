/****************************************

* 作者： 闪电黑客
* 日期： 2023/11/21 11:48:23

* 描述： Id节点分支
*
* 主要分支之一
*
* id为键值的分支
*

*/

namespace WorldTree
{
	public interface AsIdNodeBranch : AsBranch<IdNodeBranch>
	{ }

	/// <summary>
	/// Id节点约束
	/// </summary>
	/// <typeparam name="P">父节点类型</typeparam>
	public interface IdNodeOf<in P> : NodeOf<P, IdNodeBranch> where P : class, INode
	{ }

	public class IdNodeBranch : Branch<long>
	{ }

	public static class NodeIdNodeBranchRule
	{
		#region 获取

		/// <summary>
		/// 尝试获取id节点
		/// </summary>
		public static bool TryGetIdNode(this INode self, long idKey, out INode node)
		{
			node = null;
			return self.TryGetBranch(out IdNodeBranch branch) && branch.TryGetNode(idKey, out node);
		}

		/// <summary>
		/// 尝试获取id节点
		/// </summary>
		public static bool TryGetIdNode<N, T>(this N self, long idKey, out T idNode)
			where N : class, INode
			where T : class, INode, NodeOf<N, IdNodeBranch>
		{
			return (idNode = self.TryGetBranch(out IdNodeBranch branch) && branch.TryGetNode(idKey, out INode node) ? node as T : null) != null;
		}

		#endregion

		#region 裁剪

		/// <summary>
		/// 裁剪id节点
		/// </summary>
		public static void TryCutIdNode(this INode self, long idKey, out INode node) => self.TryCutNode<IdNodeBranch, long>(idKey, out node);

		#endregion

		#region 嫁接

		/// <summary>
		/// 尝试嫁接id节点
		/// </summary>
		public static bool TryGraftIdNode<N, T>(this N self, long idKey, T node)
			where N : class, INode
			where T : class, INode, NodeOf<N, IdNodeBranch>
		=> node.TryGraftSelfToTree<IdNodeBranch, long>(idKey, self);

		/// <summary>
		/// 尝试嫁接id节点
		/// </summary>
		public static bool TryGraftIdNode(this INode self, long idKey, INode node) => node.TryGraftSelfToTree<IdNodeBranch, long>(idKey, self);

		#endregion

		#region 移除

		/// <summary>
		/// 根据id移除节点
		/// </summary>
		public static void RemoveIdNode(this INode self, long idKey) => self.RemoveNode<IdNodeBranch, long>(idKey);

		/// <summary>
		/// 移除全部id节点
		/// </summary>
		public static void RemoveAllIdNode(this INode self) => self.RemoveAllNode<IdNodeBranch>();

		#endregion

		#region 添加

		#region 类型

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static INode AddIdNode(this INode self, long idKey, long type, out INode node, bool isPool = true)
			=> self.AddNode<IdNodeBranch, long>(idKey, type, out node, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static INode AddIdNode<T1>(this INode self, long idKey, long type, out INode node, T1 arg1, bool isPool = true)
			=> self.AddNode<IdNodeBranch, long, T1>(idKey, type, out node, arg1, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static INode AddIdNode<T1, T2>(this INode self, long idKey, long type, out INode node, T1 arg1, T2 arg2, bool isPool = true)
			=> self.AddNode<IdNodeBranch, long, T1, T2>(idKey, type, out node, arg1, arg2, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static INode AddIdNode<T1, T2, T3>(this INode self, long idKey, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			=> self.AddNode<IdNodeBranch, long, T1, T2, T3>(idKey, type, out node, arg1, arg2, arg3, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static INode AddIdNode<T1, T2, T3, T4>(this INode self, long idKey, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			=> self.AddNode<IdNodeBranch, long, T1, T2, T3, T4>(idKey, type, out node, arg1, arg2, arg3, arg4, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static INode AddIdNode<T1, T2, T3, T4, T5>(this INode self, long idKey, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			=> self.AddNode<IdNodeBranch, long, T1, T2, T3, T4, T5>(idKey, type, out node, arg1, arg2, arg3, arg4, arg5, isPool);

		#endregion

		#region 泛型

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddIdNode<N, T>(this N self, long idKey, out T node, bool isPool = true)
			where N : class, INode, AsIdNodeBranch
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule>
		=> self.AddNode<N, IdNodeBranch, long, T>(idKey, out node, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddIdNode<N, T, T1>(this N self, long idKey, out T node, T1 arg1, bool isPool = true)
			where N : class, INode, AsIdNodeBranch
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule<T1>>
		=> self.AddNode<N, IdNodeBranch, long, T, T1>(idKey, out node, arg1, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddIdNode<N, T, T1, T2>(this N self, long idKey, out T node, T1 arg1, T2 arg2, bool isPool = true)
			where N : class, INode, AsIdNodeBranch
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule<T1, T2>>
		=> self.AddNode<N, IdNodeBranch, long, T, T1, T2>(idKey, out node, arg1, arg2, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddIdNode<N, T, T1, T2, T3>(this N self, long idKey, out T node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			where N : class, INode, AsIdNodeBranch
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule<T1, T2, T3>>
		=> self.AddNode<N, IdNodeBranch, long, T, T1, T2, T3>(idKey, out node, arg1, arg2, arg3, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddIdNode<N, T, T1, T2, T3, T4>(this N self, long idKey, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			where N : class, INode, AsIdNodeBranch
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule<T1, T2, T3, T4>>
		=> self.AddNode<N, IdNodeBranch, long, T, T1, T2, T3, T4>(idKey, out node, arg1, arg2, arg3, arg4, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddIdNode<N, T, T1, T2, T3, T4, T5>(this N self, long idKey, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			where N : class, INode, AsIdNodeBranch
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
		=> self.AddNode<N, IdNodeBranch, long, T, T1, T2, T3, T4, T5>(idKey, out node, arg1, arg2, arg3, arg4, arg5, isPool);

		#endregion

		#endregion
	}
}