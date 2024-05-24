
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:21

* 描述： 异步任务队列锁
* 
*/

namespace WorldTree
{
	/// <summary>
	/// 异步任务队列锁
	/// </summary>
	public class TreeTaskQueueLockManager : Node, ComponentOf<WorldTreeRoot>
		, AsChildBranch
		, AsAwake
	{
		public TreeDictionary<long, DynamicNodeQueue> nodeQueueDictitonary;
	}
}
