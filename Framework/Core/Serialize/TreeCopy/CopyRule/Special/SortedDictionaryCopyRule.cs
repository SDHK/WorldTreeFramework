

using System;
using System.Collections.Generic;

namespace WorldTree.TreeCopys
{
	public static class SortedDictionaryCopyRule
	{
		[TreeCopySpecial]
		private class Copy<TKey, TValue> : IEnumerableSpecialCopyRule.CopyRuleBase<SortedDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
		{
			public override void ForeachCopy(TreeCopier self, SortedDictionary<TKey, TValue> source, SortedDictionary<TKey, TValue> target)
			{
				foreach (var item in target)
				{
					if (item.Key is IDisposable disposableKey) disposableKey.Dispose();
					if (item.Value is IDisposable disposableValue) disposableValue.Dispose();
				}
				target.Clear();
				foreach (var item in source) target.Add(self.Copy(item.Key), self.Copy(item.Value));
			}
		}
	}
}