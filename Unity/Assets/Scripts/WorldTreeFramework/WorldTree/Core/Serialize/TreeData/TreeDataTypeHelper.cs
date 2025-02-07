/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 10:10

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 树数据类型
	/// </summary>
	public static class TreeDataTypeHelper
	{
		/// <summary>
		/// 基础值类型的字节长度
		/// </summary>
		public static Dictionary<Type, int> TypeSizeDict = new()
		{
			[typeof(bool)] = 1,
			[typeof(byte)] = 1,
			[typeof(sbyte)] = 1,
			[typeof(short)] = 2,
			[typeof(ushort)] = 2,
			[typeof(int)] = 4,
			[typeof(uint)] = 4,
			[typeof(long)] = 8,
			[typeof(ulong)] = 8,
			[typeof(float)] = 4,
			[typeof(double)] = 8,
			[typeof(char)] = 4,
			[typeof(decimal)] = 16,
		};

		/// <summary>
		/// 基础值类型哈希表，这些类型序列化是直接写入数据的，用于跳跃数据检查
		/// </summary>
		public static HashSet<Type> BasicsTypeHash = new()
		{
			typeof(string),//string有特别处理可以直接写入数据，所以列为值类型
			typeof(bool),
			typeof(byte),
			typeof(sbyte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(char),
			typeof(decimal),
		};



		/// <summary>
		/// 默认类型码，类型不会记录到数据里，以下标为键，用于最小化类型码，128个内为 1 Btye 长度
		/// </summary>
		public static Type[] TypeCodes = new Type[]
		{
			typeof(object),//0,object代表任何类型
			typeof(string),//1.字符串为常用类型
			typeof(bool),
			typeof(byte),
			typeof(sbyte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(char),
			typeof(decimal),
		};

		/// <summary>
		/// 基础类型对应类型码
		/// </summary>
		private static Dictionary<Type, byte> typeCodeDict;

		/// <summary>
		/// 基础类型对应类型码
		/// </summary>
		public static Dictionary<Type, byte> TypeCodeDict
		{
			get
			{
				if (typeCodeDict == null)
				{
					typeCodeDict = new();
					for (int i = 0; i < TypeCodes.Length; i++)
						typeCodeDict.Add(TypeCodes[i], (byte)i);
				}
				return typeCodeDict;
			}
		}

	}

}
