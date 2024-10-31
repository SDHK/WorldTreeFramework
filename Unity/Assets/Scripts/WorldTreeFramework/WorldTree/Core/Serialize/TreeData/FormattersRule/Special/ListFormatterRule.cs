
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class ListFormatterRule
	{
		private class Serialize<T> : IEnumerableSpecialFormatterRule.SerializeBase<List<T>, T>
		{
			public override void ForeachWrite(TreeDataByteSequence self, List<T> obj)
			{
				self.WriteUnmanaged(obj.Count);
				if (obj.Count == 0) return;
				foreach (var item in obj) self.WriteValue(item);
			}
		}

		private class Deserialize<T> : IEnumerableSpecialFormatterRule.DeserializeBase<List<T>, T>
		{
			public override void ForeachRead(TreeDataByteSequence self, List<T> obj)
			{
				if (obj.Count != 0) obj.Clear();
				self.ReadUnmanaged(out int length);
				if (length == 0) return;
				for (int i = 0; i < length; i++) obj.Add(self.ReadValue<T>());
			}
		}
	}
}