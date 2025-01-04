/****************************************

* 作者：闪电黑客
* 日期：2024/10/31 20:55

* 描述：

*/

using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class HashSetFormatterRule
	{
		[TreeDataSpecial(1)]
		private class Serialize<T> : ICollectionSpecialFormatterRule.SerializeBase<HashSet<T>, T> { }

		private class Deserialize<T> : ICollectionSpecialFormatterRule.DeserializeBase<HashSet<T>, T> { }
	}
}