/****************************************

* 作者：闪电黑客
* 日期：2024/10/22 11:15

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class UlongFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, ulong>> TypeDict = new()
		{
			[typeof(bool)] = (self) => (ulong)(self.ReadUnmanaged<bool>() ? 1 : 0),
			[typeof(byte)] = (self) => self.ReadUnmanaged<byte>(),
			[typeof(sbyte)] = (self) => (ulong)self.ReadUnmanaged<sbyte>(),
			[typeof(short)] = (self) => (ulong)self.ReadUnmanaged<short>(),
			[typeof(ushort)] = (self) => self.ReadUnmanaged<ushort>(),
			[typeof(int)] = (self) => (ulong)self.ReadUnmanaged<int>(),
			[typeof(uint)] = (self) => self.ReadUnmanaged<uint>(),
			[typeof(long)] = (self) => (ulong)self.ReadUnmanaged<long>(),
			[typeof(ulong)] = (self) => self.ReadUnmanaged<ulong>(),
			[typeof(float)] = (self) => (ulong)self.ReadUnmanaged<float>(),
			[typeof(double)] = (self) => (ulong)self.ReadUnmanaged<double>(),
			[typeof(char)] = (self) => self.ReadUnmanaged<char>(),
			[typeof(string)] = (self) => ulong.TryParse(self.ReadString(), out ulong result) ? result : default,
			[typeof(decimal)] = (self) => (ulong)self.ReadUnmanaged<decimal>(),
		};

		class Serialize : TreeDataSerializeRule<ulong>
		{
			protected override void Execute(TreeDataByteSequence self, ref object obj)
			{
				self.WriteType(typeof(ulong));
				self.WriteUnmanaged((ulong)obj);
			}
		}

		class Deserialize : TreeDataDeserializeRule<ulong>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object obj)
			{
				if (self.TryReadType(out Type dataType) && TypeDict.TryGetValue(dataType, out var func))
					obj = func(self);
				else
					self.SkipData(dataType);
			}
		}
	}
}
