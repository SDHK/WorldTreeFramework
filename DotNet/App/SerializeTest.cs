using System;

namespace WorldTree
{

	/// <summary>
	/// 测试数据
	/// </summary>
	[TreePack]
	public partial class NodeClassDataTest<T1, T2, T3>
		where T1 : unmanaged
		where T2 : unmanaged

	{
		/// <summary>
		/// 测试泛型1
		/// </summary>
		public T1 ValueT1 = default;

		/// <summary>
		/// 测试泛型2
		/// </summary>
		public T2 ValueT2 = default;

		/// <summary>
		/// 测试class
		/// </summary>
		public NodeClassDataTest1<T1, T2> DataTest1 = default;

		/// <summary>
		/// 测试泛型3
		/// </summary>
		public T3 ValueT3 = default;

		/// <summary>
		/// 测试class
		/// </summary>
		public NodeClassDataBase DataTestBase;

	}

	/// <summary>
	/// 测试数据2
	/// </summary>
	[TreePack]
	public partial class NodeClassDataTest1<T1, T2>
		where T1 : unmanaged
	{
		/// <summary>
		/// 测试整数
		/// </summary>
		public T1[] TestInts { get; set; }

		/// <summary>
		/// 测试浮点
		/// </summary>
		public T2 TestT2 { get; set; }
	}

	/// <summary>
	/// 测试数据3
	/// </summary>
	[TreePack]
	[TreePackSub(typeof(NodeClassDataSub1))]
	[TreePackSub(typeof(NodeClassDataSub2))]
	public partial class NodeClassDataBase
	{
		/// <summary>
		/// 测试整数
		/// </summary>
		public int[] TestInts { get; set; }

		/// <summary>
		/// 测试浮点
		/// </summary>
		public int TestT2 { get; set; }
	}

	/// <summary>
	/// 测试数据3
	/// </summary>
	[TreePack]
	public partial class NodeClassDataSub1 : NodeClassDataBase
	{
		/// <summary>
		/// 测试整数
		/// </summary>
		public int TestInt_T;
	}

	/// <summary>
	/// 测试数据4
	/// </summary>
	[TreePack]
	public partial class NodeClassDataSub2 : NodeClassDataBase
	{
		/// <summary>
		/// 测试整数
		/// </summary>
		public float TestFloat_T;
	}


	/// <summary>
	/// 序列化测试
	/// </summary>
	public class SerializeTest : Node
		, ComponentOf<INode>
		, AsAwake
	{ }



	public static partial class SerializeTestRule
	{
		static OnAdd<SerializeTest> OnAddSerializeTest = (self) =>
		{
			self.Log($"嵌套序列化测试！！！！！");

			//随便写点不一样的数据
			NodeClassDataTest<int, float, int> testData = new();
			testData.ValueT1 = 987;
			testData.ValueT2 = 45.321f;
			testData.ValueT3 = 123456;

			//嵌套类型
			testData.DataTest1 = new NodeClassDataTest1<int, float>();
			//testData.DataTest1.TestInts = new[] { 1, 3, 5, 88 };
			testData.DataTest1.TestT2 = 5.789456f;

			testData.DataTestBase = new NodeClassDataBase()
			{
				TestInts = new[] { 17, 31, 54, 8899 },
				TestT2 = 5,
				//TestFloat_T = 1.999f,
			};


			// 序列化
			self.AddTemp(out ByteSequence sequenceWrite).Serialize(testData);
			byte[] bytes = sequenceWrite.ToBytes();

			self.Log($"序列化字节长度{bytes.Length}");

			// 反序列化
			self.AddTemp(out ByteSequence sequenceRead).SetBytes(bytes);
			NodeClassDataTest<int, float, int> testData2 = null;
			sequenceRead.Deserialize(ref testData2);
			string logText = $"反序列化{testData2.ValueT1} {testData2.ValueT2}  嵌套类字段： {testData2.DataTest1.TestT2}   泛型字段：{testData2.ValueT3}";

			logText += $" 数组：";
			if (testData2.DataTest1.TestInts == null)
			{
				logText += $"null !!, ";
			}
			else
			{
				foreach (var item in testData2.DataTest1.TestInts)
				{
					logText += $"{item}, ";
				}
			}


			logText += $" 基类数组：";
			NodeClassDataBase nodeClassDataSub = testData2.DataTestBase as NodeClassDataBase;
			//logText += $" {nodeClassDataSub.TestFloat_T} ";
			foreach (var item in nodeClassDataSub.TestInts)
			{
				logText += $"{item}, ";
			}
			self.Log(logText);
		};
	}
}