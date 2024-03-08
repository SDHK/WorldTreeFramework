using LiteDB;

namespace WorldTree
{
	public class LiteDBTest : Node
		, ComponentOf<InitialDomain>
		, AsRule<IAwakeRule>
	{
		public LiteDatabase db;
	}

	public class TestClass
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
