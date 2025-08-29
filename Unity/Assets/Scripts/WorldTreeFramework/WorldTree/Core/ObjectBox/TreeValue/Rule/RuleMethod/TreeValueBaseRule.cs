/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/24 11:33

* 描述： 树节点值类型基类

*/

using System;

namespace WorldTree
{
	public static partial class TreeValueBaseRule
	{
		/// <summary>
		/// 设置一个全局法则执行器
		/// </summary>
		public static void SetGlobalRuleActuator<T, R>(this TreeValueBase<T> self, R defaultRule = default)
			where T : IEquatable<T>
			where R : ISendRule<T>, IGlobalRule
		{
			//self.globalValueChange = (IRuleExecutor<ISendRule<T>>)self.Core.GetGlobalRuleExecutor(out IRuleExecutor<R> _);
		}

		/// <summary>
		/// 单向绑定 数值监听
		/// </summary>
		public static void AddListenerValueChange<T1, N>(this TreeValueBase<T1> self, N eventNode)
			where T1 : IEquatable<T1>
			where N : class, INode, AsRule<ISendRule<T1>>
		{
			if (self.valueChange is null) self.valueChange = self.AddChild(out RuleMulticaster<ValueChangeEvent<T1>> _);
			self.valueChange.Add(eventNode);
		}

		/// <summary>
		/// 单向绑定(类型转换)
		/// </summary>
		public static void Bind<T1, T2>(this TreeValueBase<T1> self, TreeValueBase<T2> treeValue)
			where T1 : IEquatable<T1>
			where T2 : IEquatable<T2>
		{
			if (self.valueChange is null) self.AddChild(out self.valueChange);
			self.valueChange.Add(treeValue);
		}

		/// <summary>
		/// 双向绑定(类型转换)
		/// </summary>
		public static void BindTwoWay<T1, T2>(this TreeValueBase<T1> self, TreeValueBase<T2> treeValue)
			where T1 : IEquatable<T1>
			where T2 : IEquatable<T2>
		{
			if (self.valueChange is null) self.AddChild(out self.valueChange);
			if (treeValue.valueChange is null) treeValue.AddChild(out treeValue.valueChange);
			self.valueChange.Add(treeValue);
			treeValue.valueChange.Add(self);
		}
	}
}