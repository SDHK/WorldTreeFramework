
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/23 17:17

* 描述： 节点回调的异步扩展

*/

using WorldTree.Internal;

namespace WorldTree
{
    /// <summary>
    /// 对类回调的异步扩展
    /// </summary>
    public static class TreeTaskExtension
    {
        /// <summary>
        /// 同步完成
        /// </summary>
        public static TreeTaskCompleted TreeTaskCompleted(this INode self)
        {
            return self.AddChild(out TreeTaskCompleted _);
        }

        /// <summary>
        /// 同步捕获令牌
        /// </summary>
        public static TreeTaskTokenCatch TreeTaskTokenCatch(this INode self)
        {
            return self.AddChild(out TreeTaskTokenCatch _);
        }
    }
}
