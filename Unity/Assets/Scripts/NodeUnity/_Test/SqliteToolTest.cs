using System.Collections.Generic;

namespace WorldTree
{
	public class SqliteToolTest : Node
		, ComponentOf<InitialDomain>
		, AsRule<IAwakeRule>
	{

	}

	public class TestData
	{
		public int id;
		public string value;
	}
}
