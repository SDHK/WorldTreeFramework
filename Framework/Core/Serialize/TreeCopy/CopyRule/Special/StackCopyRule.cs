

using System;
using System.Collections.Generic;

namespace WorldTree.TreeCopys
{
	public static class StackCopyRule
	{
		[TreeCopySpecial]
		private class Copy<T> : IEnumerableSpecialCopyRule.CopyRuleBase<Stack<T>, T>
		{
			public override void ForeachCopy(TreeCopier self, Stack<T> source, Stack<T> target)
			{
				foreach (var item in target)
				{
					if (item is IDisposable disposable) disposable.Dispose();
				}
				target.Clear();
				foreach (var item in source) target.Push(self.Copy(item));
			}
		}
	}
}