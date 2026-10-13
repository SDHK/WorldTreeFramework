/****************************************

* 作者：闪电黑客
* 日期：2024/10/31 18:06

* 描述：

*/

using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class ListFormatterRule
	{
		[TreeDataSpecial(1)]
		private class Serialize<T> : ICollectionSpecialFormatterRule.SerializeBase<List<T>, T> { }

		private class Deserialize<T> : ICollectionSpecialFormatterRule.DeserializeBase<List<T>, T> { }
	}
}