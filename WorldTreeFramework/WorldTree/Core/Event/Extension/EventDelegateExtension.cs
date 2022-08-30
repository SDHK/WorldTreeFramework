/****************************************

 * 作者: 闪电黑客

 * 日期: 2021/10/18 6:31:30


 * 描述: 
 
    EventDelegate 事件委托 

    可添加 Action 和 Func 委托
    
    Send 调用同类型的多播委托
    TrySend 没有事件可调用则返回false

    Calls 调用同类型的多播委托，并返回获取到的全部返回值List
    TryCalls 没有事件可调用则返回false

    Call 调用同类型的多播委托，并返回获取到的最后一个值
    TryCall 没有事件可调用则返回false

    当前参数最多为 5
    
*/


using System;

namespace WorldTree
{

    public static partial class EventDelegateExtension
    {
        #region Action


        #region Add
        public static void Add(this EventDelegate e, Action delegate_) { e.AddDelegate(delegate_); }
        public static void Add<T1>(this EventDelegate e, Action<T1> delegate_) { e.AddDelegate(delegate_); }
        public static void Add<T1, T2>(this EventDelegate e, Action<T1, T2> delegate_) { e.AddDelegate(delegate_); }
        public static void Add<T1, T2, T3>(this EventDelegate e, Action<T1, T2, T3> delegate_) { e.AddDelegate(delegate_); }
        public static void Add<T1, T2, T3, T4>(this EventDelegate e, Action<T1, T2, T3, T4> delegate_) { e.AddDelegate(delegate_); }
        public static void Add<T1, T2, T3, T4, T5>(this EventDelegate e, Action<T1, T2, T3, T4, T5> delegate_) { e.AddDelegate(delegate_); }
        #endregion

        #region Remove
        public static void Remove(this EventDelegate e, Action delegate_) { e.RemoveDelegate(delegate_); }
        public static void Remove<T1>(this EventDelegate e, Action<T1> delegate_) { e.RemoveDelegate(delegate_); }
        public static void Remove<T1, T2>(this EventDelegate e, Action<T1, T2> delegate_) { e.RemoveDelegate(delegate_); }
        public static void Remove<T1, T2, T3>(this EventDelegate e, Action<T1, T2, T3> delegate_) { e.RemoveDelegate(delegate_); }
        public static void Remove<T1, T2, T3, T4>(this EventDelegate e, Action<T1, T2, T3, T4> delegate_) { e.RemoveDelegate(delegate_); }
        public static void Remove<T1, T2, T3, T4, T5>(this EventDelegate e, Action<T1, T2, T3, T4, T5> delegate_) { e.RemoveDelegate(delegate_); }
        #endregion

        #region Send

