/****************************************

* 作者： 闪电黑客
* 日期： 2023/10/28 12:50

* 描述： 

*/

namespace WorldTree
{
	public class TaskTest : Node, ComponentOf<InitialDomain>
		, AsComponentBranch
		, AsAwake
	{
		public TreeTask treeTask;
	}
}