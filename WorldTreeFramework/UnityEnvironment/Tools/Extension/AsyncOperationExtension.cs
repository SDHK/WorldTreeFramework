
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/17 17:13

* 描述： 异步操作类的扩展

*/

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace WorldTree
{
    /// <summary>
    /// 异步操作类的扩展
    /// </summary>
    public static class AsyncOperationExtension
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
    }
}
