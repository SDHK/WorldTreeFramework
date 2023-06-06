/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/5 16:01

* 描述： 树任务控制器

*/

namespace WorldTree
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// 任务运行中
        /// </summary>
        Running,
        /// <summary>
        /// 任务暂停
        /// </summary>
        Stop,
        /// <summary>
        /// 任务取消
        /// </summary>
        Cancel
    }

    /// <summary>
    /// 树任务控制器
    /// </summary>
    public class TreeTaskController : Node, ChildOf<INode>, ComponentOf<INode>
    {
        public TreeTaskBase treeTaskBase;

        /// <summary>
        /// 任务状态
        /// </summary>
        public TaskState taskState;

    }
}
