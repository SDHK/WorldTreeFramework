/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

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
			self.AddTemp(out TreeTask<AsyncOperation> asyncTask);
			asyncOperation.completed += asyncTask.SetResult;
			return asyncTask;
		}


	}
}
