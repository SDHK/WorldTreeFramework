/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/10 19:36

* 描述： 

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 渐变法则接口
    /// </summary>
    public interface ITweenUpdateRule : ISendRuleBase<TimeSpan> { }

    /// <summary>
    /// 渐变法则
    /// </summary>
    public abstract class TweenUpdateRule<N> : SendRuleBase<N, ITweenUpdateRule, TimeSpan> where N : class, INode, AsRule<ITweenUpdateRule> { }

}
