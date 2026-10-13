/****************************************

* 作者：闪电黑客
* 日期：2024/11/7 19:44

* 描述：

*/
using System;

namespace WorldTree.TreeDataFormatters
{
	public static class ValueTuple1FormatterRule
	{
		class Serialize<T> : TreeDataSerializeRule<ValueTuple<T>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out ValueTuple<T> obj)) return;
				self.WriteDynamic(1);
				self.WriteValue(obj.Item1);
			}
		}
		class Deserialize<T> : TreeDataDeserializeRule<ValueTuple<T>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(ValueTuple<T>), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				ValueTuple<T> obj = (ValueTuple<T>)value;
				self.ReadValue(ref obj.Item1);
				value = obj;
			}
		}

	}

	public static class ValueTuple2FormatterRule
	{
		class Serialize<T1, T2> : TreeDataSerializeRule<ValueTuple<T1, T2>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out ValueTuple<T1, T2> obj)) return;
				self.WriteDynamic(2);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
			}
		}

		class Deserialize<T1, T2> : TreeDataDeserializeRule<ValueTuple<T1, T2>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(ValueTuple<T1, T2>), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				ValueTuple<T1, T2> obj = (ValueTuple<T1, T2>)value;
				self.ReadValue(ref obj.Item1);
				self.ReadValue(ref obj.Item2);
				value = obj;
			}
		}
	}

	public static class ValueTuple3FormatterRule
	{
		class Serialize<T1, T2, T3> : TreeDataSerializeRule<ValueTuple<T1, T2, T3>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out ValueTuple<T1, T2, T3> obj)) return;
				self.WriteDynamic(3);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
			}
		}

		class Deserialize<T1, T2, T3> : TreeDataDeserializeRule<ValueTuple<T1, T2, T3>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(ValueTuple<T1, T2, T3>), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				ValueTuple<T1, T2, T3> obj = (ValueTuple<T1, T2, T3>)value;
				self.ReadValue(ref obj.Item1);
				self.ReadValue(ref obj.Item2);
				self.ReadValue(ref obj.Item3);
				value = obj;
			}
		}
	}

	public static class ValueTuple4FormatterRule
	{
		class Serialize<T1, T2, T3, T4> : TreeDataSerializeRule<ValueTuple<T1, T2, T3, T4>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out ValueTuple<T1, T2, T3, T4> obj)) return;
				self.WriteDynamic(4);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
				self.WriteValue(obj.Item4);
			}
		}

		class Deserialize<T1, T2, T3, T4> : TreeDataDeserializeRule<ValueTuple<T1, T2, T3, T4>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(ValueTuple<T1, T2, T3, T4>), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				ValueTuple<T1, T2, T3, T4> obj = (ValueTuple<T1, T2, T3, T4>)value;
				self.ReadValue(ref obj.Item1);
				self.ReadValue(ref obj.Item2);
				self.ReadValue(ref obj.Item3);
				self.ReadValue(ref obj.Item4);
				value = obj;
			}
		}
	}

	public static class ValueTuple5FormatterRule
	{
		class Serialize<T1, T2, T3, T4, T5> : TreeDataSerializeRule<ValueTuple<T1, T2, T3, T4, T5>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out ValueTuple<T1, T2, T3, T4, T5> obj)) return;
				self.WriteDynamic(5);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
				self.WriteValue(obj.Item4);
				self.WriteValue(obj.Item5);
			}
		}

		class Deserialize<T1, T2, T3, T4, T5> : TreeDataDeserializeRule<ValueTuple<T1, T2, T3, T4, T5>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(ValueTuple<T1, T2, T3, T4, T5>), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				ValueTuple<T1, T2, T3, T4, T5> obj = (ValueTuple<T1, T2, T3, T4, T5>)value;
				self.ReadValue(ref obj.Item1);
				self.ReadValue(ref obj.Item2);
				self.ReadValue(ref obj.Item3);
				self.ReadValue(ref obj.Item4);
				self.ReadValue(ref obj.Item5);
				value = obj;
			}
		}
	}

	public static class ValueTuple6FormatterRule
	{
		class Serialize<T1, T2, T3, T4, T5, T6> : TreeDataSerializeRule<ValueTuple<T1, T2, T3, T4, T5, T6>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out ValueTuple<T1, T2, T3, T4, T5, T6> obj)) return;
				self.WriteDynamic(6);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
				self.WriteValue(obj.Item4);
				self.WriteValue(obj.Item5);
				self.WriteValue(obj.Item6);
			}
		}

		class Deserialize<T1, T2, T3, T4, T5, T6> : TreeDataDeserializeRule<ValueTuple<T1, T2, T3, T4, T5, T6>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(ValueTuple<T1, T2, T3, T4, T5, T6>), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				ValueTuple<T1, T2, T3, T4, T5, T6> obj = (ValueTuple<T1, T2, T3, T4, T5, T6>)value;
				self.ReadValue(ref obj.Item1);
				self.ReadValue(ref obj.Item2);
				self.ReadValue(ref obj.Item3);
				self.ReadValue(ref obj.Item4);
				self.ReadValue(ref obj.Item5);
				self.ReadValue(ref obj.Item6);
				value = obj;
			}
		}
	}

	public static class ValueTuple7FormatterRule
	{
		class Serialize<T1, T2, T3, T4, T5, T6, T7> : TreeDataSerializeRule<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out ValueTuple<T1, T2, T3, T4, T5, T6, T7> obj)) return;
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

		class Deserialize<T1, T2, T3, T4, T5, T6, T7> : TreeDataDeserializeRule<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(ValueTuple<T1, T2, T3, T4, T5, T6, T7>), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				ValueTuple<T1, T2, T3, T4, T5, T6, T7> obj = (ValueTuple<T1, T2, T3, T4, T5, T6, T7>)value;
				self.ReadValue(ref obj.Item1);
				self.ReadValue(ref obj.Item2);
				self.ReadValue(ref obj.Item3);
				self.ReadValue(ref obj.Item4);
				self.ReadValue(ref obj.Item5);
				self.ReadValue(ref obj.Item6);
				self.ReadValue(ref obj.Item7);
				value = obj;
			}
		}
	}

	public static class ValueTuple8FormatterRule
	{
		class Serialize<T1, T2, T3, T4, T5, T6, T7, TRest> : TreeDataSerializeRule<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
			where TRest : struct
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> obj)) return;
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

		class Deserialize<T1, T2, T3, T4, T5, T6, T7, TRest> : TreeDataDeserializeRule<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
			where TRest : struct
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>), ref value, 1, out _, out _)) return;
				ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> obj = (ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)value;
				self.ReadValue(ref obj.Item1);
				self.ReadValue(ref obj.Item2);
				self.ReadValue(ref obj.Item3);
				self.ReadValue(ref obj.Item4);
				self.ReadValue(ref obj.Item5);
				self.ReadValue(ref obj.Item6);
				self.ReadValue(ref obj.Item7);
				self.ReadValue(ref obj.Rest);
				value = obj;
			}
		}
	}

}
