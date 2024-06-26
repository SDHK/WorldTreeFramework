/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:58

* 描述： 树泛型HashSet

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 树泛型HashSet
	/// </summary>
	public partial class TreeHashSet<T> : HashSet<T>, INode, ChildOf<INode>
        , AsAwake
    {

	}

	public static class TreeHashSetRule
	{
		private class Remove<T> : RemoveRule<TreeHashSet<T>>
		{
			protected override void Execute(TreeHashSet<T> self)
			{
				self.Clear();
			}
		}
	}
}
