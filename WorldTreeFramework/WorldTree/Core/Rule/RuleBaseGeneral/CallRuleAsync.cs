/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 18:23

* 描述： 异步通用调用法则执行

*/

namespace WorldTree
{
    /// <summary>
    /// 异步通用调用法则执行
    /// </summary>
    public abstract class CallRuleAsync<N, OutT> : CallRuleAsyncBase<ICallRuleAsync<OutT>, N, OutT> where N : Node { }
    /// <summary>
    /// 异步通用调用法则执行
    /// </summary>
    public abstract class CallRuleAsync<N, T1, OutT> : CallRuleAsyncBase<ICallRuleAsync<T1, OutT>, N, T1, OutT> where N : Node { }
    /// <summary>
    /// 异步通用调用法则执行
    /// </summary>
    public abstract class CallRuleAsync<N, T1, T2, OutT> : CallRuleAsyncBase<ICallRuleAsync<T1, T2, OutT>, N, T1, T2, OutT> where N : Node { }
    /// <summary>
    /// 异步通用调用法则执行
    /// </summary>
    public abstract class CallRuleAsync<N, T1, T2, T3, OutT> : CallRuleAsyncBase<ICallRuleAsync<T1, T2, T3, OutT>, N, T1, T2, T3, OutT> where N : Node { }
    /// <summary>
    /// 异步通用调用法则执行
    /// </summary>
    public abstract class CallRuleAsync<N, T1, T2, T3, T4, OutT> : CallRuleAsyncBase<ICallRuleAsync<T1, T2, T3, T4, OutT>, N, T1, T2, T3, T4, OutT> where N : Node { }
    /// <summary>
    /// 异步通用调用法则执行
    /// </summary>
    public abstract class CallRuleAsync<N, T1, T2, T3, T4, T5, OutT> : CallRuleAsyncBase<ICallRuleAsync<T1, T2, T3, T4, T5, OutT>, N, T1, T2, T3, T4, T5, OutT> where N : Node { }
}
