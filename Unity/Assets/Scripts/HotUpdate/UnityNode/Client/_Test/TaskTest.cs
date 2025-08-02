﻿/****************************************

* 作者：闪电黑客
* 日期：2024/6/3 10:29

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 任务测试
	/// </summary>
	public class TaskTest : Node, ComponentOf<InitialDomain>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 树任务
		/// </summary>
		public TreeTask treeTask;
		/// <summary>
		/// 树任务令牌
		/// </summary>
		public TreeTaskToken treeTaskToken;

	}

	/// <summary>
	/// 测试
	/// </summary>
    public class  TestClass1
    {
		/// <summary>
		/// 测试方法
		/// </summary>
		public void Test() { }
	}
}