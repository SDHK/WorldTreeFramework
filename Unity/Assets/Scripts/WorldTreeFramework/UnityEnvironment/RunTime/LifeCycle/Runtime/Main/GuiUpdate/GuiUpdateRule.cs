/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// OnGUI法则
	/// </summary>
	public interface GuiUpdateTime : ISendRule<TimeSpan>, ILifeCycleRule { }

	/// <summary>
	/// OnGUI法则
	/// </summary>
	public interface GuiUpdate : ISendRule, ILifeCycleRule { }
}
