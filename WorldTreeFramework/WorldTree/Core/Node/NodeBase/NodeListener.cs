
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/31 19:08

* 描述： 节点监听器
* 
* 用于节点动态监听全局任意节点事件

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

    public abstract partial class Node
    {
        /// <summary>
        /// 动态监听器状态
        /// </summary>
        public ListenerState listenerState = ListenerState.Not;

        /// <summary>
        /// 动态监听目标类型
        /// </summary>
        public Type listenerTarget = null;
    }
}
