
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 18:26

* 描述： 异步通用发送系统事件

*/


namespace WorldTree 
{
    /// <summary>
    /// 异步通用发送系统事件
    /// </summary>
    public abstract class SendSystemAsync<E> : SendRuleAsyncBase<ISendRuleAsync, E> where E : Node { }
    /// <summary>
    /// 异步通用发送系统事件
    /// </summary>
    public abstract class SendSystemAsync<E, T1> : SendRuleAsyncBase<ISendRuleAsync<T1>, E, T1> where E : Node { }
    /// <summary>
    /// 异步通用发送系统事件
    /// </summary>
    public abstract class SendSystemAsync<E, T1, T2> : SendRuleAsyncBase<ISendRuleAsync<T1, T2>, E, T1, T2> where E : Node { }
    /// <summary>
    /// 异步通用发送系统事件
    /// </summary>
    public abstract class SendSystemAsync<E, T1, T2, T3> : SendRuleAsyncBase<ISendRuleAsync<T1, T2, T3>, E, T1, T2, T3> where E : Node { }
    /// <summary>
    /// 异步通用发送系统事件
    /// </summary>
    public abstract class SendSystemAsync<E, T1, T2, T3, T4> : SendRuleAsyncBase<ISendRuleAsync<T1, T2, T3, T4>, E, T1, T2, T3, T4> where E : Node { }
    /// <summary>
    /// 异步通用发送系统事件
    /// </summary>
    public abstract class SendSystemAsync<E, T1, T2, T3, T4, T5> : SendRuleAsyncBase<ISendRuleAsync<T1, T2, T3, T4, T5>, E, T1, T2, T3, T4, T5> where E : Node { }
}
