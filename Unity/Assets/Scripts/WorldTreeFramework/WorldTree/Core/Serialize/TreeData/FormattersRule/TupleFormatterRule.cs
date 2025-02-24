/****************************************

* 作者：闪电黑客
* 日期：2024/11/7 20:03

* 描述：

*/
using System;

namespace WorldTree.TreeDataFormatters
{
	public static class Tuple1FormatterRule
	{
		class Serialize<T> : TreeDataSerializeRule<Tuple<T>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out Tuple<T> obj)) return;
				self.WriteDynamic(1);
				self.WriteValue(obj.Item1);
			}
		}
		class Deserialize<T> : TreeDataDeserializeRule<Tuple<T>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(Tuple<T>), ref value, 1, out int _)) return;
				self.ReadUnmanaged(out int _);
				value = new Tuple<T>(self.ReadValue<T>());
			}
		}
	}

	public static class Tuple2FormatterRule
	{
		class Serialize<T1, T2> : TreeDataSerializeRule<Tuple<T1, T2>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out Tuple<T1, T2> obj)) return;
				self.WriteDynamic(2);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
			}
		}
		class Deserialize<T1, T2> : TreeDataDeserializeRule<Tuple<T1, T2>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(Tuple<T1, T2>), ref value, 1, out int _)) return;
				self.ReadDynamic(out int _);
				value = new Tuple<T1, T2>(
					self.ReadValue<T1>(),
					self.ReadValue<T2>()
				);
			}
		}
	}

	public static class Tuple3FormatterRule
	{
		class Serialize<T1, T2, T3> : TreeDataSerializeRule<Tuple<T1, T2, T3>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out Tuple<T1, T2, T3> obj)) return;
				self.WriteDynamic(3);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
			}
		}
		class Deserialize<T1, T2, T3> : TreeDataDeserializeRule<Tuple<T1, T2, T3>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(Tuple<T1, T2, T3>), ref value, 1, out int _)) return;
				self.ReadDynamic(out int _);
				value = new Tuple<T1, T2, T3>(
					self.ReadValue<T1>(),
					self.ReadValue<T2>(),
					self.ReadValue<T3>()
				);
			}
		}
	}

	public static class Tuple4FormatterRule
	{
		class Serialize<T1, T2, T3, T4> : TreeDataSerializeRule<Tuple<T1, T2, T3, T4>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out Tuple<T1, T2, T3, T4> obj)) return;
				self.WriteDynamic(4);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
				self.WriteValue(obj.Item4);
			}
		}
		class Deserialize<T1, T2, T3, T4> : TreeDataDeserializeRule<Tuple<T1, T2, T3, T4>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(Tuple<T1, T2, T3, T4>), ref value, 1, out int _)) return;
				self.ReadDynamic(out int _);
				value = new Tuple<T1, T2, T3, T4>(
					self.ReadValue<T1>(),
					self.ReadValue<T2>(),
					self.ReadValue<T3>(),
					self.ReadValue<T4>()
				);
			}
		}
	}

	public static class Tuple5FormatterRule
	{
		class Serialize<T1, T2, T3, T4, T5> : TreeDataSerializeRule<Tuple<T1, T2, T3, T4, T5>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out Tuple<T1, T2, T3, T4, T5> obj)) return;
				self.WriteDynamic(5);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
				self.WriteValue(obj.Item4);
				self.WriteValue(obj.Item5);
			}
		}
		class Deserialize<T1, T2, T3, T4, T5> : TreeDataDeserializeRule<Tuple<T1, T2, T3, T4, T5>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(Tuple<T1, T2, T3, T4, T5>), ref value, 1, out int _)) return;
				self.ReadDynamic(out int _);
				value = new Tuple<T1, T2, T3, T4, T5>(
					self.ReadValue<T1>(),
					self.ReadValue<T2>(),
					self.ReadValue<T3>(),
					self.ReadValue<T4>(),
					self.ReadValue<T5>()
				);
			}
		}
	}

	public static class Tuple6FormatterRule
	{
		class Serialize<T1, T2, T3, T4, T5, T6> : TreeDataSerializeRule<Tuple<T1, T2, T3, T4, T5, T6>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out Tuple<T1, T2, T3, T4, T5, T6> obj)) return;
				self.WriteDynamic(6);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
				self.WriteValue(obj.Item4);
				self.WriteValue(obj.Item5);
				self.WriteValue(obj.Item6);
			}
		}
		class Deserialize<T1, T2, T3, T4, T5, T6> : TreeDataDeserializeRule<Tuple<T1, T2, T3, T4, T5, T6>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(Tuple<T1, T2, T3, T4, T5, T6>), ref value, 1, out int _)) return;
				self.ReadDynamic(out int _);
				value = new Tuple<T1, T2, T3, T4, T5, T6>(
					self.ReadValue<T1>(),
					self.ReadValue<T2>(),
					self.ReadValue<T3>(),
					self.ReadValue<T4>(),
					self.ReadValue<T5>(),
					self.ReadValue<T6>()
				);
			}
		}
	}

	public static class Tuple7FormatterRule
	{
		class Serialize<T1, T2, T3, T4, T5, T6, T7> : TreeDataSerializeRule<Tuple<T1, T2, T3, T4, T5, T6, T7>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out Tuple<T1, T2, T3, T4, T5, T6, T7> obj)) return;
				self.WriteDynamic(7);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
				self.WriteValue(obj.Item4);
				self.WriteValue(obj.Item5);
				self.WriteValue(obj.Item6);
				self.WriteValue(obj.Item7);
			}
		}
		class Deserialize<T1, T2, T3, T4, T5, T6, T7> : TreeDataDeserializeRule<Tuple<T1, T2, T3, T4, T5, T6, T7>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(Tuple<T1, T2, T3, T4, T5, T6, T7>), ref value, 1, out int _)) return;
				self.ReadDynamic(out int _);
				value = new Tuple<T1, T2, T3, T4, T5, T6, T7>(
					self.ReadValue<T1>(),
					self.ReadValue<T2>(),
					self.ReadValue<T3>(),
					self.ReadValue<T4>(),
					self.ReadValue<T5>(),
					self.ReadValue<T6>(),
					self.ReadValue<T7>()
				);
			}
		}
	}

	public static class Tuple8FormatterRule
	{
		class Serialize<T1, T2, T3, T4, T5, T6, T7, TRest> : TreeDataSerializeRule<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> obj)) return;
				self.WriteDynamic(8);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
				self.WriteValue(obj.Item4);
				self.WriteValue(obj.Item5);
				self.WriteValue(obj.Item6);
				self.WriteValue(obj.Item7);
				self.WriteValue(obj.Rest);
			}
		}
		class Deserialize<T1, T2, T3, T4, T5, T6, T7, TRest> : TreeDataDeserializeRule<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>), ref value, 1, out int _)) return;
				self.ReadDynamic(out int _);
				value = new Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
					self.ReadValue<T1>(),
					self.ReadValue<T2>(),
					self.ReadValue<T3>(),
					self.ReadValue<T4>(),
					self.ReadValue<T5>(),
					self.ReadValue<T6>(),
					self.ReadValue<T7>(),
					self.ReadValue<TRest>()
				);
			}
		}
	}
}
