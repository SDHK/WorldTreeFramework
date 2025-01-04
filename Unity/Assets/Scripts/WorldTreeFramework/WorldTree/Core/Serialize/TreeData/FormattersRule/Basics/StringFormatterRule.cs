/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 17:25

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class StringFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, string>> typeDict = new()
		{
			[typeof(bool)] = (self) => self.ReadUnmanaged<bool>().ToString(),
			[typeof(byte)] = (self) => self.ReadUnmanaged<byte>().ToString(),
			[typeof(sbyte)] = (self) => self.ReadUnmanaged<sbyte>().ToString(),
			[typeof(short)] = (self) => self.ReadUnmanaged<short>().ToString(),
			[typeof(ushort)] = (self) => self.ReadUnmanaged<ushort>().ToString(),
			[typeof(int)] = (self) => self.ReadUnmanaged<int>().ToString(),
			[typeof(uint)] = (self) => self.ReadUnmanaged<uint>().ToString(),
			[typeof(long)] = (self) => self.ReadUnmanaged<long>().ToString(),
			[typeof(ulong)] = (self) => self.ReadUnmanaged<ulong>().ToString(),
			[typeof(float)] = (self) => self.ReadUnmanaged<float>().ToString(),
			[typeof(double)] = (self) => self.ReadUnmanaged<double>().ToString(),
			[typeof(char)] = (self) => self.ReadUnmanaged<char>().ToString(),
			[typeof(string)] = (self) => self.ReadString(),
			[typeof(decimal)] = (self) => self.ReadUnmanaged<decimal>().ToString(),
		};

		class Serialize : TreeDataSerializeRule<string>
		{
			protected override void Execute(TreeDataByteSequence self, ref object obj, ref int nameCode)
			{
				self.WriteType(typeof(string));
				self.WriteString((string)obj);
			}
		}

		class Deserialize : TreeDataDeserializeRule<string>
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
