/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

*/
namespace WorldTree.Internal
{
	/// <summary>
	/// 异步任务完成类
	/// </summary>
	public class TreeTaskCompleted : AwaiterBase, ISyncTask
		, ChildOf<INode>
		, AsAwake
    {  }
}
