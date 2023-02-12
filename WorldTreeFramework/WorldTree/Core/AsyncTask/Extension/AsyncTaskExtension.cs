
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/23 17:17

* 描述： 对类回调的异步扩展

*/

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using WorldTree.Internal;

namespace WorldTree
{
    /// <summary>
    /// 对类回调的异步扩展
    /// </summary>
    public static class AsyncTaskExtension
    {
        /// <summary>
        /// 获取异步等待
        /// </summary>
        public static AsyncTask<AsyncOperation> GetAwaiter(this Entity self, AsyncOperation asyncOperation)
        {
            AsyncTask<AsyncOperation> asyncTask = self.AddChildren<AsyncTask<AsyncOperation>>();
            asyncOperation.completed += asyncTask.SetResult;
            return asyncTask;
        }

        /// <summary>
        /// 获取异步等待
        /// </summary>
        public static AsyncTask<AsyncOperationHandle<T>> GetAwaiter<T>(this Entity self, AsyncOperationHandle<T> handle)
        {
            AsyncTask<AsyncOperationHandle<T>> asyncTask = self.AddChildren<AsyncTask<AsyncOperationHandle<T>>>();
            handle.Completed += asyncTask.SetResult;
            return asyncTask;
        }

        /// <summary>
        /// 延迟一帧
        /// </summary>
        public static AsyncTaskCompleted AsyncTaskCompleted(this Entity self)
        {
            return self.AddChildren<AsyncTaskCompleted>();

        }
    }
}
