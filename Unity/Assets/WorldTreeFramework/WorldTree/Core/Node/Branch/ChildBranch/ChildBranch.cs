/****************************************

* 作者： 闪电黑客
* 日期： 2023/11/11 04:04:48

* 描述： 子节点分支
* 
* 主要分支之一
* 
* 设定根据实例自身id为键的分支。
* 

*/

namespace WorldTree
{
	/// <summary>
	/// 子节点约束
	/// </summary>
	/// <typeparam name="T">父节点类型</typeparam>
	/// <remarks>限制节点可挂的父节点，和Where约束搭配形成结构限制</remarks>

	public interface ChildOf<in P> : NodeOf<P, ChildBranch> where P : class, INode { }

	/// <summary>
	/// 子分支
	/// </summary>
	public class ChildBranch : Branch<long> { }

	public static class NodeChildBranchRule
	{
		#region 获取

		/// <summary>
		/// 尝试获取子节点
		/// </summary>
		public static bool TryGetChild(this INode self, long id, out INode child)
		{
			child = null;
			return self.TryGetBranch(out ChildBranch branch) && branch.TryGetNodeById(id, out child);
		}

		#endregion

		#region 裁剪

		/// <summary>
		/// 裁剪子节点
		/// </summary>
		public static bool TryCutChild(this INode self, long id, out INode node) => self.TryCutNode<ChildBranch, long>(id, out node);

		#endregion

		#region 嫁接

		/// <summary>
		/// 嫁接子节点
		/// </summary>
		public static void GraftChild<N, T>(this N self, T node)
			where N : class, INode
			where T : class, INode, NodeOf<N, ChildBranch>
		=> self.TreeGraftNode<ChildBranch, long>(node.Id, node);

		#endregion

		#region 移除

		/// <summary>
		/// 根据id移除子节点
		/// </summary>
		public static void RemoveChild(this INode self, long id) => self.RemoveNode<ChildBranch, long>(id);

		/// <summary>
		/// 移除全部子节点
		/// </summary>
		public static void RemoveAllChild(this INode self) => self.RemoveBranch<ChildBranch>();

		#endregion

		#region 添加

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T>(this N self, out T node, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<IAwakeRule>
		{
			node = self.GetOrNewNode<T>(isPool);
			return self.TreeAddNode<ChildBranch, long, T>(node.Id, node);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T, T1>(this N self, out T node, T1 arg1, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<IAwakeRule<T1>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return self.TreeAddNode<ChildBranch, long, T, T1>(node.Id, node, arg1);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T, T1, T2>(this N self, out T node, T1 arg1, T2 arg2, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<IAwakeRule<T1, T2>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return self.TreeAddNode<ChildBranch, long, T, T1, T2>(node.Id, node, arg1, arg2);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T, T1, T2, T3>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<IAwakeRule<T1, T2, T3>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return self.TreeAddNode<ChildBranch, long, T, T1, T2, T3>(node.Id, node, arg1, arg2, arg3);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T, T1, T2, T3, T4>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<IAwakeRule<T1, T2, T3, T4>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return self.TreeAddNode<ChildBranch, long, T, T1, T2, T3, T4>(node.Id, node, arg1, arg2, arg3, arg4);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T, T1, T2, T3, T4, T5>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return self.TreeAddNode<ChildBranch, long, T, T1, T2, T3, T4, T5>(node.Id, node, arg1, arg2, arg3, arg4, arg5);
		}

		#endregion
	}

}
