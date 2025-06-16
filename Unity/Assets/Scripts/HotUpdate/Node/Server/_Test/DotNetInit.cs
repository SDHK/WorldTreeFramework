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
	/// 测试节点
	/// </summary>
	public partial class DotNetInit : Node, ComponentOf<INode>
		, AsComponentBranch
		, AsChildBranch
		, AsAwake
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
	public class Test<T> : Node
		, AsTestNodeEvent<DotNetInit>
		, AsRule<IRule>
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