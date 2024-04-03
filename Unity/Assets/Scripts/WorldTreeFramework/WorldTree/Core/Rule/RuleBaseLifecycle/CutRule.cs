/****************************************

* 作者： 闪电黑客
* 日期： 2023/10/23 20:39

* 描述： 剪切法则
*
* 从树上剪切时，才会触发剪切法则，树移除节点不会触发剪切法则
*

*/

namespace WorldTree
{
	/// <summary>
	/// 剪切法则接口
	/// </summary>
	public interface ICutRule : ISendRuleBase
	{ }

	/// <summary>
	/// 剪切法则
	/// </summary>
	public abstract class CutRule<N> : SendRuleBase<N, ICutRule> where N : class, INode, AsRule<ICutRule>
	{ }
}