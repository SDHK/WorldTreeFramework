using System;

namespace VM
{
	/// <summary>
	/// 联合类型变量值
	/// </summary>
	//[StructLayout(LayoutKind.Explicit)]
	public struct VarValue
	{
		//[FieldOffset(0)]
		public VarType Type;

		// 值联合体（8字节）
		//[FieldOffset(8)]
		public long LongValue;

		//[FieldOffset(8)]
		public double DoubleValue;

		//[FieldOffset(8)]
		public bool BoolValue;

		// 对象引用（字符串等）
		//[FieldOffset(8)]
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

		#region 算术运算符

		/// <summary>
		/// 加法运算符
		/// </summary>
		public static VarValue operator +(VarValue left, VarValue right)
		{
			// 字符串连接优先
			if (left.Type == VarType.String || right.Type == VarType.String)
			{
				return (VarValue)(left.ToString() + right.ToString());
			}

			// 数值运算 - 优先使用浮点数精度
			if (left.IsNumeric() && right.IsNumeric())
			{
				if (left.Type == VarType.Double || right.Type == VarType.Double)
				{
					return (VarValue)(left.ToDouble() + right.ToDouble());
				}
				else
				{
					return (VarValue)(left.ToLong() + right.ToLong());
				}
			}

			// 布尔值转数值运算
			if (left.Type == VarType.Bool) left = (VarValue)(left.BoolValue ? 1L : 0L);
			if (right.Type == VarType.Bool) right = (VarValue)(right.BoolValue ? 1L : 0L);

			return left + right; // 递归调用处理转换后的值
		}

		/// <summary>
		/// 减法运算符
		/// </summary>
		public static VarValue operator -(VarValue left, VarValue right)
		{
			// 确保都是数值类型
			if (!left.IsNumeric() || !right.IsNumeric())
			{
				throw new InvalidOperationException($"不能对 {left.Type} 和 {right.Type} 执行减法运算");
			}

			// 优先使用浮点数精度
			if (left.Type == VarType.Double || right.Type == VarType.Double)
			{
				return (VarValue)(left.ToDouble() - right.ToDouble());
			}
			else
			{
				return (VarValue)(left.ToLong() - right.ToLong());
			}
		}

		/// <summary>
		/// 乘法运算符
		/// </summary>
		public static VarValue operator *(VarValue left, VarValue right)
		{
			// 确保都是数值类型
			if (!left.IsNumeric() || !right.IsNumeric())
			{
				throw new InvalidOperationException($"不能对 {left.Type} 和 {right.Type} 执行乘法运算");
			}

			// 优先使用浮点数精度
			if (left.Type == VarType.Double || right.Type == VarType.Double)
			{
				return (VarValue)(left.ToDouble() * right.ToDouble());
			}
			else
			{
				return (VarValue)(left.ToLong() * right.ToLong());
			}
		}

		/// <summary>
		/// 除法运算符
		/// </summary>
		public static VarValue operator /(VarValue left, VarValue right)
		{
			// 确保都是数值类型
			if (!left.IsNumeric() || !right.IsNumeric())
			{
				throw new InvalidOperationException($"不能对 {left.Type} 和 {right.Type} 执行除法运算");
			}

			double rightVal = right.ToDouble();
			if (Math.Abs(rightVal) < double.Epsilon)
			{
				throw new DivideByZeroException("除数不能为零");
			}

			// 除法总是返回浮点数以保持精度
			return (VarValue)(left.ToDouble() / rightVal);
		}

		/// <summary>
		/// 取模运算符
		/// </summary>
		public static VarValue operator %(VarValue left, VarValue right)
		{
			// 确保都是数值类型
			if (!left.IsNumeric() || !right.IsNumeric())
			{
				throw new InvalidOperationException($"不能对 {left.Type} 和 {right.Type} 执行取模运算");
			}

			long rightVal = right.ToLong();
			if (rightVal == 0)
			{
				throw new DivideByZeroException("取模运算的除数不能为零");
			}

			return (VarValue)(left.ToLong() % rightVal);
		}

