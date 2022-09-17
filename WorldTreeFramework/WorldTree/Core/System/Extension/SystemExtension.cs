namespace WorldTree
{
    public static class SystemExtension
    {

        #region Send

        public static bool TrySendSystem<S>(this Entity self)
        where S : ISendSystem
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return group.TrySend(self);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendSystem<S, T1>(this Entity self, T1 arg1)
        where S : ISendSystem<T1>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return group.TrySend(self, arg1);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendSystem<S, T1, T2>(this Entity self, T1 arg1, T2 arg2)
        where S : ISendSystem<T1, T2>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return group.TrySend(self, arg1, arg2);
            }
            else
            {
                return false;
            }
        }

        public static bool TrySendSystem<S, T1, T2, T3>(this Entity self, T1 arg1, T2 arg2, T3 arg3)
      where S : ISendSystem<T1, T2, T3>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return group.TrySend(self, arg1, arg2, arg3);
            }
            else
            {
                return false;
            }
        }

        public static void SendSystem<S>(this Entity self)
            where S : ISendSystem
        {
            self.TrySendSystem<S>();
        }
        public static void SendSystem<S, T1>(this Entity self, T1 arg1)
           where S : ISendSystem<T1>
        {
            self.TrySendSystem<S, T1>(arg1);
        }
        public static void SendSystem<S, T1, T2>(this Entity self, T1 arg1, T2 arg2)
           where S : ISendSystem<T1, T2>
        {
            self.TrySendSystem<S, T1, T2>(arg1, arg2);
        }
        public static void SendSystem<S, T1, T2, T3>(this Entity self, T1 arg1, T2 arg2, T3 arg3)
           where S : ISendSystem<T1, T2, T3>
        {
            self.TrySendSystem<S, T1, T2, T3>(arg1, arg2, arg3);
        }

        #endregion

        #region Call

        public static bool TryCallSystem<S, OutT>(this Entity self, out OutT outT)
          where S : ICallSystem<OutT>
        {

            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return group.TryCall(self, out outT);
            }
            else
            {
                outT = default(OutT);
                return false;
            }

        }
        public static bool TryCallSystem<S, T1, OutT>(this Entity self, T1 arg1, out OutT outT)
          where S : ICallSystem<T1, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return group.TryCall(self, arg1, out outT);
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }


        public static OutT CallSystem<S, OutT>(this Entity self)
          where S : ICallSystem<OutT>
        {
            self.TryCallSystem<S, OutT>(out OutT outT);
            return outT;
        }
        public static OutT CallSystem<S, T1, OutT>(this Entity self, T1 arg1)
          where S : ICallSystem<T1, OutT>
        {
            self.TryCallSystem<S, T1, OutT>(arg1, out OutT outT);
            return outT;
        }


        #endregion


        #region Calls
        public static bool TryCallsSystem<S, OutT>(this Entity self, out UnitList<OutT> values)
        where S : ICallSystem<OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return group.TryCalls(self, out values);
            }
            else
            {
                values = null;
                return false;
            }
        }
        public static bool TryCallsSystem<S, T1, OutT>(this Entity self, T1 arg1, out UnitList<OutT> values)
        where S : ICallSystem<T1, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return group.TryCalls(self, arg1, out values);
            }
            else
            {
                values = null;
                return false;
            }
        }

        public static UnitList<OutT> CallsSystem<S, OutT>(this Entity self)
        where S : ICallSystem<OutT>
        {
            self.TryCallsSystem<S, OutT>(out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> CallsSystem<S, T1, OutT>(this Entity self, T1 arg1)
        where S : ICallSystem<T1, OutT>
        {
            self.TryCallsSystem<S, T1, OutT>(arg1, out UnitList<OutT> outT);
            return outT;
        }

        #endregion


    }


}
