/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 11:53

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetInit : Node, ComponentOf<INode>
		, AsComponentBranch
		, AsAwake
	{
		/// <summary>
		/// 测试
		/// </summary>
		public Action Action;
	}
}