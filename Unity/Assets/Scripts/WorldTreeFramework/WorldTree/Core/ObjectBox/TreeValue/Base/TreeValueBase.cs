/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/6 10:34

* 描述： 树节点值类型基类

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 树节点值类型基类
	/// </summary>
	public abstract class TreeValueBase : Node
	{ }

	/// <summary>
	/// 树节点值类型泛型基类
	/// </summary>
	public abstract partial class TreeValueBase<T> : TreeValueBase
		, AsChildBranch
		, AsComponentBranch
		, ChildOf<INode>
		, AsRule<IValueChangeEvent>
		where T : IEquatable<T>

	{
		/// <summary>
		/// 全局法则执行器
		/// </summary>
		public IRuleActuator<ValueChangeEvent<T>> m_GlobalValueChange;

		/// <summary>
		/// 法则执行器
		/// </summary>
		public RuleActuator<ValueChangeEvent<T>> m_ValueChange;

		/// <summary>
		/// 值
		/// </summary>
		public virtual T Value { get; set; }

		public static implicit operator T(TreeValueBase<T> treeValueBase)
		{
			return treeValueBase.Value;
		}
	}
}