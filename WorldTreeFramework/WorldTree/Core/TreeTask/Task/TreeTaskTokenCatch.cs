
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
    [AsyncMethodBuilder(typeof(TreeTaskTokenCatchMethodBuilder))]
    public class TreeTaskTokenCatch : TreeTaskBase
    {
        public TreeTaskTokenCatch GetAwaiter() => this;
        public override bool IsCompleted { get; set; }
        public TreeTaskToken GetResult() { return m_TreeTaskToken; }
    }

    class TreeTaskTokenCatchUpdateRule : UpdateRule<TreeTaskTokenCatch>
    {
        public override void OnEvent(TreeTaskTokenCatch self, float deltaTime)
        {
            World.Log($"[{self.Id}]TreeTaskTokenCatch 完成");
            self.SetCompleted();
        }
    }
}
