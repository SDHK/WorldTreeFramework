﻿/****************************************

* 作者：闪电黑客
* 日期：2024/10/22 11:15

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class UintFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, uint>> TypeDict = new()
		{
			[typeof(bool)] = (self) => (uint)(self.ReadUnmanaged<bool>() ? 1 : 0),
			[typeof(byte)] = (self) => self.ReadUnmanaged<byte>(),
			[typeof(sbyte)] = (self) => (uint)self.ReadUnmanaged<sbyte>(),
			[typeof(short)] = (self) => (uint)self.ReadUnmanaged<short>(),
			[typeof(ushort)] = (self) => self.ReadUnmanaged<ushort>(),
			[typeof(int)] = (self) => (uint)self.ReadUnmanaged<int>(),
			[typeof(uint)] = (self) => self.ReadUnmanaged<uint>(),
			[typeof(long)] = (self) => (uint)self.ReadUnmanaged<long>(),
			[typeof(ulong)] = (self) => (uint)self.ReadUnmanaged<ulong>(),
			[typeof(float)] = (self) => (uint)self.ReadUnmanaged<float>(),
			[typeof(double)] = (self) => (uint)self.ReadUnmanaged<double>(),
			[typeof(char)] = (self) => self.ReadUnmanaged<char>(),
			[typeof(string)] = (self) => uint.TryParse(self.ReadString(), out uint result) ? result : default,
			[typeof(decimal)] = (self) => (uint)self.ReadUnmanaged<decimal>(),
		};

		class Serialize : TreeDataSerializeRule<uint>
		{
			protected override void Execute(TreeDataByteSequence self, ref object obj, ref int nameCode)
			{
				self.WriteType(typeof(uint));
				self.WriteUnmanaged((uint)obj);
			}
		}

		class Deserialize : TreeDataDeserializeRule<uint>
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