﻿using System.Runtime.CompilerServices;
using System;

namespace WorldTree
{

	/// <summary>
	/// 测试类
	/// </summary>
	class MyClass
	{
		/// <summary>
		/// 整型字段
		/// </summary>
		public int IntField;
		/// <summary>
		/// 长整型字段
		/// </summary>
		public long LongField;
		/// <summary>
		/// 浮点型字段
		/// </summary>
		public float FloatField;
	}

	public static class SerializeTestRule
	{
		/// <summary>
		/// T
		/// </summary>
		/// <param name="self"></param>
		public static void Test(this SerializeTest self)
		{
			int a = self.@A;
		}

	}

	/// <summary>
	/// 序列化测试
	/// </summary>
	public class SerializeTest : Node
		, ComponentOf<INode>
		, AsAwake
	{
		/// <summary>
		/// a
		/// </summary>
		public int A = 0;

		public SerializeTest()
		{

			/*
			 * 类型码	对象UID	字节数 
			 * 
			 * long		long		int		long ?
			 * 类型码	UID/下标	分割数	字节数 
			 * 
			 *	string	0			1				
			 * 
			 *	int		1 	
			 *	
			 *	Node类型才能避免循环引用，引用UID
			 *	普通类型直接序列化，引用下标
			 *	Dict<long,INode>
			 *	HashSet<Object>
			 *	
			 */

			MyClass obj = new MyClass
			{
				IntField = 123,
				LongField = 4567890123456789,
				FloatField = 123.45f
			};

			byte[] bytes = new byte[sizeof(int) + sizeof(long) + sizeof(float)];

			unsafe
			{
				fixed (byte* pBytesStart = bytes)
				{
					byte* pBytes = pBytesStart; 

					*(int*)pBytes = obj.IntField;
					pBytes += sizeof(int); 

					*(long*)pBytes = obj.LongField;
					pBytes += sizeof(long); 

					*(float*)pBytes = obj.FloatField;
					pBytes += sizeof(float);

					*(int*)pBytes = 1;

				}
			}

			unsafe
			{
				fixed (byte* pBytes = bytes)
				{
					byte* pCurrent = pBytes;

					// Copy IntField
					byte* intFieldAsBytePtr = (byte*)Unsafe.AsPointer(ref Unsafe.As<int, byte>(ref obj.IntField));
					Unsafe.CopyBlockUnaligned(pCurrent, intFieldAsBytePtr, sizeof(int));
					pCurrent += sizeof(int);

					// Copy LongField
					byte* longFieldAsBytePtr = (byte*)Unsafe.AsPointer(ref Unsafe.As<long, byte>(ref obj.LongField));
					Unsafe.CopyBlockUnaligned(pCurrent, longFieldAsBytePtr, sizeof(long));
					pCurrent += sizeof(long);

					// Copy FloatField
					byte* floatFieldAsBytePtr = (byte*)Unsafe.AsPointer(ref Unsafe.As<float, byte>(ref obj.FloatField));
					Unsafe.CopyBlockUnaligned(pCurrent, floatFieldAsBytePtr, sizeof(float));
				}
			}

			Console.WriteLine("Bytes:");
			foreach (var b in bytes)
			{
				Console.WriteLine(b);
			}

			MyClass newObj = new MyClass();

			unsafe
			{
				fixed (byte* pBytes = bytes)
				{
					// 直接从byte数组读取数据到对象字段
					newObj.IntField = *(int*)pBytes;
					newObj.LongField = *(long*)(pBytes + sizeof(int));
					newObj.FloatField = *(float*)(pBytes + sizeof(int) + sizeof(long));
				}
			}

			//unsafe
			//{
			//	fixed (byte* pBytes = bytes)
			//	{
			//		byte* pCurrent = pBytes;

			//		// Copy back IntField
			//		newObj.IntField = Unsafe.Read<int>(pCurrent);
			//		pCurrent += sizeof(int);

			//		// Copy back LongField
			//		newObj.LongField = Unsafe.Read<long>(pCurrent);
			//		pCurrent += sizeof(long);

			//		// Copy back FloatField
			//		newObj.FloatField = Unsafe.Read<float>(pCurrent);
			//	}
			//}

			// 现在newObj包含了从bytes数组中反序列化得到的值
			Console.WriteLine($"IntField: {newObj.IntField}, LongField: {newObj.LongField}, FloatField: {newObj.FloatField}");
		}

	}
}