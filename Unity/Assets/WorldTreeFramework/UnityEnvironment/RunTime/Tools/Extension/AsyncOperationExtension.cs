
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/17 17:13

* 描述： 异步操作类的扩展

*/

using UnityEngine;
//using UnityEngine.ResourceManagement.AsyncOperations;
using YooAsset;

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
			self.AddChild(out TreeTask<AsyncOperation> asyncTask);
			asyncOperation.completed += asyncTask.SetResult;
			return asyncTask;
		}

		/// <summary>
		/// 获取异步等待
		/// </summary>
		public static TreeTask<T> GetAwaiter<T>(this INode self, T handle)
			where T : AsyncOperationBase
		{
			self.AddChild(out TreeTask<T> asyncTask);
			handle.Completed += (a) => asyncTask.SetResult(a as T);
			return asyncTask;
		}
	}
}
