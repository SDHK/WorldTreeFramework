
using System;
using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 法则列表 接口基类
    /// </summary>
    public interface IRuleList { }

    /// <summary>
    /// 法则列表 逆变泛型接口
    /// </summary>
    /// <typeparam name="T">法则类型</typeparam>
    /// <remarks>
    /// <para>主要通过法则类型逆变提示可填写参数</para>
    /// <para> RuleList 是没有泛型反射实例的，所以执行参数可能填错</para>
    /// </remarks>
    public interface IRuleList<in T> : IRuleList where T : IRule { }

    /// <summary>
    /// 法则列表
    /// </summary>
    /// <remarks>储存相同节点类型，法则类型，的法则</remarks>
    public class RuleList : List<IRule>, IRuleList<IRule>
    {
        HashSet<Type> ruleTypes = new HashSet<Type>();

        /// <summary>
        /// 通过HashSet判断法则类真实类型，禁止重复添加
        /// </summary>
        public bool TryAdd(IRule rule)
        {
            if (!ruleTypes.Contains(rule.GetType()))
            {
                base.Add(rule);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
