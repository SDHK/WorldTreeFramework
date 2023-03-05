/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:10

* 描述： 通用调用系统事件

*/

namespace WorldTree
{
    /// <summary>
    /// 通用调用系统事件
    /// </summary>
    public abstract class CallSystem<E, OutT> : CallSystemBase<ICallSystem<OutT>, E, OutT> where E : Node { }
    /// <summary>
    /// 通用调用系统事件
    /// </summary>
    public abstract class CallSystem<E, T1, OutT> : CallSystemBase<ICallSystem<T1, OutT>, E, T1, OutT> where E : Node { }
    /// <summary>
    /// 通用调用系统事件
    /// </summary>
    public abstract class CallSystem<E, T1, T2, OutT> : CallSystemBase<ICallSystem<T1, T2, OutT>, E, T1, T2, OutT> where E : Node { }
    /// <summary>
    /// 通用调用系统事件
    /// </summary>
    public abstract class CallSystem<E, T1, T2, T3, OutT> : CallSystemBase<ICallSystem<T1, T2, T3, OutT>, E, T1, T2, T3, OutT> where E : Node { }
    /// <summary>
    /// 通用调用系统事件
    /// </summary>
    public abstract class CallSystem<E, T1, T2, T3, T4, OutT> : CallSystemBase<ICallSystem<T1, T2, T3, T4, OutT>, E, T1, T2, T3, T4, OutT> where E : Node { }
    /// <summary>
    /// 通用调用系统事件
    /// </summary>
    public abstract class CallSystem<E, T1, T2, T3, T4, T5, OutT> : CallSystemBase<ICallSystem<T1, T2, T3, T4, T5, OutT>, E, T1, T2, T3, T4, T5, OutT> where E : Node { }


}
