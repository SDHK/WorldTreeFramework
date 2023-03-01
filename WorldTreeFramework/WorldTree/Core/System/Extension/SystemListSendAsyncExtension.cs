
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/22 15:21

* 描述： 系统列表系统事件异步发送

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemListSendAsyncExtension
    {
        #region Send

        public static async AsyncTask SendAsync(this List<IEntitySystem> systems, Entity self)
        {
            foreach (ISendSystemAsync system in systems)
            {
                await system.Invoke(self);
            }
        }

        public static async AsyncTask SendAsync<T1>(this List<IEntitySystem> systems, Entity self, T1 arg1)
        {
            foreach (ISendSystemAsync<T1> system in systems)
            {
                await system.Invoke(self, arg1);
            }
        }

        public static async AsyncTask SendAsync<T1, T2>(this List<IEntitySystem> systems, Entity self, T1 arg1, T2 arg2)
        {
            foreach (ISendSystemAsync<T1, T2> system in systems)
            {
                await system.Invoke(self, arg1, arg2);
            }
        }
        public static async AsyncTask SendAsync<T1, T2, T3>(this List<IEntitySystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            foreach (ISendSystemAsync<T1, T2, T3> system in systems)
            {
                await system.Invoke(self, arg1, arg2, arg3);
            }
        }
        public static async AsyncTask SendAsync<T1, T2, T3, T4>(this List<IEntitySystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            foreach (ISendSystemAsync<T1, T2, T3, T4> system in systems)
            {
                await system.Invoke(self, arg1, arg2, arg3, arg4);
            }
        }
        public static async AsyncTask SendAsync<T1, T2, T3, T4, T5>(this List<IEntitySystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            foreach (ISendSystemAsync<T1, T2, T3, T4, T5> system in systems)
            {
                await system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
            }
        }

        #endregion

    }
}
