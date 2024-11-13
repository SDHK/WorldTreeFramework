
using System.Runtime.CompilerServices;
using System;
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
	}

	/// <summary>
	/// 测试节点数据1
	/// </summary>
	[TreeDataSerializable]
	public partial class TreeDataNodeDataTest2 : Node
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




	public partial class TreeDataTest
	{
		class TreeDataSerialize1 : TreeDataSerializeRule<TreeDataTest>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				TreeDataTest obj;
				switch (nameCode)
				{
					case 0:
						self.WriteDynamic(0L);
						if (self.WriteCheckNull(value, 3, out obj)) return;
						break;
					case -1:
						self.WriteType(typeof(TreeDataTest));
						if (self.WriteCheckNull(value, 3, out obj)) return;
						break;
					default:
						obj = (TreeDataTest)value;
						break;
				}
				self.WriteUnmanaged(1869094581);
				self.WriteValue(obj.treeData);
				self.WriteUnmanaged(-306726429);
				self.WriteValue(obj.ActiveToggle);
				self.WriteUnmanaged(-2120054343);
				self.WriteValue(obj.BranchDict);
			}
		}
		class TreeDataDeserialize1 : TreeDataDeserializeRule<TreeDataTest>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				var targetType = typeof(TreeDataTest);
				if (!(self.TryReadType(out var dataType) && dataType == targetType))
				{
					self.SubTypeReadValue(dataType, targetType, ref value);
					return;
				}
				if (self.ReadCheckNull(out int count))
				{
					value = null;
					return;
				}
				if (count < 0)
				{
					self.ReadBack(4);
					self.SkipData(dataType);
					return;
				}
				if (value is not TreeDataTest obj)
				{
					value = obj = self.Core.PoolGetNode<TreeDataTest>();
					obj.IsSerialize = true;
				}
				for (int i = 0; i < count; i++)
				{
					self.ReadUnmanaged(out nameCode);
					SwitchRead1(self, ref value, nameCode);
				}
			}
			/// <summary>
			/// 字段读取
			/// </summary>
			private static void SwitchRead1(TreeDataByteSequence self, ref object value, int nameCode)
			{
				if (value is not TreeDataTest obj) return;
				switch (nameCode)
				{
					case 1869094581: self.ReadValue(ref obj.treeData); break;
					case -306726429: obj.ActiveToggle = self.ReadValue<bool>(); break;
					case -2120054343: obj.BranchDict = self.ReadValue<BranchGroup>(); break;
					default: self.SkipData(); break;
				}
			}
		}
	}
}

