/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/5 17:18

* 描述： 数值组件string类型转换法则

*/

using System;

namespace WorldTree
{
	public static partial class TreeValueBaseRule
	{
		private class StringValueChangeRuleEvent<T> : TreeValueGenericsChangeRuleEvent<string, T>
		   where T : IEquatable<T>
		{
			protected override void Execute(TreeValueBase<string> self, T arg1)
			{
				self.Value = arg1.ToString();
			}
		}

		private class StringIntChangeRuleEvent : StringValueChangeRuleEvent<int>
		{ }

		private class StringShortChangeRuleEvent : StringValueChangeRuleEvent<short>
		{ }

		private class StringLongChangeRuleEvent : StringValueChangeRuleEvent<long>
		{ }

		private class StringFloatChangeRuleEvent : StringValueChangeRuleEvent<float>
		{ }

		private class StringDoubleChangeRuleEvent : StringValueChangeRuleEvent<double>
		{ }
	}
}