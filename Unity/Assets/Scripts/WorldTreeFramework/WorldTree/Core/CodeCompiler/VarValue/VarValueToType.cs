using System;

namespace WorldTree
{
	public partial struct VarValue
	{
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
		public static explicit operator char(VarValue value) => value.ToChar();
		public static explicit operator string(VarValue value) => value.ToString();

		/// <summary>
		/// 转换为整数 
		/// </summary>
		/// <returns></returns>
		public int ToInt() => (int)ToLong();
		/// <summary>
		/// 转换为字符 
		/// </summary>
		public char ToChar()
		{
			return Type switch
			{
				VarType.Long => (char)LongValue,
				VarType.Double => (char)DoubleValue,
				VarType.Bool => BoolValue ? '1' : '0',
				VarType.String when !string.IsNullOrEmpty((string)ObjectValue) => ((string)ObjectValue)[0],
				_ => '\0'
			};
		}
		/// <summary>
		/// 转换为长整数 
		/// </summary>
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
		/// <summary>
		/// 转换为布尔值 
		/// </summary>
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
		/// <summary>
		/// 转换为浮点数 
		/// </summary>
		public float ToFloat() => (float)ToDouble();
		/// <summary>
		/// 转换为双精度浮点数 
		/// </summary>
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
