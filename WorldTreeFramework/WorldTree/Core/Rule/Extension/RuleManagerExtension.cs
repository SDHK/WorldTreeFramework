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

        public static RuleManager RuleManager(this Node self)
        {
            return self.Root.RuleManager;
        }

        /// <summary>
        /// 获取系统组
        /// </summary>
        public static RuleGroup GetRuleGroup<T>(this Node self)
        where T : IRule
        {
            return self.Root.RuleManager.GetRuleGroup<T>();
        }


        /// <summary>
        /// 获取系统组
        /// </summary>
        public static RuleGroup GetRuleGroup(this Node self, Type type)
        {
            return self.Root.RuleManager.GetRuleGroup(type);
        }

        /// <summary>
        /// 获取单类型系统列表
        /// </summary>
        public static List<IRule> GetRuleList<R>(this Node self, Type type)
        where R : IRule
        {
            return self.Root.RuleManager.GetRuleList<R>(type);
        }
    }

}
