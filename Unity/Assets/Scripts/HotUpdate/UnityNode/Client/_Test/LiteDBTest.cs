﻿/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 数据库代理
	/// </summary>
	public class DataBaseTestProxy : DataBaseProxy
		, AsRule<Awake>
	{

	}


	/// <summary>
	/// 测试
	/// </summary>
	public class LiteDBTest : Node
	, AsComponentBranch
	, AsChildBranch
	, ComponentOf<InitialDomain>
	, AsRule<Awake>
	{

	}

	/// <summary>
	/// 测试数据类
	/// </summary>
	[TreeDataSerializable]
	public partial class TestClass : NodeData
		, ChildOf<INode>
		, AsRule<Awake>
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string Name;
	}
}