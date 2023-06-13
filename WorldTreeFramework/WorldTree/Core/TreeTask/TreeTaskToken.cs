/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/5 16:01

* 描述： 树任务令牌

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
    /// 树任务令牌
    /// </summary>
    public class TreeTaskToken : Node, ChildOf<INode>, ComponentOf<INode>
    {
        /// <summary>
        /// 任务状态
        /// </summary>
        private TaskState taskState;

        /// <summary>
        /// 暂停的任务
        /// </summary>
        public TreeTaskBase stopTask;

        /// <summary>
        /// 任务的取消委托
        /// </summary>
        public RuleActuator<ISendRuleBase> cancels;


        /// <summary>
        /// 任务状态
        /// </summary>
        public TaskState State { get => taskState; }

        public override string ToString()
        {
            return $"TreeTaskController({stopTask?.Id})";
        }

        /// <summary>
        /// 继续执行
        /// </summary>
        public void Continue()
        {
            taskState = TaskState.Running;
            stopTask?.Continue();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Stop()
        {
            taskState = TaskState.Stop;
        }

        /// <summary>
        /// 取消
        /// </summary>
        public void Cancel()
        {
            taskState = TaskState.Cancel;
            cancels?.Send();
        }

    }
}
