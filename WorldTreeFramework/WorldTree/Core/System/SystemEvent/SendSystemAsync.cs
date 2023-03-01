
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
    public abstract class SendSystemAsync<E> : SendSystemAsyncBase<ISendSystemAsync, E> where E : Entity { }
    /// <summary>
    /// 异步通用发送系统事件
    /// </summary>
    public abstract class SendSystemAsync<E, T1> : SendSystemAsyncBase<ISendSystemAsync<T1>, E, T1> where E : Entity { }
    /// <summary>
    /// 异步通用发送系统事件
    /// </summary>
    public abstract class SendSystemAsync<E, T1, T2> : SendSystemAsyncBase<ISendSystemAsync<T1, T2>, E, T1, T2> where E : Entity { }
    /// <summary>
    /// 异步通用发送系统事件
    /// </summary>
    public abstract class SendSystemAsync<E, T1, T2, T3> : SendSystemAsyncBase<ISendSystemAsync<T1, T2, T3>, E, T1, T2, T3> where E : Entity { }
    /// <summary>
    /// 异步通用发送系统事件
    /// </summary>
    public abstract class SendSystemAsync<E, T1, T2, T3, T4> : SendSystemAsyncBase<ISendSystemAsync<T1, T2, T3, T4>, E, T1, T2, T3, T4> where E : Entity { }
    /// <summary>
    /// 异步通用发送系统事件
    /// </summary>
    public abstract class SendSystemAsync<E, T1, T2, T3, T4, T5> : SendSystemAsyncBase<ISendSystemAsync<T1, T2, T3, T4, T5>, E, T1, T2, T3, T4, T5> where E : Entity { }
}
