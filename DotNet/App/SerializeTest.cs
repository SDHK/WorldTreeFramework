using System.Runtime.CompilerServices;
using System;
using System.Buffers;

namespace WorldTree
{




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
	/// 测试类
	/// </summary>
	public class TestClass : Node
	{
		/// <summary>
		/// 测试私有字段
		/// </summary>
		public float testPrivateField;

		/// <summary>
		/// 测试受保护字段
		/// </summary>
		[Protected] public float testProtectedField;



	}

	/// <summary>
	/// 测试类子类
	/// </summary>
	public class TestClassSub : TestClass
	{

	}


	public static class TestClassRule
	{
		/// <summary>
		/// 自身访问测试
		/// </summary>
		public static void Test1(this TestClass self)
		{
			var a = self.testPrivateField;
			var b = self.testProtectedField;
		}

		/// <summary>
		/// 子类访问测试
		/// </summary>
		public static void Test2(this TestClassSub self)
		{
			var a = self.testPrivateField;
			var b = self.testProtectedField;
		}
		/// <summary>
		/// 外部访问测试
		/// </summary>
		public static void Test3(this MyClass self, TestClass node)
		{
			var a = node.testPrivateField;
			var b = node.testProtectedField;
		}
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