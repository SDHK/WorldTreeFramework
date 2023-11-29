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
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

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
	public class ChildBranch : UnitPoolItem, IBranch<long>
	{
		public INode Self { get; set; }

		public int Count => Nodes.Count;

		protected UnitDictionary<long, INode> Nodes;

		public override void OnGet()
		{
			Core.PoolGet(out Nodes);
		}

		public bool Contains(long key) => Nodes.ContainsKey(key);

		public bool ContainsId(long id) => Nodes.ContainsKey(id);


		public bool TryAddNode<N>(long key, N node) where N : class, INode => Nodes.TryAdd(key, node);

		public bool TryGetNodeKey(INode node, out long key) { key = node.Id; return true; }

		public bool TryGetNode(long key, out INode node) => this.Nodes.TryGetValue(key, out node);
		public bool TryGetNodeById(long id, out INode node) => this.Nodes.TryGetValue(id, out node);

		public INode GetNode(long key) => this.Nodes.TryGetValue(key, out INode node) ? node : null;
		public INode GetNodeById(long id) => this.Nodes.TryGetValue(id, out INode node) ? node : null;



		public void RemoveNode(long key) => GetNode(key)?.Dispose();

		public void RemoveNodeById(long id) => GetNodeById(id)?.Dispose();

		public void RemoveAllNode()
		{
			if (Nodes.Count == 0) return;
			//迭代器是没法一边迭代一边删除的，所以这里用了一个栈来存储需要删除的节点
			using (Self.PoolGet(out UnitStack<INode> nodes))
			{
				foreach (var item in Nodes) nodes.Push(item.Value);
				while (nodes.Count != 0) nodes.Pop().Dispose();
			}
			//假如在节点移除过程中，节点又添加了新的节点。那么就是错误的，新增节点将无法回收，父节点的分支键值将被占用。
			if (Nodes.Count != 0)
			{
				foreach (var item in Nodes)
				{
					World.LogError($"移除节点出错，意外的新节点，分支:{this.GetType()} 节点:{item.Value.GetType()}:{item.Value.Id}");
				}
			}
		}

		public void BranchRemoveNode(long nodeId) => Nodes.Remove(nodeId);


		public IEnumerator<INode> GetEnumerator() => Nodes.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Nodes.Values.GetEnumerator();

		public override void OnRecycle()
		{
			this.Nodes.Dispose();
			this.Self = null;
			this.Nodes = null;
			base.OnRecycle();
		}
	}

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
		public static void RemoveAllChild(this INode self) => self.RemoveAllNode<ChildBranch>();

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
