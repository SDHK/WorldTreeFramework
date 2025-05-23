using System.Collections.Generic;

namespace WorldTree.TreeCopys
{
	public static class HashSetCopyRule
	{
		[TreeCopySpecial]
		private class Copy<T> : ICollectionSpecialCopyRule.CopyRuleBase<HashSet<T>, T> { }
	}
}