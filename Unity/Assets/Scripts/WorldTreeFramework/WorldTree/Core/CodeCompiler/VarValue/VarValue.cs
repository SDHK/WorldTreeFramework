/****************************************

* 作者： 闪电黑客
* 日期： 2026/1/24 14:49

* 描述： 

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 联合类型变量值
	/// </summary>
	//[StructLayout(LayoutKind.Explicit)]
	public partial struct VarValue
	{
		/// <summary>
		/// 变量类型 
		/// </summary>
		//[FieldOffset(0)]
		public VarType Type;
		/// <summary>
		/// 整数值 
		/// </summary>
		// 值联合体（8字节）
		//[FieldOffset(8)]
		public long LongValue;
		/// <summary>
		/// 浮点数值  
		/// </summary>
		//[FieldOffset(8)]
		public double DoubleValue;
		/// <summary>
		/// 布尔值 
		/// </summary>
		//[FieldOffset(8)]
		public bool BoolValue;
		/// <summary>
		/// 对象值 
		/// </summary>
		// 对象引用（字符串等）
		//[FieldOffset(8)]
		public object ObjectValue;


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

}