        public static void Send(this EventDelegate e)
        {
            e.TrySend();
        }
        public static void Send<T1>(this EventDelegate e, T1 arg1)
        {
            e.TrySend(arg1);
        }
        public static void Send<T1, T2>(this EventDelegate e, T1 arg1, T2 arg2)
        {
            e.TrySend(arg1, arg2);
        }
        public static void Send<T1, T2, T3>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3)
        {
            e.TrySend(arg1, arg2, arg3);
        }
        public static void Send<T1, T2, T3, T4>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            e.TrySend(arg1, arg2, arg3, arg4);
        }
        public static void Send<T1, T2, T3, T4, T5>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            e.TrySend(arg1, arg2, arg3, arg4, arg5);
        }

        public static bool TrySend(this EventDelegate e)
        {
            var events = e.Get<Action>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        (events[i] as Action)();
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TrySend<T1>(this EventDelegate e, T1 arg1)
        {
            var events = e.Get<Action<T1>>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        (events[i] as Action<T1>)(arg1);
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TrySend<T1, T2>(this EventDelegate e, T1 arg1, T2 arg2)
        {
            var events = e.Get<Action<T1, T2>>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        (events[i] as Action<T1, T2>)(arg1, arg2);
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TrySend<T1, T2, T3>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3)
        {
            var events = e.Get<Action<T1, T2, T3>>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        (events[i] as Action<T1, T2, T3>)(arg1, arg2, arg3);
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TrySend<T1, T2, T3, T4>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var events = e.Get<Action<T1, T2, T3, T4>>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        (events[i] as Action<T1, T2, T3, T4>)(arg1, arg2, arg3, arg4);
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TrySend<T1, T2, T3, T4, T5>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var events = e.Get<Action<T1, T2, T3, T4, T5>>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        (events[i] as Action<T1, T2, T3, T4, T5>)(arg1, arg2, arg3, arg4, arg5);
                        i++;
                    }
                }
            }
            return i != 0;
        }
        #endregion


        #endregion


        #region Func


        #region Add
        public static void Add<OutT>(this EventDelegate e, Func<OutT> delegate_) { e.AddDelegate(delegate_); }
        public static void Add<T1, OutT>(this EventDelegate e, Func<T1, OutT> delegate_) { e.AddDelegate(delegate_); }
        public static void Add<T1, T2, OutT>(this EventDelegate e, Func<T1, T2, OutT> delegate_) { e.AddDelegate(delegate_); }
        public static void Add<T1, T2, T3, OutT>(this EventDelegate e, Func<T1, T2, T3, OutT> delegate_) { e.AddDelegate(delegate_); }
        public static void Add<T1, T2, T3, T4, OutT>(this EventDelegate e, Func<T1, T2, T3, T4, OutT> delegate_) { e.AddDelegate(delegate_); }
        public static void Add<T1, T2, T3, T4, T5, OutT>(this EventDelegate e, Action<T1, T2, T3, T4, T5, OutT> delegate_) { e.AddDelegate(delegate_); }
        #endregion

        #region Remove
        public static void Remove<OutT>(this EventDelegate e, Func<OutT> delegate_) { e.RemoveDelegate(delegate_); }
        public static void Remove<T1, OutT>(this EventDelegate e, Func<T1, OutT> delegate_) { e.RemoveDelegate(delegate_); }
        public static void Remove<T1, T2, OutT>(this EventDelegate e, Func<T1, T2, OutT> delegate_) { e.RemoveDelegate(delegate_); }
        public static void Remove<T1, T2, T3, OutT>(this EventDelegate e, Func<T1, T2, T3, OutT> delegate_) { e.RemoveDelegate(delegate_); }
        public static void Remove<T1, T2, T3, T4, OutT>(this EventDelegate e, Func<T1, T2, T3, T4, OutT> delegate_) { e.RemoveDelegate(delegate_); }
        public static void Remove<T1, T2, T3, T4, T5, OutT>(this EventDelegate e, Action<T1, T2, T3, T4, T5, OutT> delegate_) { e.RemoveDelegate(delegate_); }
        #endregion

        #region Call
        public static OutT Call<OutT>(this EventDelegate e)
        {
            e.TryCall(out OutT value);
            return value;
        }
        public static OutT Call<T1, OutT>(this EventDelegate e, T1 arg1)
        {
            e.TryCall(arg1, out OutT value);
            return value;
        }
        public static OutT Call<T1, T2, OutT>(this EventDelegate e, T1 arg1, T2 arg2)
        {
            e.TryCall(arg1, arg2, out OutT value);
            return value;
        }
        public static OutT Call<T1, T2, T3, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3)
        {
            e.TryCall(arg1, arg2, arg3, out OutT value);
            return value;
        }
        public static OutT Call<T1, T2, T3, T4, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            e.TryCall(arg1, arg2, arg3, arg4, out OutT value);
            return value;
        }
        public static OutT Call<T1, T2, T3, T4, T5, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            e.TryCall(arg1, arg2, arg3, arg4, arg5, out OutT value);
            return value;
        }
        public static bool TryCall<OutT>(this EventDelegate e, out OutT value)
        {
            var events = e.Get<Func<OutT>>();
            value = default(OutT);
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        value = (events[i] as Func<OutT>)();
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TryCall<T1, OutT>(this EventDelegate e, T1 arg1, out OutT value)
        {
            var events = e.Get<Func<T1, OutT>>();
            value = default(OutT);
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        value = (events[i] as Func<T1, OutT>)(arg1);
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TryCall<T1, T2, OutT>(this EventDelegate e, T1 arg1, T2 arg2, out OutT value)
        {
            var events = e.Get<Func<T1, T2, OutT>>();
            value = default(OutT);
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        value = (events[i] as Func<T1, T2, OutT>)(arg1, arg2);
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TryCall<T1, T2, T3, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, out OutT value)
        {
            var events = e.Get<Func<T1, T2, T3, OutT>>();
            value = default(OutT);
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        value = (events[i] as Func<T1, T2, T3, OutT>)(arg1, arg2, arg3);
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TryCall<T1, T2, T3, T4, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out OutT value)
        {
            var events = e.Get<Func<T1, T2, T3, T4, OutT>>();
            value = default(OutT);
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        value = (events[i] as Func<T1, T2, T3, T4, OutT>)(arg1, arg2, arg3, arg4);
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TryCall<T1, T2, T3, T4, T5, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out OutT value)
        {
            var events = e.Get<Func<T1, T2, T3, T4, T5, OutT>>();
            value = default(OutT);
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        value = (events[i] as Func<T1, T2, T3, T4, T5, OutT>)(arg1, arg2, arg3, arg4, arg5);
                        i++;
                    }
                }
            }
            return i != 0;
        }

        #endregion

        #region Calls


        public static UnitList<OutT> Calls<OutT>(this EventDelegate e)
        {
            e.TryCalls(out UnitList<OutT> values);
            return values;
        }
        public static UnitList<OutT> Calls<T1, OutT>(this EventDelegate e, T1 arg1)
        {
            e.TryCalls(arg1, out UnitList<OutT> values);
            return values;
        }
        public static UnitList<OutT> Calls<T1, T2, OutT>(this EventDelegate e, T1 arg1, T2 arg2)
        {
            e.TryCalls(arg1, arg2, out UnitList<OutT> values);
            return values;
        }
        public static UnitList<OutT> Calls<T1, T2, T3, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3)
        {
            e.TryCalls(arg1, arg2, arg3, out UnitList<OutT> values);
            return values;
        }
        public static UnitList<OutT> Calls<T1, T2, T3, T4, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            e.TryCalls(arg1, arg2, arg3, arg4, out UnitList<OutT> values);
            return values;
        }
        public static UnitList<OutT> Calls<T1, T2, T3, T4, T5, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            e.TryCalls(arg1, arg2, arg3, arg4, arg5, out UnitList<OutT> values);
            return values;
        }


        public static bool TryCalls<OutT>(this EventDelegate e, out UnitList<OutT> values)
        {
            var events = e.Get<Func<OutT>>();
            values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        values.Add((events[i] as Func<OutT>)());
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TryCalls<T1, OutT>(this EventDelegate e, T1 arg1, out UnitList<OutT> values)
        {
            var events = e.Get<Func<T1, OutT>>();
            values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        values.Add((events[i] as Func<T1, OutT>)(arg1));
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TryCalls<T1, T2, OutT>(this EventDelegate e, T1 arg1, T2 arg2, out UnitList<OutT> values)
        {
            var events = e.Get<Func<T1, T2, OutT>>();
            values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        values.Add((events[i] as Func<T1, T2, OutT>)(arg1, arg2));
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TryCalls<T1, T2, T3, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, out UnitList<OutT> values)
        {
            var events = e.Get<Func<T1, T2, T3, OutT>>();
            values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        values.Add((events[i] as Func<T1, T2, T3, OutT>)(arg1, arg2, arg3));
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TryCalls<T1, T2, T3, T4, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out UnitList<OutT> values)
        {
            var events = e.Get<Func<T1, T2, T3, T4, OutT>>();
            values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        values.Add((events[i] as Func<T1, T2, T3, T4, OutT>)(arg1, arg2, arg3, arg4));
                        i++;
                    }
                }
            }
            return i != 0;
        }
        public static bool TryCalls<T1, T2, T3, T4, T5, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out UnitList<OutT> values)
        {
            var events = e.Get<Func<T1, T2, T3, T4, T5, OutT>>();
            values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            int i = 0;
            if (events != null)
            {
                while (i < events.Count)
                {
                    if (events[i] == null)
                    {
                        events.RemoveAt(i);
                    }
                    else
                    {
                        values.Add((events[i] as Func<T1, T2, T3, T4, T5, OutT>)(arg1, arg2, arg3, arg4, arg5));
                        i++;
                    }
                }
            }
            return i != 0;
        }

        #endregion

        #endregion

    
    
    
    }
}