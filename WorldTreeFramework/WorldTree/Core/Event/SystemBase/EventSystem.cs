
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/22 9:40

* 描述： 事件系统基类

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 事件系统接口
    /// </summary>
    public interface IEventSystem : ISystem
    {
        Delegate GetDeleate();
    }

    #region Action

    public abstract class EventSendSystem : SystemBase<Action, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Action)Event;
        public abstract void Event();
    }

    public abstract class EventSendSystem<T1> : SystemBase<Action<T1>, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Action<T1>)Event;
        public abstract void Event(T1 arg1);
    }

    public abstract class EventSendSystem<T1, T2> : SystemBase<Action<T1, T2>, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Action<T1, T2>)Event;
        public abstract void Event(T1 arg1, T2 arg2);
    }
    public abstract class EventSendSystem<T1, T2, T3> : SystemBase<Action<T1, T2, T3>, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Action<T1, T2, T3>)Event ;
        public abstract void Event(T1 arg1, T2 arg2, T3 arg3);
    }

    public abstract class EventSendSystem<T1, T2, T3, T4> : SystemBase<Action<T1, T2, T3, T4>, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Action<T1, T2, T3, T4>)Event;
        public abstract void Event(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public abstract class EventSendSystem<T1, T2, T3, T4, T5> : SystemBase<Action<T1, T2, T3, T4, T5>, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Action<T1, T2, T3, T4, T5>)Event;
        public abstract void Event(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
    #endregion

    #region Funcs

    public abstract class EventCallSystem<OutT> : SystemBase<Func<OutT>, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Func<OutT>)Event;
        public abstract OutT Event();
    }

    public abstract class EventCallSystem<T1, OutT> : SystemBase<Func<T1, OutT>, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Func<T1, OutT>)Event;
        public abstract OutT Event(T1 arg1);

    }

    public abstract class EventCallSystem<T1, T2, OutT> : SystemBase<Func<T1, T2, OutT>, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Func<T1, T2, OutT>)Event;
        public abstract OutT Event(T1 arg1, T2 arg2);
    }

    public abstract class EventCallSystem<T1, T2, T3, OutT> : SystemBase<Func<T1, T2, T3, OutT>, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Func<T1, T2, T3, OutT>)Event;
        public abstract OutT Event(T1 arg1, T2 arg2, T3 arg3);
    }

    public abstract class EventCallSystem<T1, T2, T3, T4, OutT> : SystemBase<Func<T1, T2, T3, T4, OutT>, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Func<T1, T2, T3, T4, OutT>)Event;
        public abstract OutT Event(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public abstract class EventCallSystem<T1, T2, T3, T4, T5, OutT> : SystemBase<Func<T1, T2, T3, T4, T5, OutT>, IEventSystem>, IEventSystem
    {
        public Delegate GetDeleate() => (Func<T1, T2, T3, T4, T5, OutT>)Event;
        public abstract OutT Event(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
    #endregion

}
