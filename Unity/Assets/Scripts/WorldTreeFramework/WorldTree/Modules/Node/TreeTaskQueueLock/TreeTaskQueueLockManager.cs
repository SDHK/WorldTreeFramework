/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

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
		/// <summary>
		/// 锁的字典
		/// </summary>
		public TreeDictionary<long, DynamicNodeQueue> nodeQueueDict;
	}
}
