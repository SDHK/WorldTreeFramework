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
				self.WriteUnmanaged(1);
				self.WriteValue(obj.Item1);
			}
		}
		class Deserialize<T> : TreeDataDeserializeRule<ValueTuple<T>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (self.TryReadArrayHead(typeof(ValueTuple<T>), ref value, 1)) return;
				self.ReadUnmanaged(out int _);
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
				self.WriteUnmanaged(2);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
			}
		}

		class Deserialize<T1, T2> : TreeDataDeserializeRule<ValueTuple<T1, T2>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(ValueTuple<T1, T2>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = default;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(ValueTuple<T1, T2, T3>));
				if (value is not ValueTuple<T1, T2, T3> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 3);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
			}
		}

		class Deserialize<T1, T2, T3> : TreeDataDeserializeRule<ValueTuple<T1, T2, T3>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(ValueTuple<T1, T2, T3>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = default;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(ValueTuple<T1, T2, T3, T4>));
				if (value is not ValueTuple<T1, T2, T3, T4> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 4);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
				self.WriteValue(obj.Item4);
			}
		}

		class Deserialize<T1, T2, T3, T4> : TreeDataDeserializeRule<ValueTuple<T1, T2, T3, T4>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(ValueTuple<T1, T2, T3, T4>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = default;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(ValueTuple<T1, T2, T3, T4, T5>));
				if (value is not ValueTuple<T1, T2, T3, T4, T5> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 5);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
				self.WriteValue(obj.Item4);
				self.WriteValue(obj.Item5);
			}
		}

		class Deserialize<T1, T2, T3, T4, T5> : TreeDataDeserializeRule<ValueTuple<T1, T2, T3, T4, T5>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(ValueTuple<T1, T2, T3, T4, T5>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = default;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(ValueTuple<T1, T2, T3, T4, T5, T6>));
				if (value is not ValueTuple<T1, T2, T3, T4, T5, T6> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 6);
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
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(ValueTuple<T1, T2, T3, T4, T5, T6>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = default;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(ValueTuple<T1, T2, T3, T4, T5, T6, T7>));
				if (value is not ValueTuple<T1, T2, T3, T4, T5, T6, T7> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 7);
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
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(ValueTuple<T1, T2, T3, T4, T5, T6, T7>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = default;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>));
				if (value is not ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 8);
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
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = default;
					return;
				}
				self.ReadUnmanaged(out int _);
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
