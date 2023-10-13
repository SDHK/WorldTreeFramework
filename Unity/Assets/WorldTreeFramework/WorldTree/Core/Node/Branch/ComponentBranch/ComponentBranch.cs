using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 组件分支
	/// </summary>
	public class ComponentBranch : UnitDictionary<long, INode>, IBranch
	{
		public INode Self { get; set; }
		public void RemoveNode(INode node)
		{
			this.Remove(node.Type);
		}
		public void RemoveAllNode()
		{
			if (this.Count == 0) return;
			using (Self.PoolGet(out UnitStack<INode> nodes))
			{
				foreach (var item in this) nodes.Push(item.Value);
				int length = nodes.Count;
				for (int i = 0; i < length; i++) nodes.Pop().Dispose();
			}
		}
		IEnumerator<INode> IEnumerable<INode>.GetEnumerator()
		{
			return this.Values.GetEnumerator();
		}


		//外部添加和新添加

		/// <summary>
		/// 尝试添加组件
		/// </summary>
		public bool TryAddNode<T>(out T Node, bool isPool = true)
			where T : class, INode
		{
			var type = TypeInfo<T>.HashCode64;
			if (this.TryGetValue(type, out INode node))
			{
				node = isPool ? Self.Core.GetNode(type) : Self.Core.NewNodeLifecycle(type);
				node.BranchType = TypeInfo<ComponentBranch>.HashCode64;
				node.Parent = Self;
				node.Domain = Self.Domain;
				this.Add(type, node);
			}
			Node = node as T;
			return Node != null;
		}

		/// <summary>
		/// 尝试添加外部组件
		/// </summary>
		public bool TryAddNode(long key, INode value)
		{
			if (this.TryAdd(key, value))
			{
				value.BranchType = TypeInfo<ComponentBranch>.HashCode64;
				value.Parent = Self;
				if (value.Domain != value) value.Domain = Self.Domain;
				//value.OnNodeAdd();//添加入核心
				return true;
			}
			return false;
		}







		public void RemoveNode(long key)
		{
			if (this.TryGetValue(key, out INode node))
			{
				node?.Dispose();
			}
		}
	}

	public static class NodeComponentBranchRule
	{
		/// <summary>
		/// 添加组件
		/// </summary>
		public static T AddComponentB<N, T>(this N self, out T Component)
			where N : class, INode
			where T : class, INode, BranchOf<N, ComponentBranch>, AsRule<IAwakeRule>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component))
			{
				Component.SendRule(DefaultType<IAwakeRule>.Default);
				Component.OnNodeAdd();
			}
			return Component;
		}
	}

}
