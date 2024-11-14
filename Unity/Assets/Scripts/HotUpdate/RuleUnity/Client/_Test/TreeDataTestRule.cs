using System.IO;
using UnityEngine;

namespace WorldTree
{
	public static partial class TreeDataTestRule
	{
		static OnAdd<TreeDataTest> OnAdd = (self) =>
		{


		};

		static OnUpdate<TreeDataTest> OnUpdate = (self) =>
		{
			//self.Log($"初始域更新！！!");

			if (Input.GetKeyDown(KeyCode.W))
			{
				self.AddChild(out self.treeData);
				self.treeData.Name = "测试123";
				self.treeData.Age = 18789;

				self.treeData.AddChild(out TreeDataNodeDataTest2 child);
				child.Name = "测试4646";
				child.Age = 788789;

				byte[] bytes = TreeDataHelper.SerializeNode(self.treeData);
				string filePath = "C:\\Users\\admin\\Desktop\\新建文件夹\\TreeDataTest.bytes";

				//保存到桌面文件
				File.WriteAllBytes(filePath, bytes);
				self.Log($"序列化保存！！!{bytes.Length}");
			}

			if (Input.GetKeyDown(KeyCode.E))
			{
				self.treeData.Dispose();
				self.treeData = null;
			}

			if (Input.GetKeyDown(KeyCode.R))
			{
				//读取桌面文件
				string filePath = "C:\\Users\\admin\\Desktop\\新建文件夹\\TreeDataTest.bytes";
				byte[] bytes = File.ReadAllBytes(filePath);
				TreeDataHelper.DeseralizeNode(self, bytes).TryGraftSelfToTree(self);
				self.Log($"反序列化！！!{bytes.Length}");
			}
		};

		static OnUpdate<TreeDataNodeDataTest1> OnUpdate1 = (self) =>
		{
			self.Log($"测试数据更新1！！!{self.Name}:{self.Age}");
		};

		static OnUpdate<TreeDataNodeDataTest2> OnUpdate2 = (self) =>
		{
			self.Log($"测试数据更新2！！!{self.Name}:{self.Age}");
		};
	}
}
