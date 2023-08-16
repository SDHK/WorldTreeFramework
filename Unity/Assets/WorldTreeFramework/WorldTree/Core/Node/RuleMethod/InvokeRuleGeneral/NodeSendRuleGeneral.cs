/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:45

* 描述： 节点法则通知执行

*/

namespace WorldTree
{
    public static class NodeSendRuleGeneral
    {
        public static void Send<N>(this N self) where N : class, INode, AsRule<ISendRule> => self.SendRule(NullType<ISendRule>.Null);
        public static void Send<N, T1>(this N self, T1 arg1) where N : class, INode, AsRule<ISendRule<T1>> => self.SendRule(NullType<ISendRule<T1>>.Null, arg1);
        public static void Send<N, T1, T2>(this N self, T1 arg1, T2 arg2) where N : class, INode, AsRule<ISendRule<T1, T2>> => self.SendRule(NullType<ISendRule<T1, T2>>.Null, arg1, arg2);
        public static void Send<N, T1, T2, T3>(this N self, T1 arg1, T2 arg2, T3 arg3) where N : class, INode, AsRule<ISendRule<T1, T2, T3>> => self.SendRule(NullType<ISendRule<T1, T2, T3>>.Null, arg1, arg2, arg3);
        public static void Send<N, T1, T2, T3, T4>(this N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where N : class, INode, AsRule<ISendRule<T1, T2, T3, T4>> => self.SendRule(NullType<ISendRule<T1, T2, T3, T4>>.Null, arg1, arg2, arg3, arg4);
        public static void Send<N, T1, T2, T3, T4, T5>(this N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where N : class, INode, AsRule<ISendRule<T1, T2, T3, T4, T5>> => self.SendRule(NullType<ISendRule<T1, T2, T3, T4, T5>>.Null, arg1, arg2, arg3, arg4, arg5);

        public static bool TrySend(this INode self) => self.TrySendRule(NullType<ISendRule>.Null);
        public static bool TrySend<T1>(this INode self, T1 arg1) => self.TrySendRule(NullType<ISendRule<T1>>.Null, arg1);
        public static bool TrySend<T1, T2>(this INode self, T1 arg1, T2 arg2) => self.TrySendRule(NullType<ISendRule<T1, T2>>.Null, arg1, arg2);
        public static bool TrySend<T1, T2, T3>(this INode self, T1 arg1, T2 arg2, T3 arg3) => self.TrySendRule(NullType<ISendRule<T1, T2, T3>>.Null, arg1, arg2, arg3);
        public static bool TrySend<T1, T2, T3, T4>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.TrySendRule(NullType<ISendRule<T1, T2, T3, T4>>.Null, arg1, arg2, arg3, arg4);
        public static bool TrySend<T1, T2, T3, T4, T5>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.TrySendRule(NullType<ISendRule<T1, T2, T3, T4, T5>>.Null, arg1, arg2, arg3, arg4, arg5);

    }
}
