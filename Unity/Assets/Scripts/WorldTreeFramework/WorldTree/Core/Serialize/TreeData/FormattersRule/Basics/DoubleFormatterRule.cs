﻿/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 17:26

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class DoubleFormatterRule
	{
		private static Dictionary<Type, Func<TreeDataByteSequence, double>> typeDict = new()
		{
			[typeof(bool)] = (self) => (self.ReadUnmanaged<bool>() ? 1 : 0),
			[typeof(byte)] = (self) => self.ReadUnmanaged<byte>(),
			[typeof(sbyte)] = (self) => self.ReadUnmanaged<sbyte>(),
			[typeof(short)] = (self) => self.ReadUnmanaged<short>(),
			[typeof(ushort)] = (self) => self.ReadUnmanaged<ushort>(),
			[typeof(int)] = (self) => self.ReadUnmanaged<int>(),
			[typeof(uint)] = (self) => self.ReadUnmanaged<uint>(),
			[typeof(long)] = (self) => self.ReadUnmanaged<long>(),
			[typeof(ulong)] = (self) => self.ReadUnmanaged<ulong>(),
			[typeof(float)] = (self) => self.ReadUnmanaged<float>(),
			[typeof(double)] = (self) => self.ReadUnmanaged<double>(),
			[typeof(char)] = (self) => self.ReadUnmanaged<char>(),
			[typeof(string)] = (self) => double.TryParse(self.ReadString(), out double result) ? result : default,
			[typeof(decimal)] = (self) => (double)self.ReadUnmanaged<decimal>(),
		};

		class Serialize : TreeDataSerializeRule<double>
		{
			protected override void Execute(TreeDataByteSequence self, ref object obj, ref SerializedTypeMode typeMode)
			{
				if (typeMode != SerializedTypeMode.Value) self.WriteType(typeof(double));
				self.WriteUnmanaged((double)obj);
			}
		}

		class Deserialize : TreeDataDeserializeRule<double>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object obj, ref int fieldNameCode)
			{
				if (fieldNameCode != TreeDataCode.DESERIALIZE_SELF_MODE) { obj = self.ReadUnmanaged<double>(); return; }
				if (self.TryReadType(out Type dataType) && typeDict.TryGetValue(dataType, out var func))
					obj = func(self);
				else
					self.SkipData(dataType);
			}
		}
	}
}
