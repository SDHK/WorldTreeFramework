/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/18 15:45

* 描述： 移除前法则接口
*
* 用于结构移除前的处理

*/

namespace WorldTree
{
	/// <summary>
	/// 移除前法则
	/// </summary>
	/// <remarks>移除前，按照前序遍历通知</remarks>
	public interface BeforeRemove : ISendRuleBase, ILifeCycleRule
	{ }

	///// <summary>
	///// 移除前法则
	///// </summary>
	///// <remarks>移除前，按照前序遍历通知</remarks>
	//public abstract class BeforeRemoveRule<N> : SendRuleBase<N, BeforeRemove> where N : class, INode, AsRule<BeforeRemove>
	//{ }
}