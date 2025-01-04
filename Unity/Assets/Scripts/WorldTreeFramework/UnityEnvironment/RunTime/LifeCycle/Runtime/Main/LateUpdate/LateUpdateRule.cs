/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// LateUpdate法则
	/// </summary>
	public interface LateUpdate : ISendRule, ILifeCycleRule { }

	/// <summary>
	/// LateUpdate法则
	/// </summary>
	public interface LateUpdateTime : ISendRule<TimeSpan>, ILifeCycleRule { }

}
