/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 15:15

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 测试数据
	/// </summary>
	[TreePackSerializable]
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
		/// 测试泛型3
		/// </summary>
		public T3 ValueT3 { get; set; } = default;

		/// <summary>
		/// 测试class
		/// </summary>
		public NodeClassDataTest1<T1, T2> DataTest1 = default;


		/// <summary>
		/// 测试class
		/// </summary>
		public NodeClassDataBase DataTestBase;

	}

	/// <summary>
	/// 测试数据2
	/// </summary>
	[TreePackSerializable]
	public partial struct NodeClassDataTest1<T1, T2>
	//where T1 : unmanaged
	{
		/// <summary>
		/// 测试整数
		/// </summary>
		public T1[] TestInts { get; set; }

		/// <summary>
		/// 测试浮点
		/// </summary>
		public T2 TestT2 { get; set; }

		/// <summary>
		/// 测试字典
		/// </summary>
		public UnitDictionary<int, string> ValueT4Dict;
	}


	/// <summary>
	/// 测试数据3
	/// </summary>
	[TreePackSerializable]
	[TreePackSub(typeof(NodeClassDataSub1<int>))]
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
	[TreePackSerializable]
	public partial class NodeClassDataSub1<T> : NodeClassDataBase
	{
		/// <summary>
		/// 测试整数
		/// </summary>
		public int TestInt_T;
	}

	/// <summary>
	/// 测试数据4
	/// </summary>
	[TreePackSerializable]
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

}