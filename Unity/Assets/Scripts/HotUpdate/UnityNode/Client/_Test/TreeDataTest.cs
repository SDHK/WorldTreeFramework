/****************************************

* 作者：闪电黑客
* 日期：2024/9/11 19:51

* 描述：

*/
using System.Collections.Generic;

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
		, AsAwake
	{
		/// <summary>
		/// data
		/// </summary>
		public TreeDataNodeDataTest1 treeData;
	}





	/// <summary>
	/// data
	/// </summary>
	[TreeDataSerializable]
	public partial class AData : Node
		, ComponentOf<TreeDataTest>
		, ChildOf<TreeDataTest>
		, AsAwake
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
	public partial class TreeDataNodeDataTest1 : Node
		, ChildOf<TreeDataTest>
		, AsAwake
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
	}

	/// <summary>
	/// 测试节点数据1
	/// </summary>
	[TreeDataSerializable]
	public partial class TreeDataNodeDataTest2 : NodeData
		, ChildOf<TreeDataNodeDataTest1>
		, AsAwake
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string Name;

		/// <summary>
		/// 年龄
		/// </summary>
		public int Age;
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

