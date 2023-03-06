﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/23 18:17

* 描述：全局节点添加事件动态监听器
* 
* 用于监听全局节点添加

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 全局节点添加事件动态监听器
    /// </summary>
    public class GlobalNodeAddListener : Node
    {
        public DynamicNodeQueue nodeQueue;
    }
    class GlobalNodeAddListenerAddSystem : AddRule<GlobalNodeAddListener>
    {
        public override void OnEvent(GlobalNodeAddListener self)
        {
            self.TryParentTo(out self.nodeQueue);
        }
    }
    class GlobalNodeAddListenerRemoveSystem : RemoveRule<GlobalNodeAddListener>
    {
        public override void OnEvent(GlobalNodeAddListener self)
        {
            self.nodeQueue = null;
        }
    }
    class GlobalNodeAddListenerListenerAddSystem : ListenerAddRule<GlobalNodeAddListener>
    {
        public override void OnEvent(GlobalNodeAddListener self, Node node)
        {
            self.nodeQueue?.Enqueue(node);
        }
    }
}
