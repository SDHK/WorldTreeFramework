using System;
using System.Runtime.CompilerServices;

namespace WorldTree.TreeDataFormats
{
	public static class IntFormatRule
	{
		class Serialize : TreeDataSerializeRule<TreeDataByteSequence, int>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				int data = (int)value;
				self.WriteUnmanaged(self.AddTypeCode(typeof(int)));
				self.WriteUnmanaged(data);
			}
		}
		class Deserialize : TreeDataDeserializeRule<TreeDataByteSequence, int>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value)
			{
				self.ReadUnmanaged(out long typeCode);
				//通过类型码获取类型
				self.TryGetType(typeCode, out Type type);
				//是本身类型，正常读取流程
				if (typeof(int) == type)
				{
					self.ReadUnmanaged(out int data);
					value = data;
				}
			}
		}
	}
}
