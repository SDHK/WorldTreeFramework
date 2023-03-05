/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 18:23

* 描述： 异步通用调用系统事件

*/

namespace WorldTree
{
    /// <summary>
    /// 异步通用调用系统事件
    /// </summary>
    public abstract class CallSystemAsync<E, OutT> : CallSystemAsyncBase<ICallSystemAsync<OutT>, E, OutT> where E : Node { }
    /// <summary>
    /// 异步通用调用系统事件
    /// </summary>
    public abstract class CallSystemAsync<E, T1, OutT> : CallSystemAsyncBase<ICallSystemAsync<T1, OutT>, E, T1, OutT> where E : Node { }
    /// <summary>
    /// 异步通用调用系统事件
    /// </summary>
    public abstract class CallSystemAsync<E, T1, T2, OutT> : CallSystemAsyncBase<ICallSystemAsync<T1, T2, OutT>, E, T1, T2, OutT> where E : Node { }
    /// <summary>
    /// 异步通用调用系统事件
    /// </summary>
    public abstract class CallSystemAsync<E, T1, T2, T3, OutT> : CallSystemAsyncBase<ICallSystemAsync<T1, T2, T3, OutT>, E, T1, T2, T3, OutT> where E : Node { }
    /// <summary>
    /// 异步通用调用系统事件
    /// </summary>
    public abstract class CallSystemAsync<E, T1, T2, T3, T4, OutT> : CallSystemAsyncBase<ICallSystemAsync<T1, T2, T3, T4, OutT>, E, T1, T2, T3, T4, OutT> where E : Node { }
    /// <summary>
    /// 异步通用调用系统事件
    /// </summary>
    public abstract class CallSystemAsync<E, T1, T2, T3, T4, T5, OutT> : CallSystemAsyncBase<ICallSystemAsync<T1, T2, T3, T4, T5, OutT>, E, T1, T2, T3, T4, T5, OutT> where E : Node { }
}
