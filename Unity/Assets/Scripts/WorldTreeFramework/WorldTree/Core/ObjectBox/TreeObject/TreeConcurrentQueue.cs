/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/28 03:28:38

* 描述： 树泛型队列

*/

using System.Collections.Concurrent;

namespace WorldTree
{
	/// <summary>
	/// 树并发队列
	/// </summary>
	public partial class TreeConcurrentQueue<T> : ConcurrentQueue<T>, INodeData, INode
		, ChildOf<INode>, ComponentOf<INode>
		, AsAwake
	{
		public long UID { get; set; }
	}

	public static class TreeConcurrentQueueRule
	{
		private class Remove<T> : RemoveRule<TreeConcurrentQueue<T>>
		{
			protected override void Execute(TreeConcurrentQueue<T> self)
			{
				self.Clear();
			}
		}
	}
}
