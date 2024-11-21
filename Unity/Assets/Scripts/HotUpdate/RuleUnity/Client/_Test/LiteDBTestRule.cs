using LiteDB;
using System.Collections.Generic;

namespace WorldTree
{
	public static class LiteDBTestRule
	{
		private class AwakeRule : AwakeRule<LiteDBTest>
		{
			protected override void Execute(LiteDBTest self)
			{
				//创建数据库
				self.db = new LiteDatabase("LiteDBTest.db");

				//获取集合
				ILiteCollection<TestClass> liteCollection = self.db.GetCollection<TestClass>("test");

				//插入数据
				liteCollection.Insert(new TestClass { Id = 1, Name = "John Doe" });
				liteCollection.Insert(new TestClass { Id = 2, Name = "Jane Doe" });
				liteCollection.Insert(new TestClass { Id = 3, Name = "John Doe" });


				//查询数据
				IEnumerable<TestClass> result = liteCollection.Find((x) => x.Id == 3);

				//读取数据
				foreach (TestClass item in result)
				{
					self.Log($"{item.Id} {item.Name}");
				}

				//删除数据
				liteCollection.Delete(3);

				self.db.Dispose();
			}
		}
	}
}