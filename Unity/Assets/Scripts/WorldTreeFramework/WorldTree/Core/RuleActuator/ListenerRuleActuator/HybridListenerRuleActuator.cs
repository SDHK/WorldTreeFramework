
/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/4 18:26

* 描述： 混合型监听器法则执行器

*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{

	/// <summary>
	/// 混合型监听器法则执行器
	/// </summary>
	public class HybridListenerRuleActuator : Node, IListenerIgnorer, IRuleActuatorEnumerable, IRuleActuator<IRule>
		, ComponentOf<HybridListenerRuleActuatorGroup>
		, AsAwake
	{
		/// <summary>
		/// 静态监听器法则执行器
		/// </summary>
		public StaticListenerRuleActuator staticListenerRuleActuator;
		/// <summary>
		/// 动态监听器法则执行器
		/// </summary>
		public DynamicListenerRuleActuator dynamicListenerRuleActuator;

		public int TraversalCount => 0;

		public override string ToString()
		{
			return $"HybridListenerRuleActuator:{this.GetHashCode()} : {staticListenerRuleActuator == null} ??  {dynamicListenerRuleActuator == null}";
		}

		public IEnumerator<(INode, RuleList)> GetEnumerator()
		{
			if (staticListenerRuleActuator != null)
			{
				foreach (var item in staticListenerRuleActuator)
				{
					yield return item;
				}
			}
			if (dynamicListenerRuleActuator != null)
			{
				foreach (var item in dynamicListenerRuleActuator)
				{
					yield return item;
				}
			}

			//return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int RefreshTraversalCount()
		{
			return 0;
		}

		public bool TryDequeue(out (INode, RuleList) value)
		{
			value = default;
			return false;
		}

		public struct Enumerator : IEnumerator<ValueTuple<INode, RuleList>>, IDisposable, IEnumerator
		{

			private HybridListenerRuleActuator Actuator;

			/// <summary>
			/// 当前索引标记
			/// </summary>
			private int index;

			private ValueTuple<INode, RuleList> current;

			public (INode, RuleList) Current => current;

			object IEnumerator.Current => current;

			internal Enumerator(HybridListenerRuleActuator Actuator)
			{
				this.Actuator = Actuator;
				current = default;
				index = 0;
			}

			public void Dispose()
			{
				Actuator = null;
				current = default;
			}

			public bool MoveNext()
			{
				int staticTraversalCount = Actuator.staticListenerRuleActuator == null ? 0 : Actuator.staticListenerRuleActuator.TraversalCount;
				int dynamicTraversalCount = Actuator.dynamicListenerRuleActuator == null ? 0 : Actuator.dynamicListenerRuleActuator.TraversalCount;
				if (index < staticTraversalCount + dynamicTraversalCount)
				{
					if (staticTraversalCount != 0)
					{
						if (Actuator.staticListenerRuleActuator.TryDequeue(out current))
						{
							return true;
						}
					}
					if (dynamicTraversalCount != 0)
					{
						if (Actuator.dynamicListenerRuleActuator.TryDequeue(out current))
						{
							return true;
						}
					}
					index++;
				}
				return false;
			}

			public void Reset()
			{

			}
		}
	}

	public static class HybridListenerRuleActuatorRule
	{
		class RemoveRule : RemoveRule<HybridListenerRuleActuator>
		{
			protected override void Execute(HybridListenerRuleActuator self)
			{
				self.staticListenerRuleActuator = null;
				self.dynamicListenerRuleActuator = null;
			}
		}
	}
}
