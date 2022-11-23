
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/23 17:17

* 描述： 对类回调的异步扩展

*/

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace WorldTree
{
    public static class AsyncTaskExtension
    {
        public static AsyncTask<AsyncOperation> GetAwaiter(this Entity self, AsyncOperation asyncOperation)
        {
            AsyncTask<AsyncOperation> asyncTask = self.AddChildren<AsyncTask<AsyncOperation>>();
            asyncOperation.completed += asyncTask.SetResult;
            return asyncTask;
        }

        public static AsyncTask<AsyncOperationHandle<T>> GetAwaiter<T>(this Entity self, AsyncOperationHandle<T> handle)
        {
            AsyncTask<AsyncOperationHandle<T>> asyncTask = self.AddChildren<AsyncTask<AsyncOperationHandle<T>>>();
            handle.Completed += asyncTask.SetResult;
            return asyncTask;
        }
    }
}
