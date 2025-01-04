/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

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
