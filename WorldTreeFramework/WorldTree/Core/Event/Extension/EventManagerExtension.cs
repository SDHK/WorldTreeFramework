
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/22 9:40

* 描述： 事件管理器扩展方法

*/

using System;

namespace WorldTree
{


    /// <summary>
    /// 根节点组件调用扩展
    /// </summary>
    public static class EventManagerExtension
    {
        /// <summary>
        /// 获取事件管理器
        /// </summary>
        public static EventManager EventManager(this Entity self)
        {
            return self.Root.EventManager;
        }

        /// <summary>
        /// 获取默认事件
        /// </summary>
        public static EventDelegate Event(this Entity self)
        {
            return self.Root.EventManager.AddComponent<EventDelegate>();
        }



        /// <summary>
        /// 移除默认事件
        /// </summary>
        public static void EventRemove(this Entity self)
        {
            self.Root.EventManager.RemoveComponent<EventDelegate>();
        }

        /// <summary>
        /// 获取分组事件
        /// </summary>
        public static EventDelegate Event<Key>(this Entity self)
        where Key : EventDelegate
        {
            return self.Root.EventManager.AddComponent<Key>();
        }

        /// <summary>
        /// 移除分组事件
        /// </summary>
        public static void EventRemove<Key>(this Entity self)
        where Key : EventDelegate
        {
            self.Root.EventManager.RemoveComponent<Key>();
        }


        //=========

        #region Send
        public static void Send<E>(this E self)
          where E : Entity
        {
            self.Event().TrySend(self);
        }

        public static void Send<E, T1>(this E self, T1 arg1)
         where E : Entity
        {
            self.Event().TrySend(self, arg1);
        }

        public static void Send<E, T1, T2>(this E self, T1 arg1, T2 arg2)
         where E : Entity
        {
            self.Event().TrySend(self, arg1, arg2);
        }

        public static void Send<E, T1, T2, T3>(this E self, T1 arg1, T2 arg2, T3 arg3)
        where E : Entity
        {
            self.Event().TrySend(self, arg1, arg2, arg3);
        }

        public static void Send<E, T1, T2, T3, T4>(this E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where E : Entity
        {
            self.Event().TrySend(self, arg1, arg2, arg3, arg4);
        }


        public static bool TrySend<E>(this E self)
          where E : Entity
        {
            return self.Event().TrySend(self);
        }

        public static bool TrySend<E, T1>(this E self, T1 arg1)
         where E : Entity
        {
            return self.Event().TrySend(self, arg1);
        }

        public static bool TrySend<E, T1, T2>(this E self, T1 arg1, T2 arg2)
         where E : Entity
        {
            return self.Event().TrySend(self, arg1, arg2);
        }

        public static bool TrySend<E, T1, T2, T3>(this E self, T1 arg1, T2 arg2, T3 arg3)
        where E : Entity
        {
            return self.Event().TrySend(self, arg1, arg2, arg3);
        }

        public static bool TrySend<E, T1, T2, T3, T4>(this E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where E : Entity
        {
            return self.Event().TrySend(self, arg1, arg2, arg3, arg4);
        }

        #region Async

        public static async AsyncTask SendAsync<E>(this E self)
        where E : Entity
        {
            await self.Event().SendAsync(self);
        }

        public static async AsyncTask SendAsync<E, T1>(this E self, T1 arg1)
        where E : Entity
        {
            await self.Event().SendAsync(self, arg1);
        }

        public static async AsyncTask SendAsync<E, T1, T2>(this E self, T1 arg1, T2 arg2)
         where E : Entity
        {
            await self.Event().SendAsync(self, arg1, arg2);
        }

        public static async AsyncTask SendAsync<E, T1, T2, T3>(this E self, T1 arg1, T2 arg2, T3 arg3)
        where E : Entity
        {
            await self.Event().SendAsync(self, arg1, arg2, arg3);
        }

        public static async AsyncTask SendAsync<E, T1, T2, T3, T4>(this E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where E : Entity
        {
            await self.Event().SendAsync(self, arg1, arg2, arg3, arg4);
        }

        public static async AsyncTask<bool> TrySendAsync<E>(this E self)
        where E : Entity
        {
            return await self.Event().TrySendAsync(self);
        }

        public static async AsyncTask<bool> TrySendAsync<E, T1>(this E self, T1 arg1)
        where E : Entity
        {
            return await self.Event().TrySendAsync(self, arg1);
        }

        public static async AsyncTask<bool> TrySendAsync<E, T1, T2>(this E self, T1 arg1, T2 arg2)
         where E : Entity
        {
            return await self.Event().TrySendAsync(self, arg1, arg2);
        }

        public static async AsyncTask<bool> TrySendAsync<E, T1, T2, T3>(this E self, T1 arg1, T2 arg2, T3 arg3)
        where E : Entity
        {
            return await self.Event().TrySendAsync(self, arg1, arg2, arg3);
        }

        public static async AsyncTask<bool> TrySendAsync<E, T1, T2, T3, T4>(this E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where E : Entity
        {
            return await self.Event().TrySendAsync(self, arg1, arg2, arg3, arg4);
        }


        #endregion

        #endregion

    }
}
