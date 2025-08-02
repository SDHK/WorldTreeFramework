﻿/****************************************

* 作者：闪电黑客
* 日期：2024/8/12 11:43

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 测试数据
	/// </summary>
	[TreePackSerializable]
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
	/// 测试数据
	/// </summary>
	[TreePackSerializable]
	public partial struct NodeDataTest
	{
		/// <summary>
		/// 测试
		/// </summary>
		public int Age;
		/// <summary>
		/// 测试2
		/// </summary>
		public int Age1;


	}



	//public partial class NodeClassDataTest<T1, T2>
	//{
	//	class Serialize : SerializeRule<ByteSequence, NodeClassDataTest<T1, T2>>
	//	{
	//		protected override void Execute(ByteSequence self, ref NodeClassDataTest<T1, T2> value)
	//		{
	//			self.Write(value.TestFloat);
	//			self.WriteDynamic(value.TestInt);
	//			self.WriteDynamic(value.TestLong);
	//			self.Write(value.TestDouble);
	//			self.Write(value.TestBool);
	//			self.Write(value.ValueT1);
	//			self.Write(value.ValueT2);
	//		}
	//	}
	//	class Deserialize : DeserializeRule<ByteSequence, NodeClassDataTest<T1, T2>>
	//	{
	//		protected override void Execute(ByteSequence self, ref NodeClassDataTest<T1, T2> value)
	//		{
	//			if (value == null) value = new();
	//			self.Read(out value.TestFloat);
	//			self.ReadDynamic(out value.TestInt);
	//			self.ReadDynamic(out value.TestLong);
	//			self.Read(out value.TestDouble);
	//			self.Read(out value.TestBool);
	//			self.Read(out value.ValueT1);
	//			self.Read(out value.ValueT2);
	//		}
	//	}
	//}





	/// <summary>
	/// 序列化测试
	/// </summary>
	public partial class SerializeTest : Node
		, ComponentOf<INode>
		, AsRule<Awake>
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
		/// 测试结构体
		/// </summary>
		public NodeDataTest nodeDataTest = new NodeDataTest() { Age = 123, Age1 = 10456 };

	}
}

