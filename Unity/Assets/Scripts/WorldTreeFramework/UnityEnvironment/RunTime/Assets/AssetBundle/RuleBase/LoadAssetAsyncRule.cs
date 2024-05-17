
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 10:35

* 描述： 

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 异步加载资源法则
	/// </summary>
	public interface LoadAssetAsync : ICallRuleAsync<string, Object>, IMethodRule { }
}
