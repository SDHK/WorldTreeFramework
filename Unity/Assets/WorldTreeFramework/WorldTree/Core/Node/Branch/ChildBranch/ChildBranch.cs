/****************************************

* 作者： 闪电黑客
* 日期： 2023/11/11 04:04:48

* 描述： 子节点分支
* 
* 主要分支之一
* 设定为根据实例id为键进行存储。
* 

*/

namespace WorldTree
{
	/// <summary>
	/// 子分支
	/// </summary>
	public class ChildBranch : Branch<long> //内置与id对应的键值字典
	{
		public override bool TryAddNode<N>(long key, out N Node, bool isPool = true)
		{
			if (Nodes.TryGetValue(key, out INode node))
			{
				node = isPool ? Self.Core.GetNode(TypeInfo<N>.HashCode64) : Self.Core.NewNodeLifecycle(TypeInfo<N>.HashCode64);
				NodeKeys.Add(node.Id, key);
				node.BranchType = TypeInfo<ChildBranch>.HashCode64;
				node.Parent = Self;
				node.Core = Self.Core;
				node.Root = Self.Root;
				if (node.Domain != node) node.Domain = Self.Domain;
				node.SetActive(true);//激活节点
				Nodes.Add(key, node);
			}
			Node = node as N;
			return Node != null;
		}

		public override bool TryGraftNode(long key, INode node)
		{
			if (Nodes.TryAdd(key, node))
			{
				NodeKeys.Add(node.Id, key);
				node.BranchType = TypeInfo<ChildBranch>.HashCode64;
				node.Parent = Self;
				return true;
			}
			return false;
		}
	}
}
