
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/23 17:17

* 描述： 对类回调的异步扩展

*/

using WorldTree.Internal;

namespace WorldTree
{
    /// <summary>
    /// 对类回调的异步扩展
    /// </summary>
    public static class AsyncTaskExtension
    {
        /// <summary>
        /// 延迟一帧
        /// </summary>
        public static AsyncTaskCompleted AsyncTaskCompleted(this Entity self)
        {
            return self.AddChildren<AsyncTaskCompleted>();

        }

        public static AsyncTask AsyncTaskQueue(this Entity self)
        {

            return self.AddChildren<AsyncTask>();
        }
    }
}
