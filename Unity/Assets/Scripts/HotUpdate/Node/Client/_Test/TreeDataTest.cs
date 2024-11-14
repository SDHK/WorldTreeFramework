namespace WorldTree
{
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
	/// 树数据测试
	/// </summary>
	public class TreeDataTest : Node
		, ComponentOf<INode>
		, AsAwake
		, AsChildBranch
	{

		/// <summary>
		/// data
		/// </summary>
		public TreeDataNodeDataTest1 treeData;
	}
}

