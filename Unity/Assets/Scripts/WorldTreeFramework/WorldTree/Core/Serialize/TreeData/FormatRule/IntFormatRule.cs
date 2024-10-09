using System;

namespace WorldTree.TreeDataFormats
{
	public static class IntFormatRule
	{
		class Serialize : TreeDataSerializeRule<TreeDataByteSequence, int>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				self.WriteType(typeof(int));
				self.WriteUnmanaged((int)value);
			}
		}

		class Deserialize : TreeDataDeserializeRule<TreeDataByteSequence, int>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value)
			{
				self.TryReadType(out Type type);
				if (typeof(int) == type)
				{
					self.ReadUnmanaged(out int data);
					value = data;
				}
			}
		}
	}
}
