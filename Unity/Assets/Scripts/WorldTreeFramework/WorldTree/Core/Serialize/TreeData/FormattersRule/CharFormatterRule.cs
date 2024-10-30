/****************************************

* 作者：闪电黑客
* 日期：2024/10/22 11:15

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class CharFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, char>> TypeDict = new()
		{
			[typeof(bool)] = (self) => (char)(self.ReadUnmanaged<bool>() ? 1 : 0),
			[typeof(byte)] = (self) => (char)self.ReadUnmanaged<byte>(),
			[typeof(sbyte)] = (self) => (char)self.ReadUnmanaged<sbyte>(),
			[typeof(short)] = (self) => (char)self.ReadUnmanaged<short>(),
			[typeof(ushort)] = (self) => (char)self.ReadUnmanaged<ushort>(),
			[typeof(int)] = (self) => (char)self.ReadUnmanaged<int>(),
			[typeof(uint)] = (self) => (char)self.ReadUnmanaged<uint>(),
			[typeof(long)] = (self) => (char)self.ReadUnmanaged<long>(),
			[typeof(ulong)] = (self) => (char)self.ReadUnmanaged<ulong>(),
			[typeof(float)] = (self) => (char)self.ReadUnmanaged<float>(),
			[typeof(double)] = (self) => (char)self.ReadUnmanaged<double>(),
			[typeof(char)] = (self) => self.ReadUnmanaged<char>(),
			[typeof(string)] = (self) => char.TryParse(self.ReadString(), out char result) ? result : default,
			[typeof(decimal)] = (self) => (char)self.ReadUnmanaged<decimal>(),
		};

		class Serialize : TreeDataSerializeRule<char>
		{
			protected override void Execute(TreeDataByteSequence self, ref object obj, ref int nameCode)
			{
				self.WriteType(typeof(char));
				self.WriteUnmanaged((char)obj);
			}
		}

		class Deserialize : TreeDataDeserializeRule<char>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object obj, ref int nameCode)
			{
				if (self.TryReadType(out Type dataType) && TypeDict.TryGetValue(dataType, out var func))
					obj = func(self);
				else
					self.SkipData(dataType);
			}
		}
	}
}
