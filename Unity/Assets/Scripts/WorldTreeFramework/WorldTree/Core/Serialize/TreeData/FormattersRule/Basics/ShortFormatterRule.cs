/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 17:25

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class ShortFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, short>> typeDict = new()
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

		class Serialize : TreeDataSerializeRule<short>
		{
			protected override void Execute(TreeDataByteSequence self, ref object obj, ref SerializedTypeMode typeMode)
			{
				if (typeMode != SerializedTypeMode.Value) self.WriteType(typeof(short));
				self.WriteUnmanaged((short)obj);
			}
		}

		class Deserialize : TreeDataDeserializeRule<short>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object obj, ref int fieldNameCode)
			{
				if (fieldNameCode != TreeDataCode.DESERIALIZE_SELF_MODE) { obj = self.ReadUnmanaged<short>(); return; }
				if (self.TryReadType(out Type dataType) && typeDict.TryGetValue(dataType, out var func))
					obj = func(self);
				else
					self.SkipData(dataType);
			}
		}
	}
}
