using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
	/// <summary>
	/// 测试数据
	/// </summary>
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


	/// <summary>
	/// 序列化生成兄弟类
	/// </summary>
	public partial class SerializeTest
	{
		/// <summary>
		/// 序列化
		/// </summary>
		public static void Serialize(ref SerializeTest self, ByteSequence byteSequence)
		{
			var a = self.state;

		}

		/// <summary>
		/// 反序列化
		/// </summary>
		public static void Deserialize(ref SerializeTest self, ByteSequence byteSequence)
		{

		}
	}



	/// <summary>
	/// 序列化测试
	/// </summary>
	public partial class SerializeTest : Node
		, ComponentOf<INode>
		, AsAwake
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
