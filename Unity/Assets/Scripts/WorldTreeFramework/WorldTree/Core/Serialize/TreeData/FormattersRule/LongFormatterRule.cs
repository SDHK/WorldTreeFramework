/****************************************

* 作者：闪电黑客
* 日期：2024/10/22 11:15

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class LongFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, long>> TypeDict = new()
		{
			[typeof(bool)] = (self) => (long)(self.ReadUnmanaged<bool>() ? 1 : 0),
			[typeof(byte)] = (self) => self.ReadUnmanaged<byte>(),
			[typeof(sbyte)] = (self) => self.ReadUnmanaged<sbyte>(),
			[typeof(short)] = (self) => self.ReadUnmanaged<short>(),
			[typeof(ushort)] = (self) => self.ReadUnmanaged<ushort>(),
			[typeof(int)] = (self) => self.ReadUnmanaged<int>(),
			[typeof(uint)] = (self) => self.ReadUnmanaged<uint>(),
			[typeof(long)] = (self) => self.ReadUnmanaged<long>(),
			[typeof(ulong)] = (self) => (long)self.ReadUnmanaged<ulong>(),
			[typeof(float)] = (self) => (long)self.ReadUnmanaged<float>(),
			[typeof(double)] = (self) => (long)self.ReadUnmanaged<double>(),
			[typeof(char)] = (self) => self.ReadUnmanaged<char>(),
			[typeof(string)] = (self) => long.TryParse(self.ReadString(), out long result) ? result : default,
			[typeof(decimal)] = (self) => (long)self.ReadUnmanaged<decimal>(),
		};

		class Serialize : TreeDataSerializeRule<TreeDataByteSequence, long>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				self.WriteType(typeof(long));
				self.WriteUnmanaged((long)value);
			}
		}

		class Deserialize : TreeDataDeserializeRule<TreeDataByteSequence, long>
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
