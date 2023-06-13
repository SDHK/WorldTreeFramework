
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 异步任务完成类
* 
* 因为没有使用单例，所以必须让构建器执行6号步骤创建Task。
* 
* 因此没法进行跳过处理，只能通过 OnEvent 调用，延迟到下一帧执行，无解。

*/

using System.Runtime.CompilerServices;

namespace WorldTree.Internal
{
    /// <summary>
    /// 异步任务完成类
    /// </summary>
    [AsyncMethodBuilder(typeof(TreeTaskCompletedMethodBuilder))]
    public class TreeTaskCompleted : TreeTaskBase, ChildOf<INode>
    {
        public TreeTaskCompleted GetAwaiter() => this;
        public override bool IsCompleted { get; set; }
        public void GetResult() { }
    }

    class TreeTaskCompletedUpdateRule : UpdateRule<TreeTaskCompleted>
    {
        public override void OnEvent(TreeTaskCompleted self, float deltaTime)
        {
            World.Log($"Completed [{self.Id}] 结束");

            self.SetCompleted();
        }
    }
}
