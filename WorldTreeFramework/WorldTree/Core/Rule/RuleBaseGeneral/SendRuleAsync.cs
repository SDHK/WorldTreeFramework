
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 18:26

* 描述： 异步通用通知法则

*/


namespace WorldTree
{
    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N> : SendRuleAsyncBase<ISendRuleAsync, N> where N : class,INode { }
    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N, T1> : SendRuleAsyncBase<ISendRuleAsync<T1>, N, T1> where N : class,INode { }
    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N, T1, T2> : SendRuleAsyncBase<ISendRuleAsync<T1, T2>, N, T1, T2> where N : class,INode { }
    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N, T1, T2, T3> : SendRuleAsyncBase<ISendRuleAsync<T1, T2, T3>, N, T1, T2, T3> where N : class,INode { }
    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N, T1, T2, T3, T4> : SendRuleAsyncBase<ISendRuleAsync<T1, T2, T3, T4>, N, T1, T2, T3, T4> where N : class,INode { }
    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N, T1, T2, T3, T4, T5> : SendRuleAsyncBase<ISendRuleAsync<T1, T2, T3, T4, T5>, N, T1, T2, T3, T4, T5> where N : class,INode { }
}
