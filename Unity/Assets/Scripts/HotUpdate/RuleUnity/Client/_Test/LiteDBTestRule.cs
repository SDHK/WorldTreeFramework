namespace WorldTree
{
	public static class LiteDBTestProxyRule
	{
		private class AwakeRule : AwakeRule<LiteDBTestProxy>
		{
			protected override void Execute(LiteDBTestProxy self)
			{
				string path = "C:\\Users\\admin\\Desktop\\新建文件夹\\LiteDBTest.db";
				self.DataBase = self.AddComponent(out LiteDBManager _, path);
			}
		}
	}

	public static class LiteDBTestRule
	{

		private class AwakeRule : AwakeRule<LiteDBTest>
		{
			protected override void Execute(LiteDBTest self)
			{
				//获取数据库代理
				self.Root.AddComponent(out LiteDBTestProxy liteDB);

				long id;

				self.AddChild(out TestClass testClass);
				testClass.Name = "A123测试Test!!!!!ABCD=123456789";

				id = testClass.Id;

				//插入数据
				liteDB.Insert(testClass);

				//销毁类型
				testClass.Dispose();

				//查询数据
				TestClass result = liteDB.FindById<TestClass>(id);

				result.SetParent(self);//设置父节点


				self.Log($"读取：{result.Name}");

				self.Log(NodeRule.ToStringDrawTree(self));
			}
		}
	}
}