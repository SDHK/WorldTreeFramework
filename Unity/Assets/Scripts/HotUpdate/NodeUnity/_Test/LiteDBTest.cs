using LiteDB;

namespace WorldTree
{
	/// <summary>
	/// LiteDB测试
	/// </summary>
	public class LiteDBTest : Node
		, ComponentOf<InitialDomain>
		, AsAwake
	{
		/// <summary>
		/// 数据库
		/// </summary>
		public LiteDatabase db;
	}

	/// <summary>
	/// 测试数据类
	/// </summary>
	public class TestClass
	{
		/// <summary>
		/// ID
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }
	}
}