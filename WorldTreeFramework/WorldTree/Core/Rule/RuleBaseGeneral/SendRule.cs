
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:04

* 描述： 通用通知法则

*/

namespace WorldTree
{
    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N> : SendRuleBase<N, ISendRule> where N : class, INode, AsRule<ISendRule> { }
    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N, T1> : SendRuleBase<N, ISendRule<T1>, T1> where N : class, INode, AsRule<ISendRule<T1>> { }
    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N, T1, T2> : SendRuleBase<N, ISendRule<T1, T2>, T1, T2> where N : class, INode, AsRule<ISendRule<T1, T2>> { }
    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N, T1, T2, T3> : SendRuleBase<N, ISendRule<T1, T2, T3>, T1, T2, T3> where N : class, INode, AsRule<ISendRule<T1, T2, T3>> { }
    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N, T1, T2, T3, T4> : SendRuleBase<N, ISendRule<T1, T2, T3, T4>, T1, T2, T3, T4> where N : class, INode, AsRule<ISendRule<T1, T2, T3, T4>> { }
    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N, T1, T2, T3, T4, T5> : SendRuleBase<N, ISendRule<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5> where N : class, INode, AsRule<ISendRule<T1, T2, T3, T4, T5>> { }

}
