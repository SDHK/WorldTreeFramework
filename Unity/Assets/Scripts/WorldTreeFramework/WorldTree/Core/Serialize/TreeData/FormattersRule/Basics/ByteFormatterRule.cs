/****************************************

* 作者：闪电黑客
* 日期：2024/10/22 10:47

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class ByteFormatterRule
	{
		/// <summary>
		/// 转换表
		/// </summary>
		private static Dictionary<Type, Func<TreeDataByteSequence, byte>> TypeDict = new()
		{
			[typeof(bool)] = (self) => (byte)(self.ReadUnmanaged<bool>() ? 1 : 0),
			[typeof(byte)] = (self) => self.ReadUnmanaged<byte>(),
			[typeof(sbyte)] = (self) => (byte)self.ReadUnmanaged<sbyte>(),
			[typeof(short)] = (self) => (byte)self.ReadUnmanaged<short>(),
			[typeof(ushort)] = (self) => (byte)self.ReadUnmanaged<ushort>(),
			[typeof(int)] = (self) => (byte)self.ReadUnmanaged<int>(),
			[typeof(uint)] = (self) => (byte)self.ReadUnmanaged<uint>(),
			[typeof(long)] = (self) => (byte)self.ReadUnmanaged<long>(),
			[typeof(ulong)] = (self) => (byte)self.ReadUnmanaged<ulong>(),
			[typeof(float)] = (self) => (byte)self.ReadUnmanaged<float>(),
			[typeof(double)] = (self) => (byte)self.ReadUnmanaged<double>(),
			[typeof(char)] = (self) => (byte)self.ReadUnmanaged<char>(),
			[typeof(string)] = (self) => byte.TryParse(self.ReadString(), out byte result) ? result : (byte)0,
			[typeof(decimal)] = (self) => (byte)self.ReadUnmanaged<decimal>(),
		};

		class Serialize : TreeDataSerializeRule<byte>
		{
			protected override void Execute(TreeDataByteSequence self, ref object obj, ref int nameCode)
			{
				self.WriteType(typeof(byte));
				self.WriteUnmanaged((byte)obj);
			}
		}
		class Deserialize : TreeDataDeserializeRule<byte>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object obj, ref int nameCode)
			{
				if (self.TryReadType(out Type dataType) && TypeDict.TryGetValue(dataType, out var func))
				{
					obj = func(self);
				}
				else
				{
					self.SkipData(dataType);
				}
			}
		}
	}
}
