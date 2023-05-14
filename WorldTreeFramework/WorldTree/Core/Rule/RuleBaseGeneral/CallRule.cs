/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:10

* 描述： 通用调用法则执行

*/

namespace WorldTree
{
    /// <summary>
    /// 通用调用法则执行
    /// </summary>
    public abstract class CallRule<N, OutT> : CallRuleBase<N, ICallRule<OutT>, OutT> where N : class, INode, AsRule<ICallRule<OutT>> { }
    /// <summary>
    /// 通用调用法则执行
    /// </summary>
    public abstract class CallRule<N, T1, OutT> : CallRuleBase<N, ICallRule<T1, OutT>, T1, OutT> where N : class, INode, AsRule<ICallRule<T1, OutT>> { }
    /// <summary>
    /// 通用调用法则执行
    /// </summary>
    public abstract class CallRule<N, T1, T2, OutT> : CallRuleBase<N, ICallRule<T1, T2, OutT>, T1, T2, OutT> where N : class, INode, AsRule<ICallRule<T1, T2, OutT>> { }
    /// <summary>
    /// 通用调用法则执行
    /// </summary>
    public abstract class CallRule<N, T1, T2, T3, OutT> : CallRuleBase<N, ICallRule<T1, T2, T3, OutT>, T1, T2, T3, OutT> where N : class, INode, AsRule<ICallRule<T1, T2, T3, OutT>> { }
    /// <summary>
    /// 通用调用法则执行
    /// </summary>
    public abstract class CallRule<N, T1, T2, T3, T4, OutT> : CallRuleBase<N, ICallRule<T1, T2, T3, T4, OutT>, T1, T2, T3, T4, OutT> where N : class, INode, AsRule<ICallRule<T1, T2, T3, T4, OutT>> { }
    /// <summary>
    /// 通用调用法则执行
    /// </summary>
    public abstract class CallRule<N, T1, T2, T3, T4, T5, OutT> : CallRuleBase<N, ICallRule<T1, T2, T3, T4, T5, OutT>, T1, T2, T3, T4, T5, OutT> where N : class, INode, AsRule<ICallRule<T1, T2, T3, T4, T5, OutT>> { }


}
