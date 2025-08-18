/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 10:10

* 描述：

*/
using System.IO;
using UnityEngine;

namespace WorldTree
{
	public static partial class TreeDataTestRule
	{
		[NodeRule(nameof(AddRule<TreeDataTest>))]
		private static void OnAddRule(this TreeDataTest self)
		{

		}

		[NodeRule(nameof(UpdateRule<TreeDataTest>))]
		private static void OnUpdateRule(this TreeDataTest self)
		{
			//self.Log($"初始域更新！！!");

			self.Log($"测试数据更新！！!{self.TypeToCode(typeof(long))}");

			if (Input.GetKeyDown(KeyCode.W))
			{
				self.AddChild(out self.treeData);
				self.treeData.Name = "测试123";
				self.treeData.Age = 18789;

				self.treeData.AddChild(out TreeDataNodeDataTest2 child);
				child.Name = "测试4646";
				child.Age = 788789;

				byte[] bytes = TreeDataHelper.SerializeNode(self.treeData);
				string filePath = "C:\\Users\\SDHK\\Desktop\\TreeDataTest.bytes";

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
				string filePath = "C:\\Users\\SDHK\\Desktop\\TreeDataTest.bytes";
				byte[] bytes = File.ReadAllBytes(filePath);
				TreeDataHelper.Deseralize<TreeDataNodeDataTest1>(self, bytes).SetParent(self);
				self.Log($"反序列化！！!{bytes.Length}");
			}
		}

		[NodeRule(nameof(UpdateRule<TreeDataNodeDataTest1>))]
		private static void OnUpdateRule(this TreeDataNodeDataTest1 self)
		{
			self.Log($"测试数据更新1！！!{self.Name}:{self.Age}");
		}

		[NodeRule(nameof(UpdateRule<TreeDataNodeDataTest2>))]
		private static void OnUpdateRule(this TreeDataNodeDataTest2 self)
		{
			self.Log($"测试数据更新2！！!{self.Name}:{self.Age}");
		}
	}
}
