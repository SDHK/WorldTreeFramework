
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:04

* 描述： 通用发送系统事件

*/

namespace WorldTree
{
    /// <summary>
    /// 通用发送系统事件
    /// </summary>
    public abstract class SendSystem<E> : SendSystemBase<ISendSystem, E> where E : Entity { }
    /// <summary>
    /// 通用发送系统事件
    /// </summary>
    public abstract class SendSystem<E, T1> : SendSystemBase<ISendSystem<T1>, E, T1> where E : Entity { }
    /// <summary>
    /// 通用发送系统事件
    /// </summary>
    public abstract class SendSystem<E, T1, T2> : SendSystemBase<ISendSystem<T1, T2>, E, T1, T2> where E : Entity { }
    /// <summary>
    /// 通用发送系统事件
    /// </summary>
    public abstract class SendSystem<E, T1, T2, T3> : SendSystemBase<ISendSystem<T1, T2, T3>, E, T1, T2, T3> where E : Entity { }
    /// <summary>
    /// 通用发送系统事件
    /// </summary>
    public abstract class SendSystem<E, T1, T2, T3, T4> : SendSystemBase<ISendSystem<T1, T2, T3, T4>, E, T1, T2, T3, T4> where E : Entity { }
    /// <summary>
    /// 通用发送系统事件
    /// </summary>
    public abstract class SendSystem<E, T1, T2, T3, T4, T5> : SendSystemBase<ISendSystem<T1, T2, T3, T4, T5>, E, T1, T2, T3, T4, T5> where E : Entity { }

}
