/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 15:22

* 描述： 节点法则异步通知执行

*/

namespace WorldTree
{
    public static class SendAsyncExtension
    {
        public static TreeTask<bool> TrySendAsync(this INode self) => self.TrySendRuleAsync(default(ISendRuleAsync));
        public static TreeTask<bool> TrySendAsync<T1>(this INode self, T1 arg1) => self.TrySendRuleAsync(default(ISendRuleAsync<T1>),arg1);
        public static TreeTask<bool> TrySendAsync<T1, T2>(this INode self, T1 arg1, T2 arg2) => self.TrySendRuleAsync(default(ISendRuleAsync<T1, T2>),arg1, arg2);
        public static TreeTask<bool> TrySendAsync<T1, T2, T3>(this INode self, T1 arg1, T2 arg2, T3 arg3) => self.TrySendRuleAsync(default(ISendRuleAsync<T1, T2, T3>),arg1, arg2, arg3);
        public static TreeTask<bool> TrySendAsync<T1, T2, T3, T4>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.TrySendRuleAsync(default(ISendRuleAsync<T1, T2, T3, T4>),arg1, arg2, arg3, arg4);
        public static TreeTask<bool> TrySendAsync<T1, T2, T3, T4, T5>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.TrySendRuleAsync(default(ISendRuleAsync<T1, T2, T3, T4, T5>),arg1, arg2, arg3, arg4, arg5);


        public static TreeTask SendAsync(this INode self) => self.SendRuleAsync(default(ISendRuleAsync));
        public static TreeTask SendAsync<T1>(this INode self, T1 arg1) => self.SendRuleAsync(default(ISendRuleAsync<T1>), arg1);
        public static TreeTask SendAsync<T1, T2>(this INode self, T1 arg1, T2 arg2) => self.SendRuleAsync(default(ISendRuleAsync<T1, T2>), arg1, arg2);
        public static TreeTask SendAsync<T1, T2, T3>(this INode self, T1 arg1, T2 arg2, T3 arg3) => self.SendRuleAsync(default(ISendRuleAsync<T1, T2, T3>), arg1, arg2, arg3);
        public static TreeTask SendAsync<T1, T2, T3, T4>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.SendRuleAsync(default(ISendRuleAsync<T1, T2, T3, T4>), arg1, arg2, arg3, arg4);
        public static TreeTask SendAsync<T1, T2, T3, T4, T5>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.SendRuleAsync(default(ISendRuleAsync<T1, T2, T3, T4, T5>), arg1, arg2, arg3, arg4, arg5);

    }
}
