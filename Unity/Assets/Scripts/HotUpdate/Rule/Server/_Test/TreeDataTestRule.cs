/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 10:10

* 描述：

*/
using System.IO;

namespace WorldTree
{


	//添加一个标记序列化的枚举
	//利用嫁接的方式，将节点挂到树上，并彻底初始化

	public static partial class TreeDataTestRule
	{
		static int Key = nameof(Key).GetFNV1aHash32();
		static int Value = nameof(Value).GetFNV1aHash32();


		static OnRemove<TreeDataNodeDataTest1> OnRemove = (self) =>
		{
			//self.Name = null;
			//self.Age = 0;
			self.KeyCode = KeyCodeTest.A;
			//self.KeyCodes = null;
			//self.KeyCodeRefs = null;
			//self.NodeRef = null;
			self.NodeRef2 = null;
			//self.Ints = null;
			//self.Tuple = default;
		};


		static OnRemove<TreeDataNodeDataTest2> OnRemove2 = (self) =>
		{
			self.Name = null;
			self.Age = 0;
			self.Node = null;
		};


		static unsafe OnAdd<TreeDataTest> OnAdd = (self) =>
		{
			self.treeData = self.AddChild(out TreeDataNodeDataTest1 treeNode);
			treeNode.Name = "测试123";
			treeNode.Age = 18789;

			//多维数组
			treeNode.Ints = new int[][,,]{
				new int[2,2,5]{
					{ { 1, 2, 30, 4, 5 }, { 20, 45, 90, 75, 23 } },
					{ { 1, 23, 360, 84, 5 }, { 2, 5, 9, 5, 2 } }
				},
				new int[2,1,5]{ { { 1220, 45, 90, 75, 23 } }, { { 1, 23, 360, 84, 5 }} },
			};
			treeNode.KeyCodes = [KeyCodeTest.A, KeyCodeTest.C, KeyCodeTest.B,
				KeyCodeTest.A, KeyCodeTest.C, KeyCodeTest.B,
				KeyCodeTest.A, KeyCodeTest.C, KeyCodeTest.B];

			treeNode.KeyCodeRefs = treeNode.KeyCodes;

			treeNode.Tuple = (125, 41.1f);

			//子节点数据
			treeNode.AddChild(out TreeDataNodeDataTest2 child);
			child.Name = "测试4658";
			child.Age = 788723;
			treeNode.NodeRef = child;
			treeNode.NodeRef2 = child;
			child.Node = child;

			////枚举
			treeNode.KeyCode = KeyCodeTest.C;
			//======================================

			//实例 -> bytes
			byte[] bytes = TreeDataHelper.SerializeNode(self.treeData);
			string filePath = "C:\\Users\\admin\\Desktop\\新建文件夹\\TreeDataTest.bytes";

			self.Log($"序列化字节长度{bytes.Length}\n");

			self.treeData.Dispose();
			self.treeData = null;

			//保存到桌面文件
			File.WriteAllBytes(filePath, bytes);
			self.Log($"序列化保存！！!");

			//读取桌面文件
			bytes = File.ReadAllBytes(filePath);

			//bytes -> TreeData
			TreeData treeData = TreeDataHelper.DeserializeTreeData(self, bytes);
			//TreeData -> bytes
			byte[] treeDataBytes = TreeDataHelper.SerializeTreeData(treeData);
			self.Log($"TreeData序列化字节长度{treeDataBytes.Length}\n");

			//bytes -> 实例
			TreeDataNodeDataTest10 node = TreeDataHelper.DeseralizeNode<TreeDataNodeDataTest10>(self, treeDataBytes);
			node.SetParent(self);
			self.treeData10 = node;
			//====
			self.Log($"反序列化引用还原测试！！！ {node.NodeRef.Value == node.NodeRef.Value.Node}");
			//self.Log($"反序列化引用还原测试！！！{node.NodeRef.Value.Node.Name}");

			//self.Log($"反序列化引用还原测试！！！{self.treeData.KeyCode} => {self.treeData.KeyCodes[2]},{self.treeData.Tuple} ： {self.treeData.NodeRef.Value.Age}");
			self.Log($"反序列化引用还原测试！！！{self.treeData10.KeyCode} => {self.treeData10.KeyCodes[2]},{self.treeData10.Tuple} ： {self.treeData10.NodeRef.Value.Name}");

			self.Log("\n通用结构打印：\n");
			self.Log(NodeRule.ToStringDrawTree(self));
		};



		static unsafe OnUpdate<TreeDataNodeDataTest2> OnUpdate = (self) =>
		{
			//self.Log($"TreeDataNodeDataTest{self.Name}:{self.Age}");
		};



		static unsafe OnAdd<TreeDataTest> OnAdd1 = (self) =>
		{
			if (self != null) return;
			AData data = self.AddComponent(out AData _);

			data.AInt = 401.5f;
			data.Ints = new int[][,,]{
				new int[2,2,5]{ { { 1, 2, 30, 4, 5 }, { 20, 45, 90, 75, 23 } }, { { 1, 23, 360, 84, 5 }, { 2, 5, 9, 5, 2 } } },
				new int[2,1,5]{ { { 1220, 45, 90, 75, 23 } }, { { 1, 23, 360, 84, 5 }} },
			};

			data.DataDict = new UnitDictionary<int, string>()
			{
				{ 1, "1.1f测123" },
				{ 2, "2.2f测试123" },
				{ 3, "3.3f" },
				{ 4, "4.4f" },
				{ 5, "5.5f"},
			};

			AData aDataBase = data;
			byte[] bytes = TreeDataHelper.SerializeNode(aDataBase);
			self.RemoveComponent<TreeDataTest, AData>();
			self.Log($"序列化字节长度{bytes.Length}\n");


			TreeDataNodeDataTest1 node = TreeDataHelper.DeseralizeNode<TreeDataNodeDataTest1>(self, bytes);
			self.treeData = node;
			node.SetParent(self);

			self.TryGetComponent(out AData data2);
			string logText = $"\n反序列化{data2.AInt} \n";

			logText += $"\n数组数量{data2.Ints.Length} :\n";
			foreach (var item in data2.Ints)
			{
				logText += $"数组长度{item.Length} :";

				foreach (var item1 in item)
				{
					logText += $"{item1} ";
				}

				//logText += $"{item} ";
			}
			if (data2.DataDict != null)
			{
				logText += $"\n字典数量{data2.DataDict.Count} :\n";
				foreach (var item in data2.DataDict)
				{
					logText += $"[{item.Key}: {item.Value}],";
				}
			}
			self.Log(logText);
			self.Log("\n反序列化结构打印\n");

			self.Log(NodeRule.ToStringDrawTree(self));

		};
	}


}