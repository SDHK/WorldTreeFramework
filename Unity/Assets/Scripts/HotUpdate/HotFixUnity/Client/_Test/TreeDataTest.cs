/****************************************

* 作者：闪电黑客
* 日期：2024/9/11 19:51

* 描述：

*/
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 序列化测试
	/// </summary>
	[TreeDataSerializable]
	public partial class TreeDataTest : Node
		, ComponentOf<INode>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// data
		/// </summary>
		public TreeDataNodeDataTest1 treeData;

		[NodeRule(nameof(UpdateRule<TreeDataTest>))]
		private static void OnUpdateRule(TreeDataTest self)
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
				string filePath = "C:\\Users\\admin\\Desktop\\TreeDataTest.bytes";

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
				string filePath = "C:\\Users\\admin\\Desktop\\TreeDataTest.bytes";
				byte[] bytes = File.ReadAllBytes(filePath);
				TreeDataHelper.Deseralize<TreeDataNodeDataTest1>(self, bytes).SetParent(self);
				self.Log($"反序列化！！!{bytes.Length}");
			}
		}
	}





	/// <summary>
	/// data
	/// </summary>
	[TreeDataSerializable]
	public partial class AData : Node
		, ComponentOf<TreeDataTest>
		, ChildOf<TreeDataTest>
		, AsRule<Awake>
		, AsComponentBranch
		, AsChildBranch
	{

		/// <summary>
		/// 测试int
		/// </summary>
		public long AInt1 = 1;



		/// <summary>
		/// 测试int
		/// </summary>
		public float AInt = 10.1f;

		/// <summary>
		/// 测试int数组
		/// </summary>
		public int[][,,] Ints;

		/// <summary>
		/// 测试字典
		/// </summary>
		public Dictionary<int, string> DataDict;

	}

	/// <summary>
	/// 按键码
	/// </summary>
	public enum KeyCodeTest : ushort //0~65535
	{
		/// <summary>
		/// A
		/// </summary>
		A = 0,
		/// <summary>
		/// b
		/// </summary>
		B = 1,
		/// <summary>
		/// c
		/// </summary>
		C = 2,
	}



	/// <summary>
	/// 测试节点数据1
	/// </summary>
	[TreeDataSerializable]
	public partial class TreeDataNodeDataTest1 : NodeData
		, ChildOf<TreeDataTest>
		, AsRule<Awake>
		, AsChildBranch
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string Name;

		/// <summary>
		/// 年龄
		/// </summary>
		public int Age;

		/// <summary>
		/// 按键码
		/// </summary>
		public KeyCodeTest KeyCode;

		/// <summary>
		/// 测试节点数据2
		/// </summary>
		public NodeRef<TreeDataNodeDataTest2> NodeRef;

		[NodeRule(nameof(UpdateRule<TreeDataNodeDataTest1>))]
		private static void OnUpdateRule(TreeDataNodeDataTest1 self)
		{
			self.Log($"测试数据更新1！！!{self.Name}:{self.Age}");
		}
	}

	/// <summary>
	/// 测试节点数据1
	/// </summary>
	[TreeDataSerializable]
	public partial class TreeDataNodeDataTest2 : Node
		, ChildOf<TreeDataNodeDataTest1>
		, AsRule<Awake>
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string Name;

		/// <summary>
		/// 年龄
		/// </summary>
		public int Age;

		[NodeRule(nameof(UpdateRule<TreeDataNodeDataTest2>))]
		private static void OnUpdateRule(TreeDataNodeDataTest2 self)
		{
			self.Log($"测试数据更新2！！!{self.Name}:{self.Age}");
		}
	}



	/// <summary>
	/// data
	/// </summary>
	[TreeDataSerializable]
	public abstract partial class ADataBase
	{

	}




	//public partial class TreeDataNodeDataTest1
	//{
	//	class TreeDataSerialize1 : TreeDataSerializeRule<TreeDataNodeDataTest1>
	//	{
	//		protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
	//		{
	//			if (self.TryWriteDataHead(value, nameCode, 7, out TreeDataNodeDataTest1 obj, false)) return;
	//			self.WriteUnmanaged(266367750);
	//			self.WriteValue(obj.Name);
	//			self.WriteUnmanaged(-1899241156);
	//			self.WriteValue(obj.Age);
	//			self.WriteUnmanaged(1953212297);
	//			self.WriteValue((int)obj.KeyCode);
	//			self.WriteUnmanaged(1644898394);
	//			self.WriteValue(obj.NodeRef);
	//			self.WriteUnmanaged(921221376);
	//			self.WriteValue(obj.Id);
	//			self.WriteUnmanaged(-306726429);
	//			self.WriteValue(obj.ActiveToggle);
	//			self.WriteUnmanaged(-2120054343);
	//			self.WriteValue(obj.BranchDict);
	//		}
	//	}
	//	class TreeDataDeserialize1 : TreeDataDeserializeRule<TreeDataNodeDataTest1>
	//	{
	//		protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
	//		{
	//			if (self.TryReadClassHead(typeof(TreeDataNodeDataTest1), ref value, out int count)) return;
	//			if (value is not TreeDataNodeDataTest1 obj)
	//			{
	//				value = obj = self.Core.PoolGetNode<TreeDataNodeDataTest1>(true);
	//			}
	//			for (int i = 0; i < count; i++)
	//			{
	//				self.ReadUnmanaged(out nameCode);
	//				SwitchRead(self, ref value, nameCode);
	//			}
	//		}
	//		/// <summary>
	//		/// 字段读取
	//		/// </summary>
	//		private static void SwitchRead(TreeDataByteSequence self, ref object value, int nameCode)
	//		{
	//			if (value is not TreeDataNodeDataTest1 obj) return;
	//			switch (nameCode)
	//			{
	//				case 266367750: self.ReadValue(ref obj.Name); break;
	//				case -1899241156: self.ReadValue(ref obj.Age); break;
	//				case 1953212297: obj.KeyCode = (KeyCode)self.ReadValue<int>(); break;
	//				case 1644898394: self.ReadValue(ref obj.NodeRef); break;
	//				case 921221376: obj.Id = self.ReadValue<long>(); break;
	//				case -306726429: obj.ActiveToggle = self.ReadValue<bool>(); break;
	//				case -2120054343: obj.BranchDict = self.ReadValue<BranchGroup>(); break;
	//				default: self.SkipData(); break;
	//			}
	//		}
	//	}
	//}
}

