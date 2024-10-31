
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;

namespace WorldTree
{

	/// <summary>
	/// data
	/// </summary>
	[TreeDataSerializable]
	public partial struct BDataBase
	{
		/// <summary>
		/// 测试int
		/// </summary>
		public float AInt { get; set; } = 10.1f;

		public BDataBase()
		{
		}
	}


	/// <summary>
	/// data
	/// </summary>
	[TreeDataSerializable]
	public partial interface IADataBase
	{
		/// <summary>
		/// 测试int
		/// </summary>
		public float AInt { get; set; }
	}

	/// <summary>
	/// data
	/// </summary>
	[TreeDataSerializable]
	public abstract partial class ADataBase
	{
		/// <summary>
		/// 测试int
		/// </summary>
		public float AInt = 10.1f;
	}

	/// <summary>
	/// data
	/// </summary>
	[TreeDataSerializable]
	public partial class AData : ADataBase
	{
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
	/// 序列化测试
	/// </summary>
	public class TreeDataTest : Node
			, ComponentOf<INode>
			, AsAwake
	{ }
}

