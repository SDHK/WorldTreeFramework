
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


}

