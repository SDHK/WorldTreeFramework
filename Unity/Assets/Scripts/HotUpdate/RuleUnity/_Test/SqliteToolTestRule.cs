using System.Collections.Generic;

namespace WorldTree
{
	public static class SqliteToolTestRule
	{
		class AddRule : AddRule<SqliteToolTest>
		{
			protected override void Execute(SqliteToolTest self)
			{

				string path = "C:\\Users\\MyPC\\Desktop\\SDHK_Tool\\test.db";

				SqliteTool.Connection(path);
				//SqliteTool.CreateTable<TestData>(SqliteTool.PRIMARY_KEY_NOT_NULL);

				//SqliteTool.Insert(new TestData() { id = 0, value = "A" });
				//SqliteTool.Insert(new TestData() { id = 1, value = "B" });
				//SqliteTool.Insert(new TestData() { id = 2, value = "C" });
				//SqliteTool.Insert(new TestData() { id = 3, value = "D" });
				//SqliteTool.Insert(new TestData() { id = 4, value = "E" });

				List<TestData> testDatas = SqliteTool.Select<TestData>("id > @id", new[] { "id" }, new[] { 2.GetBytes() });
				foreach (var testData in testDatas)
				{
					self.Log($"{testData.id} : {testData.value}");
				}
			}
		}

		class RemoveRule : RemoveRule<SqliteToolTest>
		{
			protected override void Execute(SqliteToolTest self)
			{
				self.Log($"SqliteToolTest关闭！！");
				SqliteTool.Close();
			}
		}
	}
}
