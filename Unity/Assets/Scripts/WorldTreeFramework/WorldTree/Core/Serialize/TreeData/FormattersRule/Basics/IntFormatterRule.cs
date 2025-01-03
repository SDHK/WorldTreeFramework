/****************************************

* 作者：闪电黑客
* 日期：2024/10/22 11:15

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class IntFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, int>> typeDict = new()
		{
			[typeof(bool)] = (self) => (self.ReadUnmanaged<bool>() ? 1 : 0),
			[typeof(byte)] = (self) => self.ReadUnmanaged<byte>(),
			[typeof(sbyte)] = (self) => self.ReadUnmanaged<sbyte>(),
			[typeof(short)] = (self) => self.ReadUnmanaged<short>(),
			[typeof(ushort)] = (self) => self.ReadUnmanaged<ushort>(),
			[typeof(int)] = (self) => self.ReadUnmanaged<int>(),
			[typeof(uint)] = (self) => (int)self.ReadUnmanaged<uint>(),
			[typeof(long)] = (self) => (int)self.ReadUnmanaged<long>(),
			[typeof(ulong)] = (self) => (int)self.ReadUnmanaged<ulong>(),
			[typeof(float)] = (self) => (int)self.ReadUnmanaged<float>(),
			[typeof(double)] = (self) => (int)self.ReadUnmanaged<double>(),
			[typeof(char)] = (self) => self.ReadUnmanaged<char>(),
			[typeof(string)] = (self) => int.TryParse(self.ReadString(), out int result) ? result : default,
			[typeof(decimal)] = (self) => (int)self.ReadUnmanaged<decimal>(),
		};

		class Serialize : TreeDataSerializeRule<int>
		{
			protected override void Execute(TreeDataByteSequence self, ref object obj, ref int nameCode)
			{
				self.WriteType(typeof(int));
				self.WriteUnmanaged((int)obj);
			}
		}

		class Deserialize : TreeDataDeserializeRule<int>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object obj, ref int nameCode)
			{
				if (self.TryReadType(out Type dataType) && typeDict.TryGetValue(dataType, out var func))
					obj = func(self);
				else
					self.SkipData(dataType);
			}
		}
	}
}
