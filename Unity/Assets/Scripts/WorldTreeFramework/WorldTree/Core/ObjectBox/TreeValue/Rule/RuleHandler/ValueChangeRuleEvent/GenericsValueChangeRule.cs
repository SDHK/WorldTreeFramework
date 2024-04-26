/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 11:41

* 描述：

*/

using System;

namespace WorldTree
{
	public static partial class TreeValueBaseRule
	{
		private class GenericsValueChangeRuleEvent<T> : ValueChangeRuleEvent<TreeValueBase<T>, T>
			where T : IEquatable<T>
		{
			protected override void Execute(TreeValueBase<T> self, T arg1)
			{
				self.Value = arg1;
			}
		}
	}
}