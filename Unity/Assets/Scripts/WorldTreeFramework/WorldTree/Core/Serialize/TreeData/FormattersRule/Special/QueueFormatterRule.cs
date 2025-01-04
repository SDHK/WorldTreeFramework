/****************************************

* 作者：闪电黑客
* 日期：2024/10/31 21:01

* 描述：

*/


using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class QueueFormatterRule
	{
		[TreeDataSpecial(1)]
		private class Serialize<T> : IEnumerableSpecialFormatterRule.SerializeBase<Queue<T>, T>
		{
			public override void ForeachWrite(TreeDataByteSequence self, Queue<T> obj)
			{
				self.WriteDynamic(obj.Count);
				foreach (var item in obj) self.WriteValue(item);
			}
		}

		private class Deserialize<T> : IEnumerableSpecialFormatterRule.DeserializeBase<Queue<T>, T>
		{
			public override void ForeachRead(TreeDataByteSequence self, Queue<T> obj)
			{
				if (obj.Count != 0) obj.Clear();
				self.ReadDynamic(out int length);
				if (length == 0) return;
				for (int i = 0; i < length; i++) obj.Enqueue(self.ReadValue<T>());
			}
		}
	}
}