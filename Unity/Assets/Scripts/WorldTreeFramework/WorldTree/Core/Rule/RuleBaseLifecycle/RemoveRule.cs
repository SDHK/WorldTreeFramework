/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 移除法则
*
* 从节点移除时触发

*/

namespace WorldTree
{
	/// <summary>
	/// 移除法则
	/// </summary>
	/// <remarks>移除时，按照后序遍历通知</remarks>
	public interface Remove : ISendRule, ILifeCycleRule
	{ }

	///// <summary>
	///// 移除法则
	///// </summary>
	///// <remarks>移除时，按照后序遍历通知</remarks>
	//public abstract class RemoveRule<N> : SendRuleBase<N, Remove> where N : class, INode, AsRule<Remove>
	//{ }
}