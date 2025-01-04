/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// DrawGizmos法则
	/// </summary>
	public interface DrawGizmosUpdateTime : ISendRule<TimeSpan>, ILifeCycleRule { }

	/// <summary>
	/// DrawGizmos法则
	/// </summary>
	public interface DrawGizmosUpdate : ISendRule, ILifeCycleRule { }
}
