using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 子分支
	/// </summary>
	public class ChildBranch : UnitDictionary<long, INode>, IBranch
	{
		public INode Self { get; set; }

		public void RemoveNode(INode node)
		{
			this.Remove(node.Id);
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
		/// <summary>
		/// 节点枚举器
		/// </summary>
		IEnumerator<INode> IEnumerable<INode>.GetEnumerator()
		{
			return this.Values.GetEnumerator();
		}
	}
}
