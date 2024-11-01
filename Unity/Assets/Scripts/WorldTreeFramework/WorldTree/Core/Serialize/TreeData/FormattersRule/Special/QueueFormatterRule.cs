

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
				self.WriteUnmanaged(obj.Count);
				foreach (var item in obj) self.WriteValue(item);
			}
		}

		private class Deserialize<T> : IEnumerableSpecialFormatterRule.DeserializeBase<Queue<T>, T>
		{
			public override void ForeachRead(TreeDataByteSequence self, Queue<T> obj)
			{
				if (obj.Count != 0) obj.Clear();
				self.ReadUnmanaged(out int length);
				if (length == 0) return;
				for (int i = 0; i < length; i++) obj.Enqueue(self.ReadValue<T>());
			}
		}
	}
}