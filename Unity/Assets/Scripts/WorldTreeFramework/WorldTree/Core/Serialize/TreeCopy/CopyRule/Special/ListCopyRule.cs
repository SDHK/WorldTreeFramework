using System.Collections.Generic;

namespace WorldTree.TreeCopys
{
	public static class ListCopyRule
	{
		[TreeCopySpecial]
		private class Copy<T> : ICollectionSpecialCopyRule.CopyRuleBase<List<T>, T> { }
	}
}