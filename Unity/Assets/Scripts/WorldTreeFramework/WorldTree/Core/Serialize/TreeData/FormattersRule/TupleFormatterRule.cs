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
				self.WriteType(typeof(Tuple<T>));
				if (value is not Tuple<T> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 1);
				self.WriteValue(obj.Item1);
			}
		}
		class Deserialize<T> : TreeDataDeserializeRule<Tuple<T>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(Tuple<T>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
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
				self.WriteType(typeof(Tuple<T1, T2>));
				if (value is not Tuple<T1, T2> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 2);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
			}
		}
		class Deserialize<T1, T2> : TreeDataDeserializeRule<Tuple<T1, T2>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(Tuple<T1, T2>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(Tuple<T1, T2, T3>));
				if (value is not Tuple<T1, T2, T3> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 3);
				self.WriteValue(obj.Item1);
				self.WriteValue(obj.Item2);
				self.WriteValue(obj.Item3);
			}
		}
		class Deserialize<T1, T2, T3> : TreeDataDeserializeRule<Tuple<T1, T2, T3>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(Tuple<T1, T2, T3>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(Tuple<T1, T2, T3, T4>));
				if (value is not Tuple<T1, T2, T3, T4> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 4);
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
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(Tuple<T1, T2, T3, T4>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(Tuple<T1, T2, T3, T4, T5>));
				if (value is not Tuple<T1, T2, T3, T4, T5> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 5);
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
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(Tuple<T1, T2, T3, T4, T5>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(Tuple<T1, T2, T3, T4, T5, T6>));
				if (value is not Tuple<T1, T2, T3, T4, T5, T6> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
				self.WriteUnmanaged(~1, 6);
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
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(Tuple<T1, T2, T3, T4, T5, T6>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(Tuple<T1, T2, T3, T4, T5, T6, T7>));
				if (value is not Tuple<T1, T2, T3, T4, T5, T6, T7> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
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
		class Deserialize<T1, T2, T3, T4, T5, T6, T7> : TreeDataDeserializeRule<Tuple<T1, T2, T3, T4, T5, T6, T7>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(Tuple<T1, T2, T3, T4, T5, T6, T7>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				self.ReadUnmanaged(out int _);
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
				self.WriteType(typeof(Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>));
				if (value is not Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> obj) { self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT); return; }
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
		class Deserialize<T1, T2, T3, T4, T5, T6, T7, TRest> : TreeDataDeserializeRule<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>)))
				{
					self.SkipData(dataType);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				self.ReadUnmanaged(out int _);
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
