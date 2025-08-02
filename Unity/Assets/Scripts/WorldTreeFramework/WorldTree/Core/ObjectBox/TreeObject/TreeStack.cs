
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:55

* 描述： 树泛型栈

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 实体泛型栈
	/// </summary>
	[INodeProxy]
	public partial class TreeStack<T> : Stack<T>, INode, ChildOf<INode>
		, AsRule<Awake>
	{
	}

	public static class TreeStackRule
	{
		private class Remove<T> : RemoveRule<TreeStack<T>>
		{
			protected override void Execute(TreeStack<T> self)
			{
				self.Clear();
			}
		}

	}
}
