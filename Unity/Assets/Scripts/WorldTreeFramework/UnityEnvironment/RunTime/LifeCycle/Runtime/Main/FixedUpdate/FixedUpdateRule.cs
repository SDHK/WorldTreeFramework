/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// FixedUpdate法则
	/// </summary>
	public interface FixedUpdate : ISendRule, ILifeCycleRule { }

	/// <summary>
	/// FixedUpdate法则
	/// </summary>
	public interface FixedUpdateTime : ISendRule<TimeSpan>, ILifeCycleRule { }
}
