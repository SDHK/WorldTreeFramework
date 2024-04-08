﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/24 17:31

* 描述： 活跃禁用法则
* 
* 同时会在移除节点前触发事件
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 活跃禁用法则接口
    /// </summary>
    public interface IDisableRule : ISendRuleBase { }

    /// <summary>
    /// 活跃禁用法则
    /// </summary>
    public abstract class DisableRule<N> : SendRuleBase<N, IDisableRule>
        where N : class, INode, AsRule<IDisableRule>
    {
        public override void Invoke(INode self)
        {
            if (self.IsActive != self.m_ActiveEventMark)
            {
                if (!self.IsActive)
                {
					self.m_ActiveEventMark = self.IsActive;
					Execute(self as N);
				}
			}
        }
    }
}