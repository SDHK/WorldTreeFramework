/****************************************

* 作者：闪电黑客
* 日期：2024/10/22 10:31

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class BoolFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, bool>> typeDict = new()
		{
			[typeof(bool)] = (self) => self.ReadUnmanaged<bool>(),
			[typeof(byte)] = (self) => self.ReadUnmanaged<byte>() != 0,
			[typeof(sbyte)] = (self) => self.ReadUnmanaged<sbyte>() != 0,
			[typeof(short)] = (self) => self.ReadUnmanaged<short>() != 0,
			[typeof(ushort)] = (self) => self.ReadUnmanaged<ushort>() != 0,
			[typeof(int)] = (self) => self.ReadUnmanaged<int>() != 0,
			[typeof(uint)] = (self) => self.ReadUnmanaged<uint>() != 0,
			[typeof(long)] = (self) => self.ReadUnmanaged<long>() != 0,
			[typeof(ulong)] = (self) => self.ReadUnmanaged<ulong>() != 0,
			[typeof(float)] = (self) => self.ReadUnmanaged<float>() != 0,
			[typeof(double)] = (self) => self.ReadUnmanaged<double>() != 0,
			[typeof(char)] = (self) => self.ReadUnmanaged<char>() != '0',
			[typeof(string)] = (self) => bool.TryParse(self.ReadString(), out bool result) && result,
			[typeof(decimal)] = (self) => self.ReadUnmanaged<decimal>() != 0,
		};

		class Serialize : TreeDataSerializeRule<bool>
		{
			protected override void Execute(TreeDataByteSequence self, ref object obj, ref SerializedTypeMode typeMode)
			{
				self.WriteType(typeof(bool));
				self.WriteUnmanaged((bool)obj);
			}
		}
		class Deserialize : TreeDataDeserializeRule<bool>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object obj, ref int nameCode)
			{
				if (nameCode != -1) { obj = self.ReadUnmanaged<bool>(); return; }
				if (self.TryReadType(out Type dataType) && typeDict.TryGetValue(dataType, out var func))
					obj = func(self);
				else
					self.SkipData(dataType);
			}
		}
	}
}
