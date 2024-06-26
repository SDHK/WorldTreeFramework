/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/15 15:58

* 描述： 世界树节点监听器基类

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 世界树动态节点监听器基类
	/// </summary>
	public abstract partial class DynamicNodeListener : Node, INodeListener
	{
		#region Listener

		/// <summary>
		/// 监听状态
		/// </summary>
		public ListenerState listenerState { get; set; } = ListenerState.Not;

		/// <summary>
		/// 监听目标
		/// </summary>
		public long listenerTarget { get; set; }

		#endregion
	}
}