/****************************************

* 作者：闪电黑客
* 日期：2024/10/22 11:16

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class ShortFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, short>> TypeDict = new()
		{
			[typeof(bool)] = (self) => (short)(self.ReadUnmanaged<bool>() ? 1 : 0),
			[typeof(byte)] = (self) => (short)self.ReadUnmanaged<byte>(),
			[typeof(sbyte)] = (self) => (short)self.ReadUnmanaged<sbyte>(),
			[typeof(short)] = (self) => self.ReadUnmanaged<short>(),
			[typeof(ushort)] = (self) => (short)self.ReadUnmanaged<ushort>(),
			[typeof(int)] = (self) => (short)self.ReadUnmanaged<int>(),
			[typeof(uint)] = (self) => (short)self.ReadUnmanaged<uint>(),
			[typeof(long)] = (self) => (short)self.ReadUnmanaged<long>(),
			[typeof(ulong)] = (self) => (short)self.ReadUnmanaged<ulong>(),
			[typeof(float)] = (self) => (short)self.ReadUnmanaged<float>(),
			[typeof(double)] = (self) => (short)self.ReadUnmanaged<double>(),
			[typeof(char)] = (self) => (short)self.ReadUnmanaged<char>(),
			[typeof(string)] = (self) => short.TryParse(self.ReadString(), out short result) ? result : default,
			[typeof(decimal)] = (self) => (short)self.ReadUnmanaged<decimal>(),
		};

		class Serialize : TreeDataSerializeRule<TreeDataByteSequence, short>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				self.WriteType(typeof(short));
				self.WriteUnmanaged((short)value);
			}
		}

		class Deserialize : TreeDataDeserializeRule<TreeDataByteSequence, short>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value)
			{
				if (self.TryReadType(out Type type) && TypeDict.TryGetValue(type, out var func))
					value = func(self);
				else
					self.SkipData(type);
			}
		}
	}
}
