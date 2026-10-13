/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:51

* 描述： 世界环境

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 世界环境的工作请求
	/// </summary>
	public struct WorldContextRequest
	{
		/// <summary>
		/// 委托回调
		/// </summary>
		private readonly Action<WorldContextData> delagateCallback;

		/// <summary>
		/// 执行数据
		/// </summary>
		private readonly WorldContextData delagateState;

		public WorldContextRequest(Action<WorldContextData> callback, WorldContextData state)
		{
			delagateCallback = callback;
			delagateState = state;
		}
		/// <summary>
		/// 执行委托
		/// </summary>
		public void Invoke() => delagateCallback.Invoke(delagateState);
	}
}
