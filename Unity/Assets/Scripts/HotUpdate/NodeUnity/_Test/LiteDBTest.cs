using LiteDB;

namespace WorldTree
{
	public class LiteDBTest : Node
		, ComponentOf<InitialDomain>
		, AsRule<IAwakeRule>
	{
		public LiteDatabase db;
	}

	/// <summary>
	/// 测试数据类
	/// </summary>
	public class TestClass
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}