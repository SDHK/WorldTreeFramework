
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/15 15:57

* 描述： 节点监听器最底层接口
* 
* 

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 监听器状态
    /// </summary>
    public enum ListenerState
    {
        /// <summary>
        /// 不是监听器
        /// </summary>
        Not,
        /// <summary>
        /// 监听目标是节点
        /// </summary>
        Node,
        /// <summary>
        /// 监听目标是法则
        /// </summary>
        Rule
    }

	/// <summary>
	/// 监听器忽略标记
	/// </summary>
	/// <remarks>避免核心启动时监听处理出现死循环</remarks>
	public interface IListenerIgnorer { }

	/// <summary>
	/// 节点监听器接口
	/// </summary>
	public interface INodeListener : INode
	    , AsListenerAddRule
		, AsListenerRemoveRule
	{ }

	/// <summary>
	/// 动态监听器接口
	/// </summary>
	public interface IDynamicNodeListener : INodeListener
    {
        #region Listener
        /// <summary>
        /// 动态监听器状态
        /// </summary>
        public ListenerState ListenerState { get; set; }

        /// <summary>
        /// 动态监听目标类型
        /// </summary>
        public long ListenerTarget { get; set; }
        #endregion
    }
}
