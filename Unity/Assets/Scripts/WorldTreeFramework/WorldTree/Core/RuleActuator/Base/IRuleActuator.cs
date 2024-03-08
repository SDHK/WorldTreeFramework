/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/30 19:23

* 描述： 
* 

*/

using System.Collections.Generic;
using System;

namespace WorldTree
{
	/// <summary>
	/// 法则执行器的逆变泛型限制接口
	/// </summary>
	/// <typeparam name="T">法则类型</typeparam>
	/// <remarks>
	/// <para>主要作用是通过法则类型逆变提示可填写参数</para>
	/// </remarks>
	public interface IRuleActuator<in T> : IRuleActuatorBase where T : IRule { }

	/// <summary>
	/// 法则执行器接口基类
	/// </summary>
	public interface IRuleActuatorBase : INode { }

	/// <summary>
	/// 法则执行器遍历接口
	/// </summary>
	/// <remarks>让执行器可以遍历执行</remarks>
	public interface IRuleActuatorEnumerable : IRuleActuatorBase, IEnumerable<ValueTuple<INode, RuleList>> { }

	/// <summary>
	/// 法则列表执行器接口
	/// </summary>
	public interface IRuleGroupActuator : IRuleActuator
	{
		/// <summary>
		/// 尝试添加节点
		/// </summary>
		public bool TryAdd(INode node);
	}

	/// <summary>
	/// 法则执行器接口
	/// </summary>
	public interface IRuleActuator : IRuleActuatorBase
	{
		/// <summary>
		/// 尝试添加节点与对应法则
		/// </summary>
		public bool TryAdd(INode node, RuleList ruleList);

		/// <summary>
		/// 移除节点
		/// </summary>
		public void Remove(long id);
		/// <summary>
		/// 移除节点
		/// </summary>
		public void Remove(INode node);

		/// <summary>
		/// 清除
		/// </summary>
		public void Clear();
	}
}
