
/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/13 19:30

* 描述： 树任务令牌捕获

*/

using System.Runtime.CompilerServices;
using WorldTree.Internal;

namespace WorldTree
{
    /// <summary>
    /// 树任务令牌捕获
    /// </summary>
    public class TreeTaskTokenCatch : TreeTaskBase
		, ChildOf<INode>
		, AsAwake
        , ISyncTask
    {
        public TreeTaskTokenCatch GetAwaiter() => this;
        public override bool IsCompleted { get; set; }
        public TreeTaskToken GetResult() { return m_TreeTaskToken; }
    }
}
