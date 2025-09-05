using System;

namespace VM
{
	/// <summary>
	/// 变量类型
	/// </summary>
	public enum VarType : byte
	{
		/// <summary>
		/// 整数类型
		/// </summary>
		Long,
		/// <summary>
		/// 浮点数类型
		/// </summary>
		Double,
		/// <summary>
		/// 布尔类型
		/// </summary>
		Bool,
		/// <summary>
		/// 字符串类型
		/// </summary>
		String,

		/// <summary>
		/// 对象类型
		/// </summary>
		Object,
	}


	/// <summary>
	/// 优化的变量值（模仿Lua TValue）
	/// </summary>
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
	public struct VarValue
	{
		[System.Runtime.InteropServices.FieldOffset(0)]
		public VarType Type;

		// 值联合体（8字节）
		[System.Runtime.InteropServices.FieldOffset(8)]
		public long LongValue;

		[System.Runtime.InteropServices.FieldOffset(8)]
		public double DoubleValue;

		[System.Runtime.InteropServices.FieldOffset(8)]
		public bool BoolValue;

		// 对象引用（字符串等）
		[System.Runtime.InteropServices.FieldOffset(8)]
		public object ObjectValue;


		// 隐式转换运算符
		public static implicit operator VarValue(int value) => new VarValue { Type = VarType.Long, LongValue = value };
		public static implicit operator VarValue(long value) => new VarValue { Type = VarType.Long, LongValue = value };
		public static implicit operator VarValue(float value) => new VarValue { Type = VarType.Double, DoubleValue = value };
		public static implicit operator VarValue(double value) => new VarValue { Type = VarType.Double, DoubleValue = value };
		public static implicit operator VarValue(bool value) => new VarValue { Type = VarType.Bool, BoolValue = value };
		public static implicit operator VarValue(string value) => new VarValue { Type = VarType.String, ObjectValue = value };

		// 显式转换运算符
		public static explicit operator int(VarValue value) => value.ToInt();
		public static explicit operator long(VarValue value) => value.ToLong();
		public static explicit operator float(VarValue value) => value.ToFloat();
		public static explicit operator double(VarValue value) => value.ToDouble();
		public static explicit operator bool(VarValue value) => value.ToBool();
		public static explicit operator string(VarValue value) => value.ToString();


		public int ToInt() => (int)ToLong();

		public long ToLong()
		{
			return Type switch
			{
				VarType.Long => LongValue,
				VarType.Double => (long)DoubleValue,
				VarType.Bool => BoolValue ? 1L : 0L,
				VarType.String when long.TryParse((string)ObjectValue, out var result) => result,
				_ => 0L
			};
		}

		public bool ToBool()
		{
			return Type switch
			{
				VarType.Bool => BoolValue,
				VarType.Long => LongValue != 0,
				VarType.Double => Math.Abs(DoubleValue) > 0.0001,
				VarType.String => !string.IsNullOrEmpty((string)ObjectValue),
				_ => ObjectValue != null
			};
		}
		public float ToFloat() => (float)ToDouble();

		public double ToDouble()
		{
			return Type switch
			{
				VarType.Long => LongValue,
				VarType.Double => DoubleValue,
				VarType.Bool => BoolValue ? 1.0 : 0.0,
				VarType.String when double.TryParse((string)ObjectValue, out var result) => result,
				_ => double.NaN
			};
		}

		public override string ToString()
		{
			return Type switch
			{
				VarType.Long => LongValue.ToString(),
				VarType.Double => DoubleValue.ToString(),
				VarType.Bool => BoolValue ? "true" : "false",
				VarType.String => (string)ObjectValue ?? "",
				_ => ObjectValue?.ToString() ?? "null"
			};
		}
	}
}