		/// <summary>
		/// 一元负号运算符
		/// </summary>
		public static VarValue operator -(VarValue value)
		{
			if (!value.IsNumeric())
			{
				throw new InvalidOperationException($"不能对 {value.Type} 执行负号运算");
			}

			return value.Type switch
			{
				VarType.Long => (VarValue)(-value.LongValue),
				VarType.Double => (VarValue)(-value.DoubleValue),
				VarType.Bool => (VarValue)(value.BoolValue ? -1L : 0L),
				_ => throw new InvalidOperationException($"不支持对 {value.Type} 执行负号运算")
			};
		}

		/// <summary>
		/// 一元正号运算符
		/// </summary>
		public static VarValue operator +(VarValue value)
		{
			if (!value.IsNumeric())
			{
				throw new InvalidOperationException($"不能对 {value.Type} 执行正号运算");
			}
			return value; // 正号不改变值
		}

		#endregion

		#region 比较运算符

		/// <summary>
		/// 相等比较
		/// </summary>
		public static bool operator ==(VarValue left, VarValue right)
		{
			// 类型相同时直接比较
			if (left.Type == right.Type)
			{
				return left.Type switch
				{
					VarType.Long => left.LongValue == right.LongValue,
					VarType.Double => Math.Abs(left.DoubleValue - right.DoubleValue) < double.Epsilon,
					VarType.Bool => left.BoolValue == right.BoolValue,
					VarType.String => string.Equals((string)left.ObjectValue, (string)right.ObjectValue),
					VarType.Object => ReferenceEquals(left.ObjectValue, right.ObjectValue),
					_ => false
				};
			}

			// 不同类型的数值比较
			if (left.IsNumeric() && right.IsNumeric())
			{
				return Math.Abs(left.ToDouble() - right.ToDouble()) < double.Epsilon;
			}

			return false;
		}

		/// <summary>
		/// 不等比较
		/// </summary>
		public static bool operator !=(VarValue left, VarValue right)
		{
			return !(left == right);
		}

		/// <summary>
		/// 大于比较
		/// </summary>
		public static bool operator >(VarValue left, VarValue right)
		{
			if (left.IsNumeric() && right.IsNumeric())
			{
				return left.ToDouble() > right.ToDouble();
			}

			if (left.Type == VarType.String && right.Type == VarType.String)
			{
				return string.Compare((string)left.ObjectValue, (string)right.ObjectValue, StringComparison.Ordinal) > 0;
			}

			throw new InvalidOperationException($"不能比较 {left.Type} 和 {right.Type}");
		}

		/// <summary>
		/// 小于比较
		/// </summary>
		public static bool operator <(VarValue left, VarValue right)
		{
			if (left.IsNumeric() && right.IsNumeric())
			{
				return left.ToDouble() < right.ToDouble();
			}

			if (left.Type == VarType.String && right.Type == VarType.String)
			{
				return string.Compare((string)left.ObjectValue, (string)right.ObjectValue, StringComparison.Ordinal) < 0;
			}

			throw new InvalidOperationException($"不能比较 {left.Type} 和 {right.Type}");
		}

		/// <summary>
		/// 大于等于比较
		/// </summary>
		public static bool operator >=(VarValue left, VarValue right)
		{
			return left > right || left == right;
		}

		/// <summary>
		/// 小于等于比较
		/// </summary>
		public static bool operator <=(VarValue left, VarValue right)
		{
			return left < right || left == right;
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 判断是否为数值类型
		/// </summary>
		public bool IsNumeric()
		{
			return Type == VarType.Long || Type == VarType.Double || Type == VarType.Bool;
		}

		/// <summary>
		/// 判断是否为整数类型
		/// </summary>
		public bool IsInteger()
		{
			return Type == VarType.Long || Type == VarType.Bool;
		}

		/// <summary>
		/// 判断是否为浮点数类型
		/// </summary>
		public bool IsFloat()
		{
			return Type == VarType.Double;
		}

		#endregion

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

		public override bool Equals(object obj)
		{
			if (obj is VarValue other)
			{
				return this == other;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Type switch
			{
				VarType.Long => HashCode.Combine(Type, LongValue),
				VarType.Double => HashCode.Combine(Type, DoubleValue),
				VarType.Bool => HashCode.Combine(Type, BoolValue),
				VarType.String => HashCode.Combine(Type, ObjectValue),
				_ => HashCode.Combine(Type, ObjectValue)
			};
		}
	}

	/// <summary>
	/// 变量类型
	/// </summary>
	public enum VarType
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
		/// 对象字符串类型
		/// </summary>
		Object,
	}
}
