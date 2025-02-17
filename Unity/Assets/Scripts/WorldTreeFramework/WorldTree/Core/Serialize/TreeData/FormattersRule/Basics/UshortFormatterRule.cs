/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 17:26

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class UshortFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, ushort>> typeDict = new()
		{
			[typeof(bool)] = (self) => (ushort)(self.ReadUnmanaged<bool>() ? 1 : 0),
			[typeof(byte)] = (self) => self.ReadUnmanaged<byte>(),
			[typeof(sbyte)] = (self) => (ushort)self.ReadUnmanaged<sbyte>(),
			[typeof(short)] = (self) => (ushort)self.ReadUnmanaged<short>(),
			[typeof(ushort)] = (self) => self.ReadUnmanaged<ushort>(),
			[typeof(int)] = (self) => (ushort)self.ReadUnmanaged<int>(),
			[typeof(uint)] = (self) => (ushort)self.ReadUnmanaged<uint>(),
			[typeof(long)] = (self) => (ushort)self.ReadUnmanaged<long>(),
			[typeof(ulong)] = (self) => (ushort)self.ReadUnmanaged<ulong>(),
			[typeof(float)] = (self) => (ushort)self.ReadUnmanaged<float>(),
			[typeof(double)] = (self) => (ushort)self.ReadUnmanaged<double>(),
			[typeof(char)] = (self) => self.ReadUnmanaged<char>(),
			[typeof(string)] = (self) => ushort.TryParse(self.ReadString(), out ushort result) ? result : default,
			[typeof(decimal)] = (self) => (ushort)self.ReadUnmanaged<decimal>(),
		};

		class Serialize : TreeDataSerializeRule<ushort>
		{
			protected override void Execute(TreeDataByteSequence self, ref object obj, ref SerializedTypeMode typeMode)
			{
				if (typeMode != SerializedTypeMode.Value) self.WriteType(typeof(ushort));
				self.WriteUnmanaged((ushort)obj);
			}
		}

		class Deserialize : TreeDataDeserializeRule<ushort>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object obj, ref int fieldNameCode)
			{
				if (fieldNameCode != TreeDataCode.DESERIALIZE_SELF_MODE) { obj = self.ReadUnmanaged<ushort>(); return; }
				if (self.TryReadType(out Type dataType) && typeDict.TryGetValue(dataType, out var func))
					obj = func(self);
				else
					self.SkipData(dataType);
			}
		}
	}
}
