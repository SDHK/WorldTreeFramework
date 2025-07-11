﻿/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{
	public static class DataBaseTestProxyRule
	{
		private class AwakeRule : AwakeRule<DataBaseTestProxy>
		{
			protected override void Execute(DataBaseTestProxy self)
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
				self.World.AddComponent(out DataBaseTestProxy dataBaseProxy);

				long id;

				self.AddChild(out TestClass testClass);
				testClass.Name = "A123测试Test!!!!!ABCD=123456789";

				id = testClass.Id;

				//插入数据
				dataBaseProxy.Insert(testClass.Id, testClass);

				//销毁类型
				testClass.Dispose();

				//查询数据
				TestClass result = dataBaseProxy.Find<TestClass>(id);

				result.SetParent(self);//设置父节点


				self.Log($"读取：{result.Name}");

				self.Log(NodeRule.ToStringDrawTree(self));
			}
		}
	}
}