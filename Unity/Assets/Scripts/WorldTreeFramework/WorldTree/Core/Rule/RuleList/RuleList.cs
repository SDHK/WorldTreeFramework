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
	public interface IRuleList<in T> : IRuleList where T : IRule
	{
	}

	/// <summary>
	/// 法则多播列表
	/// </summary>
	/// <remarks>储存相同节点类型，法则类型，的法则</remarks>
	public class RuleList : List<IRule>, IRuleList<IRule>
	{
		/// <summary>
		/// 节点的类型
		/// </summary>
		public long NodeType;

		/// <summary>
		/// 法则的类型
		/// </summary>
		public long RuleType;

		/// <summary>
		/// 重复添加则覆盖
		/// </summary>
		public void AddRule(IRule rule)
		{
			int index = FindIndex((old) => old.Type == rule.Type);
			if (index == -1)
			{
				Add(rule);
				for (int i = 0; i < Count; i++)
				{
					this[i].RuleIndex = i;
					this[i].RuleCount = Count;
				}
			}
			else //重复则覆盖
			{
				this[index] = rule;
				rule.RuleIndex = index;
				rule.RuleCount = Count;
				return;
			}
		}
	}
}
