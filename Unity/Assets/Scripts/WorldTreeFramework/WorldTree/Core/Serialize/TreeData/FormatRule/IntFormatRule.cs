using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace WorldTree.TreeDataFormats
{
	public static class IntFormatRule
	{
		//泛型一维
		class ArraySerialize : TreeDataSerializeRule<TreeDataByteSequence, int[]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object arg1)
			{
				self.WriteType(typeof(int[]));
				self.DangerousWriteUnmanagedArray((int[])arg1);
			}
		}

		class ArrayDeserialize : TreeDataDeserializeRule<TreeDataByteSequence, int[]>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value)
			{
				self.TryReadType(out Type type);
				if (type == typeof(int[]))
				{
					self.DangerousReadUnmanagedArray(ref Unsafe.AsRef<int[]>(Unsafe.AsPointer(ref value)));
				}
				else
				{
					//读取指针回退，类型码
					self.ReadBack(8);
					//跳跃数据
					self.SkipData();
				}
			}
		}

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
