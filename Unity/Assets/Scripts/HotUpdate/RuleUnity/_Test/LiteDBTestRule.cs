using LiteDB;
using System.Collections.Generic;


namespace WorldTree
{
	public static class LiteDBTestRule
	{
		class AwakeRule : AwakeRule<LiteDBTest>
		{
			protected override void OnEvent(LiteDBTest self)
			{
				self.db = new LiteDatabase("LiteDBTest.db");
				ILiteCollection<TestClass> liteCollection = self.db.GetCollection<TestClass>("test");


				liteCollection.Insert(new TestClass { Id = 1, Name = "John Doe" });
				liteCollection.Insert(new TestClass { Id = 2, Name = "Jane Doe" });
				liteCollection.Insert(new TestClass { Id = 3, Name = "John Doe" });


				IEnumerable<TestClass> result = liteCollection.Find((x) => x.Id == 3);


				foreach (TestClass item in result)
				{
					self.Log($"{item.Id} {item.Name}");
				}
				self.db.Dispose();
			}
		}
	}
}
