/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/5 16:01

* 描述： 树任务控制器

*/

namespace WorldTree
{
    /// <summary>
    /// 树任务控制器
    /// </summary>
    public class TreeTaskController : Node, ChildOf<INode>, ComponentOf<INode>
    {
        public TreeTaskBase treeTaskBase;


    }
}
