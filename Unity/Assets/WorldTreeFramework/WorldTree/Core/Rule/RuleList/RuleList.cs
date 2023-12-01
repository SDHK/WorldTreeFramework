
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
		/// <summary>
		/// 法则的类型
		/// </summary>
		public long RuleType;

		/// <summary>
		/// 重复添加则覆盖
		/// </summary>
		public void AddRule(IRule rule)
		{
			int index = FindIndex((old) => old.GetType() == rule.GetType());
			if (index == -1)
			{
				Add(rule);
			}
			else //重复则覆盖
			{
				this[index] = rule;
			}
		}
	}
}
