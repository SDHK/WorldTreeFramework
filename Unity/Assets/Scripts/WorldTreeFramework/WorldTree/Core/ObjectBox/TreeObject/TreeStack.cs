
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:55

* 描述： 树泛型栈

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 实体泛型栈
    /// </summary>
    public partial class TreeStack<T> : Stack<T>, INodeData, INode, ChildOf<INode>
        , AsAwake
    {
		public long UID { get; set; }
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
