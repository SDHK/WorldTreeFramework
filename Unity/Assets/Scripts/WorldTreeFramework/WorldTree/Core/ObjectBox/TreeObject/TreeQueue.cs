/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:51

* 描述： 树泛型队列

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 树泛型队列
	/// </summary>
	[INodeProxy]
	public partial class TreeQueue<T> : Queue<T>, INode, ChildOf<INode>, ComponentOf<INode>
		, AsAwake
	{
	}

	public static class TreeQueueRule
	{
		private class Remove<T> : RemoveRule<TreeQueue<T>>
		{
			protected override void Execute(TreeQueue<T> self)
			{
				self.Clear();
			}
		}
	}



}
