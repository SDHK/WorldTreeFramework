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
	/// <summary>
	/// Id节点约束
	/// </summary>
	/// <typeparam name="P">父节点类型</typeparam>
	public interface IdNodeOf<in P> : NodeOf<P, IdNodeBranch> where P : class, INode { }

	public class IdNodeBranch : Branch<long> { }

	public static class NodeIdNodeBranchRule
	{
		#region 获取

		/// <summary>
		/// 尝试获取id节点
		/// </summary>
		public static bool TryGetIdNode(this INode self, long id, out INode node)
		{
			node = null;
			return self.TryGetBranch(out IdNodeBranch branch) && branch.TryGetNodeById(id, out node);
		}

		#endregion

		#region 裁剪

		/// <summary>
		/// 裁剪id节点
		/// </summary>
		public static void TryCutIdNode(this INode self, long id, out INode node) => self.TryCutNode<IdNodeBranch, long>(id, out node);

		#endregion

		#region 嫁接

		/// <summary>
		/// 嫁接id节点
		/// </summary>
		public static void GraftIdNode<N, T>(this N self, long id, T node)
			where N : class, INode
			where T : class, INode, NodeOf<N, IdNodeBranch>
		=> self.TreeGraftNode<IdNodeBranch, long>(id, node);


		#endregion

		#region 移除

		/// <summary>
		/// 根据id移除节点
		/// </summary>
		public static void RemoveIdNode(this INode self, long id) => self.RemoveNode<IdNodeBranch, long>(id);

		/// <summary>
		/// 移除全部id节点
		/// </summary>
		public static void RemoveAllIdNode(this INode self) => self.RemoveAllNode<IdNodeBranch>();

		#endregion

		#region 添加

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddIdNode<N, T>(this N self, long idKey, out T node, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule>
		=> self.AddNode<N, IdNodeBranch, long, T>(idKey, out node, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddChild<N, T, T1>(this N self, long idKey, out T node, T1 arg1, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule<T1>>
		=> self.AddNode<N, IdNodeBranch, long, T, T1>(idKey, out node, arg1, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddChild<N, T, T1, T2>(this N self, long idKey, out T node, T1 arg1, T2 arg2, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule<T1, T2>>
		=> self.AddNode<N, IdNodeBranch, long, T, T1, T2>(idKey, out node, arg1, arg2, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddChild<N, T, T1, T2, T3>(this N self, long idKey, out T node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule<T1, T2, T3>>
		=> self.AddNode<N, IdNodeBranch, long, T, T1, T2, T3>(idKey, out node, arg1, arg2, arg3, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddChild<N, T, T1, T2, T3, T4>(this N self, long idKey, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule<T1, T2, T3, T4>>
		=> self.AddNode<N, IdNodeBranch, long, T, T1, T2, T3, T4>(idKey, out node, arg1, arg2, arg3, arg4, isPool);

		/// <summary>
		/// Id为键值，添加节点
		/// </summary>
		public static T AddChild<N, T, T1, T2, T3, T4, T5>(this N self, long idKey, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, IdNodeBranch>, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
		=> self.AddNode<N, IdNodeBranch, long, T, T1, T2, T3, T4, T5>(idKey, out node, arg1, arg2, arg3, arg4, arg5, isPool);

		#endregion

	}
}
