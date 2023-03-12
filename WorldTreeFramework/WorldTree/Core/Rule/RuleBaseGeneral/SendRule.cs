﻿
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
    public abstract class SendRule<N> : SendRuleBase<ISendRule, N> where N : class,INode { }
    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N, T1> : SendRuleBase<ISendRule<T1>, N, T1> where N : class,INode { }
    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N, T1, T2> : SendRuleBase<ISendRule<T1, T2>, N, T1, T2> where N : class,INode { }
    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N, T1, T2, T3> : SendRuleBase<ISendRule<T1, T2, T3>, N, T1, T2, T3> where N : class,INode { }
    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N, T1, T2, T3, T4> : SendRuleBase<ISendRule<T1, T2, T3, T4>, N, T1, T2, T3, T4> where N : class,INode { }
    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N, T1, T2, T3, T4, T5> : SendRuleBase<ISendRule<T1, T2, T3, T4, T5>, N, T1, T2, T3, T4, T5> where N : class,INode { }

}