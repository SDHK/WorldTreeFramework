/****************************************

* 作者：闪电黑客
* 日期：2024/10/22 10:54

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class SbyteFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, sbyte>> TypeDict = new()
		{
			[typeof(bool)] = (self) => (sbyte)(self.ReadUnmanaged<bool>() ? 1 : 0),
			[typeof(byte)] = (self) => (sbyte)self.ReadUnmanaged<byte>(),
			[typeof(sbyte)] = (self) => self.ReadUnmanaged<sbyte>(),
			[typeof(short)] = (self) => (sbyte)self.ReadUnmanaged<short>(),
			[typeof(ushort)] = (self) => (sbyte)self.ReadUnmanaged<ushort>(),
			[typeof(int)] = (self) => (sbyte)self.ReadUnmanaged<int>(),
			[typeof(uint)] = (self) => (sbyte)self.ReadUnmanaged<uint>(),
			[typeof(long)] = (self) => (sbyte)self.ReadUnmanaged<long>(),
			[typeof(ulong)] = (self) => (sbyte)self.ReadUnmanaged<ulong>(),
			[typeof(float)] = (self) => (sbyte)self.ReadUnmanaged<float>(),
			[typeof(double)] = (self) => (sbyte)self.ReadUnmanaged<double>(),
			[typeof(char)] = (self) => (sbyte)self.ReadUnmanaged<char>(),
			[typeof(string)] = (self) => sbyte.TryParse(self.ReadString(), out sbyte result) ? result : default,
			[typeof(decimal)] = (self) => (sbyte)self.ReadUnmanaged<decimal>(),
		};

		class Serialize : TreeDataSerializeRule<TreeDataByteSequence, sbyte>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				self.WriteType(typeof(sbyte));
				self.WriteUnmanaged((sbyte)value);
			}
		}

		class Deserialize : TreeDataDeserializeRule<TreeDataByteSequence, sbyte>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value)
			{
				if (self.TryReadType(out Type type) && TypeDict.TryGetValue(type, out var func))
				{
					value = func(self);
				}
				else
				{
					self.SkipData(type);
				}
			}
		}
	}
}
