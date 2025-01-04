/****************************************

* 作者：闪电黑客
* 日期：2024/10/31 21:01

* 描述：

*/


using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class StackFormatterRule
	{
		[TreeDataSpecial(1)]
		private class Serialize<T> : IEnumerableSpecialFormatterRule.SerializeBase<Stack<T>, T>
		{
			public override void ForeachWrite(TreeDataByteSequence self, Stack<T> obj)
			{
				self.WriteDynamic(obj.Count);
				foreach (var item in obj) self.WriteValue(item);
			}
		}

		private class Deserialize<T> : IEnumerableSpecialFormatterRule.DeserializeBase<Stack<T>, T>
		{
			public override void ForeachRead(TreeDataByteSequence self, Stack<T> obj)
			{
				if (obj.Count != 0) obj.Clear();
				self.ReadDynamic(out int length);
				for (int i = 0; i < length; i++) obj.Push(self.ReadValue<T>());
			}
		}
	}
}