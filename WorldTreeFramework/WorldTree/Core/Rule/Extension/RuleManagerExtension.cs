/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/17 14:22

* 描述： 

*/
using System;
using System.Collections.Generic;

namespace WorldTree
{
    public static class RuleManagerExtension
    {

        public static RuleManager RuleManager(this INode self)
        {
            return self.Core.RuleManager;
        }

        /// <summary>
        /// 获取法则集合
        /// </summary>
        public static RuleGroup GetRuleGroup<T>(this INode self)
        where T : IRule
        {
            return self.Core.RuleManager.GetRuleGroup<T>();
        }


        /// <summary>
        /// 获取法则集合
        /// </summary>
        public static RuleGroup GetRuleGroup(this INode self, Type type)
        {
            return self.Core.RuleManager.GetRuleGroup(type);
        }

        /// <summary>
        /// 获取单类型法则列表
        /// </summary>
        public static IRuleList<R> GetRuleList<R>(this INode self, Type type)
        where R : IRule
        {
            return self.Core.RuleManager.GetRuleList<R>(type);
        }
    }

}
