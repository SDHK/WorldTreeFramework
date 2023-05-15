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
    public interface ITweenUpdateRule : ISendRule<float> { }

    /// <summary>
    /// 渐变法则
    /// </summary>
    public abstract class TweenUpdateRule<N> : SendRuleBase<N, ITweenUpdateRule, float> where N : class, INode, AsRule<ITweenUpdateRule> { }

}
