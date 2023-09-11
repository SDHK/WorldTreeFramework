/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 14:55

* 描述： 节点监听器
* 
* 用于节点动态监听全局任意节点事件
*/

namespace WorldTree
{
    public static class NodeListenerRule
    {
        /// <summary>
        /// 动态监听器切换目标
        /// </summary>
        public static bool ListenerSwitchesTarget<T>(this INodeListener self, ListenerState state)
        {
            if (self.listenerTarget != TypeInfo<T>.HashCode64)
            {
                //判断是否为监听器
                if (self.Core.RuleManager.DynamicListenerTypeHash.Contains(self.Type))
                {
                    self.Core.ReferencedPoolManager.RemoveDynamicListener(self);
                    self.listenerTarget = TypeInfo<T>.HashCode64;
                    self.listenerState = state;
                    self.Core.ReferencedPoolManager.TryAddDynamicListener(self);
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 动态监听器切换目标
        /// </summary>
        public static bool ListenerSwitchesTarget(this INodeListener self, long targetType, ListenerState state)
        {
            if (self.listenerTarget != targetType)
            {
                //判断是否为监听器
                if (self.Core.RuleManager.DynamicListenerTypeHash.Contains(self.Type))
                {
                    self.Core.ReferencedPoolManager.RemoveDynamicListener(self);
                    self.listenerTarget = targetType;
                    self.listenerState = state;
                    self.Core.ReferencedPoolManager.TryAddDynamicListener(self);
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 动态监听器切换节点目标
        /// </summary>
        public static bool ListenerSwitchesEntity<T>(this INodeListener self)
            where T : class, INode
        {
            return self.ListenerSwitchesTarget<T>(ListenerState.Node);
        }
        /// <summary>
        /// 动态监听器切换系统目标
        /// </summary>
        public static bool ListenerSwitchesRule<T>(this INodeListener self)
            where T : IRule
        {
            return self.ListenerSwitchesTarget<T>(ListenerState.Rule);
        }

        /// <summary>
        /// 动态监听器清除目标
        /// </summary>
        public static void ListenerClearTarget(this INodeListener self)
        {
            self.Core.ReferencedPoolManager.RemoveDynamicListener(self);
        }
    }
}
