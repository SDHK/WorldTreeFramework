/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 11:53

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.Server
{

	/// <summary>
	/// a
	/// </summary>
	[INodeProxy]

	public partial class TestList<T> : List<T>, INode
	{

	}

	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetInit : Node
		, ComponentOf<MainWorld>
		//, AsBranch<IBranch>
		, AsComponentBranch
		//, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 测试
		/// </summary>
		public int ConfigId;
		/// <summary>
		/// 测试
		/// </summary>
		public Action Action;

		/// <summary>
		/// a
		/// </summary>
		public List<int> intsList;


	}




	/// <summary>
	/// 测试
	/// </summary>
	public partial class Test<T> : Node
	, ComponentOf<DotNetInit>
	, AsRule<TestNodeEvent<DotNetInit>>
	, AsRule<TestNodeEvent<Type>>
	{
		/// <summary>
		/// 字段
		/// </summary>
		public int ConfigId;

		/// <summary>
		/// 属性
		/// </summary>
		public long ConfigName => ConfigId;
	}
}