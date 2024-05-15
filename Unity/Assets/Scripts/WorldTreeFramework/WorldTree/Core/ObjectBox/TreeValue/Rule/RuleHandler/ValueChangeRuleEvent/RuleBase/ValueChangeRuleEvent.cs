/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/4 17:20

* 描述： 数值变化监听事件法则

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 泛型数值监听接口，用于监听未知类型，同时解除AsRule的法则限制
	/// </summary>
	public interface IValueChangeEvent : IRule
	{ }

	/// <summary>
	/// 数值变化监听事件法则
	/// </summary>
	public interface ValueChangeEvent<T1> : ISendRule<T1>, IValueChangeEvent, IRuleSupplementIgnore
		where T1 : IEquatable<T1>
	{ }

	/// <summary>
	/// 数值变化监听事件法则(同类型转换)
	/// </summary>
	public abstract class ValueChangeRuleEvent<N, T1> : SendRule<N, ValueChangeEvent<T1>, T1>
		where N : class, INode, AsRule<ValueChangeEvent<T1>>
		where T1 : IEquatable<T1>
	{ }

	/// <summary>
	/// 数值变化监听事件法则(同类型转换)
	/// </summary>
	public abstract class TreeValueChangeRuleEvent<T1> : ValueChangeRuleEvent<TreeValueBase<T1>, T1>
		where T1 : IEquatable<T1>
	{ }

	/// <summary>
	/// 数值变化监听事件法则(不同类型转换)
	/// </summary>
	public abstract class TreeValueGenericsChangeRuleEvent<T1, T2> : ValueChangeRuleEvent<TreeValueBase<T1>, T2>
		where T1 : IEquatable<T1>
		where T2 : IEquatable<T2>
	{ }
}