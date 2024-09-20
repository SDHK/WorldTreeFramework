using System;
using System.Xml.Linq;

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
				//通过类型码获取类型
				self.TryReadType(out Type type);
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
