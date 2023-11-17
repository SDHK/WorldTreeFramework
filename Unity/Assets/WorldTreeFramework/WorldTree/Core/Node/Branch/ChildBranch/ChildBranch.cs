/****************************************

* 作者： 闪电黑客
* 日期： 2023/11/11 04:04:48

* 描述： 子节点分支
* 
* 主要分支之一
* 设定根据实例id为键挂载。
* 

*/

namespace WorldTree
{
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
		public static void CutChild(this INode self, long id)
		{
			if (self.TryGetBranch(out ChildBranch branch))
			{
				if (branch.TryGetNode(id, out INode node))
				{
					node.TreeCutSelf();
				}
			}
		}

		#endregion

		#region 嫁接

		/// <summary>
		/// 嫁接子节点
		/// </summary>
		public static void GraftChild<N, T>(this INode self, T node)
			where N : class, INode, AsNode<ChildBranch, T>
			where T : class, INode
		{
			self.TreeGraftNode<ChildBranch, long>(node.Id, node);
		}

		/// <summary>
		/// 设定id嫁接子节点
		/// </summary>
		public static void GraftChild<N, T>(this INode self, long id, T node)
			where N : class, INode, AsNode<ChildBranch, T>
			where T : class, INode
		{
			self.TreeGraftNode<ChildBranch, long>(id, node);
		}

		#endregion

		#region 移除

		/// <summary>
		/// 根据键值移除子节点
		/// </summary>
		public static void RemoveChild(this INode self, long id)
		{
			if (self.TryGetBranch(out ChildBranch branch))
			{
				branch.RemoveNode(id);
			}
		}

		/// <summary>
		/// 移除全部子节点
		/// </summary>
		public static void RemoveAllChild(this INode self)
		{
			if (self.TryGetBranch(out ChildBranch branch)) branch.RemoveAllNode();
		}

		#endregion

		#region 添加

		#region 池



		#endregion

		#region 非池

		#endregion

		#endregion
	}

}
