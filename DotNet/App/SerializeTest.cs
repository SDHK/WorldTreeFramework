using System.Runtime.CompilerServices;
using System;
using System.Buffers;

namespace WorldTree
{


	/// <summary>
	/// 测试数据
	/// </summary>
	[TreePack]
	public partial class NodeClassDataTest<T1, T2>
		where T1 : unmanaged
		where T2 : unmanaged

	{
		/// <summary>
		/// 测试浮点
		/// </summary>
		public float TestFloat = 1.54321f;
		/// <summary>
		/// 测试整数
		/// </summary>
		public int TestInt = 123;
		/// <summary>
		/// 测试长整数
		/// </summary>
		public long TestLong = 456;
		/// <summary>
		/// 测试双精度
		/// </summary>
		public double TestDouble = 7.123456;
		/// <summary>
		/// 测试布尔
		/// </summary>
		public bool TestBool = true;

		/// <summary>
		/// 测试泛型1
		/// </summary>
		public T1 ValueT1 = default;

		/// <summary>
		/// 测试泛型2
		/// </summary>
		public T2 ValueT2 = default;

	}


	/// <summary>
	/// 测试类
	/// </summary>
	public class MyClass : Node
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
			 * 类型码 字节数 
			 * int值
			 * 结构体类型码 字节数
			 * 
			 * 
			 * 
			 * 数组类型码 字节数 {类型码 字节数}
			 * 
			 * 字典类型码 字节数 值数组类型码 字节数
			 * 
			 *	
			 */



			MyClass obj = new MyClass
			{
				IntField = 123,
				LongField = 4567890123456789,
				FloatField = 123.45f
			};

			Span<byte> span = new byte[sizeof(int) + sizeof(long) + sizeof(float)];

			unsafe
			{
				fixed (byte* pSpan = span)
				{
					byte* pCurrent = pSpan;

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
			foreach (var b in span)
			{
				Console.WriteLine(b);
			}

			MyClass newObj = new MyClass();

			unsafe
			{
				fixed (byte* pSpan = span)
				{
					byte* pCurrent = pSpan;
					// 使用Unsafe.ReadUnaligned从byte数组读取数据到对象字段
					newObj.IntField = Unsafe.ReadUnaligned<int>(pCurrent);
					pCurrent += sizeof(int);
					newObj.LongField = Unsafe.ReadUnaligned<long>(pCurrent);
					pCurrent += sizeof(long);
					newObj.FloatField = Unsafe.ReadUnaligned<float>(pCurrent);
				}
			}


			// 现在newObj包含了从bytes数组中反序列化得到的值
			Console.WriteLine($"IntField: {newObj.IntField}, LongField: {newObj.LongField}, FloatField: {newObj.FloatField}");
		}

	}
}