
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/15 15:58

* 描述： 世界树节点监听器基类

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 世界树节点监听器基类
    /// </summary>
    public abstract partial class NodeListener : Node, INodeListener
    {
        #region Listener 

        public ListenerState listenerState { get; set; } = ListenerState.Not;

        public Type listenerTarget { get; set; }

        #endregion
    }
}
