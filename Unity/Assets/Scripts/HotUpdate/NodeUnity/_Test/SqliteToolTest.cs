using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// SqliteTool测试
	/// </summary>
	public class SqliteToolTest : Node
		, ComponentOf<InitialDomain>
		, AsAwake
	{

	}

	/// <summary>
	/// 测试数据类
	/// </summary>
	public class TestData
	{
		/// <summary>
		/// Id
		/// </summary>
		public int Id;
		/// <summary>
		/// 名称
		/// </summary>
		public string Value;
	}
}
