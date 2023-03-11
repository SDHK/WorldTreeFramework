/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 10:57

* 描述： 法则集合异步调用执行

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class RuleGroupCallAsyncExtension
    {


        #region Call

        public static async TreeTask<OutT> CallAsync<OutT>(this RuleGroup group, INode self)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                outT = await ruleList.CallAsync<OutT>(self);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }


        public static async TreeTask<OutT> CallAsync<T1, OutT>(this RuleGroup group, INode self, T1 arg1)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                outT = await ruleList.CallAsync<T1, OutT>(self, arg1);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<T1, T2, OutT>(this RuleGroup group, INode self, T1 arg1, T2 arg2)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                outT = await ruleList.CallAsync<T1, T2, OutT>(self, arg1, arg2);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<T1, T2, T3, OutT>(this RuleGroup group, INode self, T1 arg1, T2 arg2, T3 arg3)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                outT = await ruleList.CallAsync<T1, T2, T3, OutT>(self, arg1, arg2, arg3);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }
        public static async TreeTask<OutT> CallAsync<T1, T2, T3, T4, OutT>(this RuleGroup group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                outT = await ruleList.CallAsync<T1, T2, T3, T4, OutT>(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<T1, T2, T3, T4, T5, OutT>(this RuleGroup group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                outT = await ruleList.CallAsync<T1, T2, T3, T4, T5, OutT>(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }


        #endregion

        #region Calls

        public static async TreeTask<UnitList<OutT>> CallsAsync<OutT>(this RuleGroup group, INode self)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                values = await ruleList.CallsAsync<OutT>(self);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, OutT>(this RuleGroup group, INode self, T1 arg1)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                values = await ruleList.CallsAsync<T1, OutT>(self, arg1);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, OutT>(this RuleGroup group, INode self, T1 arg1, T2 arg2)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                values = await ruleList.CallsAsync<T1, T2, OutT>(self, arg1, arg2);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, OutT>(this RuleGroup group, INode self, T1 arg1, T2 arg2, T3 arg3)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                values = await ruleList.CallsAsync<T1, T2, T3, OutT>(self, arg1, arg2, arg3);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, OutT>(this RuleGroup group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                values = await ruleList.CallsAsync<T1, T2, T3, T4, OutT>(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, T5, OutT>(this RuleGroup group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IRule> ruleList))
            {
                values = await ruleList.CallsAsync<T1, T2, T3, T4, T5, OutT>(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }
        #endregion

    }
}
