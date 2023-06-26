
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 18:26

* 描述： 异步通用通知法则

*/


namespace WorldTree
{
    /// <summary>
    /// 异步通用通知法则接口
    /// </summary>
    public interface ISendRuleAsync : ISendRuleAsyncBase { }
    /// <summary>
    /// 异步通用通知法则接口
    /// </summary>
    public interface ISendRuleAsync<T1> : ISendRuleAsyncBase<T1> { }
    /// <summary>
    /// 异步通用通知法则接口
    /// </summary>
    public interface ISendRuleAsync<T1, T2> : ISendRuleAsyncBase<T1, T2> { }
    /// <summary>
    /// 异步通用通知法则接口
    /// </summary>
    public interface ISendRuleAsync<T1, T2, T3> : ISendRuleAsyncBase<T1, T2, T3> { }
    /// <summary>
    /// 异步通用通知法则接口
    /// </summary>
    public interface ISendRuleAsync<T1, T2, T3, T4> : ISendRuleAsyncBase<T1, T2, T3, T4> { }
    /// <summary>
    /// 异步通用通知法则接口
    /// </summary>
    public interface ISendRuleAsync<T1, T2, T3, T4, T5> : ISendRuleAsyncBase<T1, T2, T3, T4, T5> { }



    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N> : SendRuleAsyncBase<N, ISendRuleAsync> where N : class, INode, AsRule<ISendRuleAsync> { }
    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N, T1> : SendRuleAsyncBase<N, ISendRuleAsync<T1>, T1> where N : class, INode, AsRule<ISendRuleAsync<T1>> { }
    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N, T1, T2> : SendRuleAsyncBase<N, ISendRuleAsync<T1, T2>, T1, T2> where N : class, INode, AsRule<ISendRuleAsync<T1, T2>> { }
    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N, T1, T2, T3> : SendRuleAsyncBase<N, ISendRuleAsync<T1, T2, T3>, T1, T2, T3> where N : class, INode, AsRule<ISendRuleAsync<T1, T2, T3>> { }
    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N, T1, T2, T3, T4> : SendRuleAsyncBase<N, ISendRuleAsync<T1, T2, T3, T4>, T1, T2, T3, T4> where N : class, INode, AsRule<ISendRuleAsync<T1, T2, T3, T4>> { }
    /// <summary>
    /// 异步通用通知法则
    /// </summary>
    public abstract class SendRuleAsync<N, T1, T2, T3, T4, T5> : SendRuleAsyncBase<N, ISendRuleAsync<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5> where N : class, INode, AsRule<ISendRuleAsync<T1, T2, T3, T4, T5>> { }
}
