using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{

	/// <summary>
	/// 临时节点约束
	/// </summary>
	/// <typeparam name="T">父节点类型</typeparam>
	public interface TempOf<in P> : NodeOf<P, TempBranch> where P : class, INode
	{ }

	/// <summary>
	/// 临时节点分支
	/// </summary>
	public class TempBranch : ChildBranch
	{

	}

	public static class NodeTempBranchRule
	{

		#region 获取

		/// <summary>
		/// 尝试获取临时节点
		/// </summary>
		public static bool TryGetTemp(this INode self, long id, out INode temp)
			=> (temp = self.GetBranch<TempBranch>()?.GetNode(id)) != null;

		/// <summary>
		/// 尝试获取临时节点
		/// </summary>
		public static bool TryGetTemp<N, T>(this N self, long id, out T temp)
			where N : class, INode
			where T : class, INode, NodeOf<N, TempBranch>
		=> (temp = (self.GetBranch<TempBranch>()?.GetNode(id)) as T) != null;

		#endregion

		#region 裁剪

		/// <summary>
		/// 裁剪临时节点
		/// </summary>
		public static bool TryCutTemp(this INode self, long id, out INode node)
			=> (node = self.GetBranch<TempBranch>()?.GetNode(id)?.CutSelf()) != null;

		#endregion

		#region 嫁接

		/// <summary>
		/// 尝试嫁接临时节点
		/// </summary>
		public static bool TryGraftTemp<N, T>(this N self, T node)
			where N : class, INode
			where T : class, INode, NodeOf<N, TempBranch>
		=> node.TryGraftSelfToTree<TempBranch, long>(node.Id, self);

		/// <summary>
		/// 尝试嫁接临时节点
		/// </summary>
		public static bool TryGraftTemp(this INode self, INode node) => node.TryGraftSelfToTree<TempBranch, long>(node.Id, self);

		#endregion

		#region 移除

		/// <summary>
		/// 根据id移除临时节点
		/// </summary>
		public static void RemoveTemp(this INode self, long id)
			=> self.GetBranch<TempBranch>()?.GetNode(id)?.Dispose();

		/// <summary>
		/// 移除全部临时节点
		/// </summary>
		public static void RemoveAllTemp(this INode self)
			=> self.RemoveAllNode(TypeInfo<TempBranch>.TypeCode);

		#endregion

		#region 添加

		#region 类型

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static INode AddTemp(this INode self, long type, out INode node, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<TempBranch, long>(node.Id, self);
		}

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static INode AddTemp<T1>(this INode self, long type, out INode node, T1 arg1, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<TempBranch, long, T1>(node.Id, self, arg1);
		}

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static INode AddTemp<T1, T2>(this INode self, long type, out INode node, T1 arg1, T2 arg2, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<TempBranch, long, T1, T2>(node.Id, self, arg1, arg2);
		}

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static INode AddTemp<T1, T2, T3>(this INode self, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<TempBranch, long, T1, T2, T3>(node.Id, self, arg1, arg2, arg3);
		}

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static INode AddTemp<T1, T2, T3, T4>(this INode self, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<TempBranch, long, T1, T2, T3, T4>(node.Id, self, arg1, arg2, arg3, arg4);
		}

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static INode AddTemp<T1, T2, T3, T4, T5>(this INode self, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<TempBranch, long, T1, T2, T3, T4, T5>(node.Id, self, arg1, arg2, arg3, arg4, arg5);
		}

		#endregion

		#region 泛型

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static T AddTemp<N, T>(this N self, out T node, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TempBranch>, AsRule<Awake>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<TempBranch, long>(node.Id, self);
		}

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static T AddTemp<N, T, T1>(this N self, out T node, T1 arg1, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TempBranch>, AsRule<Awake<T1>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<TempBranch, long, T1>(node.Id, self, arg1);
		}

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static T AddTemp<N, T, T1, T2>(this N self, out T node, T1 arg1, T2 arg2, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TempBranch>, AsRule<Awake<T1, T2>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<TempBranch, long, T1, T2>(node.Id, self, arg1, arg2);
		}

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static T AddTemp<N, T, T1, T2, T3>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TempBranch>, AsRule<Awake<T1, T2, T3>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<TempBranch, long, T1, T2, T3>(node.Id, self, arg1, arg2, arg3);
		}

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static T AddTemp<N, T, T1, T2, T3, T4>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TempBranch>, AsRule<Awake<T1, T2, T3, T4>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<TempBranch, long, T1, T2, T3, T4>(node.Id, self, arg1, arg2, arg3, arg4);
		}

		/// <summary>
		/// 添加临时节点
		/// </summary>
		public static T AddTemp<N, T, T1, T2, T3, T4, T5>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			where N : class, INode
			where T : class, INode, NodeOf<N, TempBranch>, AsRule<Awake<T1, T2, T3, T4, T5>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<TempBranch, long, T1, T2, T3, T4, T5>(node.Id, self, arg1, arg2, arg3, arg4, arg5);
		}

		#endregion

		#endregion

	}
}
