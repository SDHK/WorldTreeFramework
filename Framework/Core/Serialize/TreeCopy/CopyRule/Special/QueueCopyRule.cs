

using System;
using System.Collections.Generic;

namespace WorldTree.TreeCopys
{
	public static class QueueCopyRule
	{
		[TreeCopySpecial]
		private class Copy<T> : IEnumerableSpecialCopyRule.CopyRuleBase<Queue<T>, T>
		{
			public override void ForeachCopy(TreeCopier self, Queue<T> source, Queue<T> target)
			{
				foreach (var item in target)
				{
					if (item is IDisposable disposable) disposable.Dispose();
				}
				target.Clear();
				foreach (var item in source) target.Enqueue(self.Copy(item));
			}
		}
	}
}