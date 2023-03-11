
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
        public static TreeTask<AsyncOperation> GetAwaiter(this INode self, AsyncOperation asyncOperation)
        {
            TreeTask<AsyncOperation> asyncTask = self.AddChildren<TreeTask<AsyncOperation>>();
            asyncOperation.completed += asyncTask.SetResult;
            return asyncTask;
        }

        /// <summary>
        /// 获取异步等待
        /// </summary>
        public static TreeTask<AsyncOperationHandle<T>> GetAwaiter<T>(this INode self, AsyncOperationHandle<T> handle)
        {
            TreeTask<AsyncOperationHandle<T>> asyncTask = self.AddChildren<TreeTask<AsyncOperationHandle<T>>>();
            handle.Completed += asyncTask.SetResult;
            return asyncTask;
        }
    }
}
