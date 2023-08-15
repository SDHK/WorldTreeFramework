
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/30 20:35

* 描述： 全局法则执行器基类
* 
* 只有全局执行器需要动态全局监听

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 全局法则执行器基类
    /// </summary>
    public abstract class GlobalRuleActuatorBase : RuleActuatorBase, INodeListener, IRuleActuator
    {
        public ListenerState listenerState { get; set; }
        public long listenerTarget { get; set; }

    }

    public static class GlobalRuleActuatorBaseRule
    {


        class ListenerAddRule : ListenerAddRule<GlobalRuleActuatorBase>
        {
            protected override void OnEvent(GlobalRuleActuatorBase self, INode node)
            {
                self.TryAdd(node);
            }
        }

        class ListenerRemoveRule : ListenerRemoveRule<GlobalRuleActuatorBase>
        {
            protected override void OnEvent(GlobalRuleActuatorBase self, INode node)
            {
                self.Remove(node);
            }
        }
    }

}
