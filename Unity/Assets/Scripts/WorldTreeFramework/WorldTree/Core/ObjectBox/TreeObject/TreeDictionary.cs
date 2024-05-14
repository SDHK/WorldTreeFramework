/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/10 12:00

* 描述： 树字典

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 树字典泛型类
	/// </summary>
	public partial class TreeDictionary<Key, V> : Dictionary<Key, V>, INode, ComponentOf<INode>, ChildOf<INode>
		, AsAwake
	{
	
	}

	public static class TreeDictionaryRule
	{
		private class Remove<Key, V> : RemoveRule<TreeDictionary<Key, V>>
		{
			protected override void Execute(TreeDictionary<Key, V> self)
			{
				self.Clear();
			}
		}
	}
}