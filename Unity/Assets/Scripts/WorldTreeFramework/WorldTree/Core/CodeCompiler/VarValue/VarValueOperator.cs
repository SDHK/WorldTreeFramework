using System;

namespace WorldTree
{
	public partial struct VarValue
	{
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

	}
}
