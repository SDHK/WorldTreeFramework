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

using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	public interface AsChildBranch : AsBranch<ChildBranch>
	{ }

	/// <summary>
	/// 子节点约束
	/// </summary>
	/// <typeparam name="T">父节点类型</typeparam>
	/// <remarks>限制节点可挂的父节点，和Where约束搭配形成结构限制</remarks>

	public interface ChildOf<in P> : NodeOf<P, ChildBranch> where P : class, INode
	{ }

	/// <summary>
	/// 子分支
	/// </summary>
	public class ChildBranch : UnitPoolItem, IBranch<long>
	{
		public INode Self { get; set; }

		public int Count => Nodes.Count;

		protected UnitDictionary<long, INode> Nodes;

		public override void OnGet()
		{
			Core.PoolGetUnit(out Nodes);
		}

		public bool Contains(long key) => Nodes.ContainsKey(key);

		public bool ContainsId(long id) => Nodes.ContainsKey(id);

		public bool TryAddNode<N>(long key, N node) where N : class, INode => Nodes.TryAdd(key, node);

		public bool TryGetNodeKey(long nodeId, out long key)
		{ key = nodeId; return true; }

		public bool TryGetNode(long key, out INode node) => this.Nodes.TryGetValue(key, out node);

		public bool TryGetNodeById(long id, out INode node) => this.Nodes.TryGetValue(id, out node);

		public INode GetNode(long key) => this.Nodes.TryGetValue(key, out INode node) ? node : null;

		public INode GetNodeById(long id) => this.Nodes.TryGetValue(id, out INode node) ? node : null;

		public void RemoveNode(long nodeId) => Nodes.Remove(nodeId);

		public void Clear()
		{
			Nodes.Clear();
		}

		public IEnumerator<INode> GetEnumerator() => Nodes.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => Nodes.Values.GetEnumerator();

		public override void OnRecycle()
		{
			this.Nodes.Dispose();
			this.Self = null;
			this.Nodes = null;
		}
	}

	public static class NodeChildBranchRule
	{
		#region 获取

		/// <summary>
		/// 尝试获取子节点
		/// </summary>
		public static bool TryGetChild(this INode self, long id, out INode child)
			=> (child = self.GetBranch<ChildBranch>()?.GetNode(id)) != null;

		/// <summary>
		/// 尝试获取子节点
		/// </summary>
		public static bool TryGetChild<N, T>(this N self, long id, out T child)
			where N : class, INode
			where T : class, INode, NodeOf<N, ChildBranch>
		=> (child = (self.GetBranch<ChildBranch>()?.GetNode(id)) as T) != null;

		#endregion

		#region 裁剪

		/// <summary>
		/// 裁剪子节点
		/// </summary>
		public static bool TryCutChild(this INode self, long id, out INode node)
			=> (node = self.GetBranch<ChildBranch>()?.GetNode(id)?.CutSelf()) != null;

		#endregion

		#region 嫁接

		/// <summary>
		/// 尝试嫁接子节点
		/// </summary>
		public static bool TryGraftChild<N, T>(this N self, T node)
			where N : class, INode
			where T : class, INode, NodeOf<N, ChildBranch>
		=> node.TryGraftSelfToTree<ChildBranch, long>(node.Id, self);

		/// <summary>
		/// 尝试嫁接子节点
		/// </summary>
		public static bool TryGraftChild(this INode self, INode node) => node.TryGraftSelfToTree<ChildBranch, long>(node.Id, self);

		#endregion

		#region 移除

		/// <summary>
		/// 根据id移除子节点
		/// </summary>
		public static void RemoveChild(this INode self, long id)
			=> self.GetBranch<ChildBranch>()?.GetNode(id)?.Dispose();

		/// <summary>
		/// 移除全部子节点
		/// </summary>
		public static void RemoveAllChild(this INode self)
			=> self.RemoveAllNode(TypeInfo<ChildBranch>.TypeCode);

		#endregion

		#region 添加

		#region 类型

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static INode AddChild(this INode self, long type, out INode node, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<ChildBranch, long>(node.Id, self);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static INode AddChild<T1>(this INode self, long type, out INode node, T1 arg1, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<ChildBranch, long, T1>(node.Id, self, arg1);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static INode AddChild<T1, T2>(this INode self, long type, out INode node, T1 arg1, T2 arg2, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<ChildBranch, long, T1, T2>(node.Id, self, arg1, arg2);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static INode AddChild<T1, T2, T3>(this INode self, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<ChildBranch, long, T1, T2, T3>(node.Id, self, arg1, arg2, arg3);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static INode AddChild<T1, T2, T3, T4>(this INode self, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<ChildBranch, long, T1, T2, T3, T4>(node.Id, self, arg1, arg2, arg3, arg4);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static INode AddChild<T1, T2, T3, T4, T5>(this INode self, long type, out INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
		{
			node = self.GetOrNewNode(type, isPool);
			return node.AddSelfToTree<ChildBranch, long, T1, T2, T3, T4, T5>(node.Id, self, arg1, arg2, arg3, arg4, arg5);
		}

		#endregion

		#region 泛型

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T>(this N self, out T node, bool isPool = true)
			where N : class, INode, AsChildBranch
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<Awake>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<ChildBranch, long>(node.Id, self);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T, T1>(this N self, out T node, T1 arg1, bool isPool = true)
			where N : class, INode, AsChildBranch
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<Awake<T1>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<ChildBranch, long, T1>(node.Id, self, arg1);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T, T1, T2>(this N self, out T node, T1 arg1, T2 arg2, bool isPool = true)
			where N : class, INode, AsChildBranch
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<Awake<T1, T2>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<ChildBranch, long, T1, T2>(node.Id, self, arg1, arg2);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T, T1, T2, T3>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			where N : class, INode, AsChildBranch
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<Awake<T1, T2, T3>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<ChildBranch, long, T1, T2, T3>(node.Id, self, arg1, arg2, arg3);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T, T1, T2, T3, T4>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			where N : class, INode, AsChildBranch
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<Awake<T1, T2, T3, T4>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<ChildBranch, long, T1, T2, T3, T4>(node.Id, self, arg1, arg2, arg3, arg4);
		}

		/// <summary>
		/// 添加子节点
		/// </summary>
		public static T AddChild<N, T, T1, T2, T3, T4, T5>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			where N : class, INode, AsChildBranch
			where T : class, INode, NodeOf<N, ChildBranch>, AsRule<Awake<T1, T2, T3, T4, T5>>
		{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<ChildBranch, long, T1, T2, T3, T4, T5>(node.Id, self, arg1, arg2, arg3, arg4, arg5);
		}

		#endregion

		#endregion
	}
}