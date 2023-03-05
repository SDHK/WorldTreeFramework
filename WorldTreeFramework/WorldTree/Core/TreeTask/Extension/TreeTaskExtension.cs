﻿
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
        /// 延迟一帧
        /// </summary>
        public static TreeTaskCompleted AsyncTaskCompleted(this Node self)
        {
            return self.AddChildren<TreeTaskCompleted>();

        }

        public static TreeTask AsyncTaskQueue(this Node self)
        {

            return self.AddChildren<TreeTask>();
        }
    }
}