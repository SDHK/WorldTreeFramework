/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/20 15:16

* 描述： 
    EventDelegate 异步事件委托 

    可添加 Action 和 Func 异步委托
    
    SendAsync 调用同类型的多播委托
    TrySendAsync 没有事件可调用则返回false

    CallsAsync 调用同类型的多播委托，并返回获取到的全部返回值List
    CallAsync 调用同类型的多播委托，并返回获取到的最后一个值

    当前参数最多为 5
*/

using System;

namespace WorldTree
{
    public static partial class EventDelegateExtension
    {
        #region Send
        public static async AsyncTask SendAsync<T1>(this EventDelegate e, T1 arg1)
        {
            await e.TrySendAsync(arg1);
        }
        public static async AsyncTask SendAsync<T1, T2>(this EventDelegate e, T1 arg1, T2 arg2)
        {
            await e.TrySendAsync(arg1, arg2);
        }
        public static async AsyncTask SendAsync<T1, T2, T3>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3)
        {
            await e.TrySendAsync(arg1, arg2, arg3);
        }
        public static async AsyncTask SendAsync<T1, T2, T3, T4>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            await e.TrySendAsync(arg1, arg2, arg3, arg4);
        }
        public static async AsyncTask SendAsync<T1, T2, T3, T4, T5>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            await e.TrySendAsync(arg1, arg2, arg3, arg4, arg5);
        }

        public static async AsyncTask<bool> TrySendAsync<T1>(this EventDelegate e, T1 arg1)
        {
            var events = e.Get<Func<T1, AsyncTask>>();
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
                        await (events[i] as Func<T1, AsyncTask>)(arg1);
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return i != 0;
        }

        public static async AsyncTask<bool> TrySendAsync<T1, T2>(this EventDelegate e, T1 arg1, T2 arg2)
        {
            var events = e.Get<Func<T1, T2, AsyncTask>>();
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
                        await (events[i] as Func<T1, T2, AsyncTask>)(arg1, arg2);
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return i != 0;
        }

        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3)
        {
            var events = e.Get<Func<T1, T2, T3, AsyncTask>>();
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
                        await (events[i] as Func<T1, T2, T3, AsyncTask>)(arg1, arg2, arg3);
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return i != 0;
        }

        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3, T4>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var events = e.Get<Func<T1, T2, T3, T4, AsyncTask>>();
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
                        await (events[i] as Func<T1, T2, T3, T4, AsyncTask>)(arg1, arg2, arg3, arg4);
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return i != 0;
        }

        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3, T4, T5>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var events = e.Get<Func<T1, T2, T3, T4, T5, AsyncTask>>();
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
                        await (events[i] as Func<T1, T2, T3, T4, T5, AsyncTask>)(arg1, arg2, arg3, arg4, arg5);
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return i != 0;
        }

        #endregion


        #region Call

        public static async AsyncTask<OutT> CallAsync<OutT>(this EventDelegate e)
        {
            var events = e.Get<Func<AsyncTask<OutT>>>();
            OutT value = default(OutT);
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
                        value = await (events[i] as Func<AsyncTask<OutT>>)();
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return value;
        }

        public static async AsyncTask<OutT> CallAsync<T1, OutT>(this EventDelegate e, T1 arg1)
        {
            var events = e.Get<Func<T1, AsyncTask<OutT>>>();
            OutT value = default(OutT);
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
                        value = await (events[i] as Func<T1, AsyncTask<OutT>>)(arg1);
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return value;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, OutT>(this EventDelegate e, T1 arg1, T2 arg2)
        {
            var events = e.Get<Func<T1, T2, AsyncTask<OutT>>>();
            OutT value = default(OutT);
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
                        value = await (events[i] as Func<T1, T2, AsyncTask<OutT>>)(arg1, arg2);
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return value;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3)
        {
            var events = e.Get<Func<T1, T2, T3, AsyncTask<OutT>>>();
            OutT value = default(OutT);
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
                        value = await (events[i] as Func<T1, T2, T3, AsyncTask<OutT>>)(arg1, arg2, arg3);
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return value;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, T4, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var events = e.Get<Func<T1, T2, T3, T4, AsyncTask<OutT>>>();
            OutT value = default(OutT);
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
                        value = await (events[i] as Func<T1, T2, T3, T4, AsyncTask<OutT>>)(arg1, arg2, arg3, arg4);
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return value;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, T4, T5, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var events = e.Get<Func<T1, T2, T3, T4, T5, AsyncTask<OutT>>>();
            OutT value = default(OutT);
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
                        value = await (events[i] as Func<T1, T2, T3, T4, T5, AsyncTask<OutT>>)(arg1, arg2, arg3, arg4, arg5);
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return value;
        }

        #endregion


        #region Calls

        public static async AsyncTask<UnitList<OutT>> CallsAsync<OutT>(this EventDelegate e)
        {
            var events = e.Get<Func<AsyncTask<OutT>>>();
            UnitList<OutT> values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
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
                        values.Add(await (events[i] as Func<AsyncTask<OutT>>)());
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return values;
        }

        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, OutT>(this EventDelegate e, T1 arg1)
        {
            var events = e.Get<Func<T1, AsyncTask<OutT>>>();
            UnitList<OutT> values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
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
                        values.Add(await (events[i] as Func<T1, AsyncTask<OutT>>)(arg1));
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return values;
        }

        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, OutT>(this EventDelegate e, T1 arg1, T2 arg2)
        {
            var events = e.Get<Func<T1, T2, AsyncTask<OutT>>>();
            UnitList<OutT> values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
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
                        values.Add(await (events[i] as Func<T1, T2, AsyncTask<OutT>>)(arg1, arg2));
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return values;
        }

        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3)
        {
            var events = e.Get<Func<T1, T2, T3, AsyncTask<OutT>>>();
            UnitList<OutT> values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
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
                        values.Add(await (events[i] as Func<T1, T2, T3, AsyncTask<OutT>>)(arg1, arg2, arg3));
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return values;
        }

        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var events = e.Get<Func<T1, T2, T3, T4, AsyncTask<OutT>>>();
            UnitList<OutT> values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
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
                        values.Add(await (events[i] as Func<T1, T2, T3, T4, AsyncTask<OutT>>)(arg1, arg2, arg3, arg4));
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return values;
        }

        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, T5, OutT>(this EventDelegate e, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var events = e.Get<Func<T1, T2, T3, T4, T5, AsyncTask<OutT>>>();
            UnitList<OutT> values = e.Root.ObjectPoolManager.Get<UnitList<OutT>>();
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
                        values.Add(await (events[i] as Func<T1, T2, T3, T4, T5, AsyncTask<OutT>>)(arg1, arg2, arg3, arg4, arg5));
                        i++;
                    }
                }
            }
            if (i == 0)
            {
                await e.AsyncYield();
            }
            return values;
        }

        #endregion

    }
}
