
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/24 17:30

* 描述： 活跃启用法则
* 
* 会在加入节点后触发事件

*/

namespace WorldTree
{
    /// <summary>
    /// 活跃启用法则接口
    /// </summary>
    public interface IEnableRule : ISendRule { }

    /// <summary>
    /// 活跃启用法则
    /// </summary>
    public abstract class EnableRule<N> : SendRuleBase<IEnableRule, N>
    where N : class,INode
    {
        public override void Invoke(INode self)
        {
            if (self.IsActive != self.m_ActiveEventMark)
            {
                if (self.IsActive)
                {
                    OnEvent(self as N);
                }
                self.m_ActiveEventMark = self.IsActive;
            }
        }
    }
}
