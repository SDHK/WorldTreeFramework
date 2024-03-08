
/****************************************

* 作者： 闪电黑客
* 日期： 2023/10/23 20:36

* 描述： 嫁接法则
* 
* 外部节点添加到树上时，会触发嫁接法则，树自己添加的节点不会触发嫁接法则
* 

*/

namespace WorldTree
{
	/// <summary>
	/// 嫁接法则接口
	/// </summary>
	public interface IGraftRule : ISendRuleBase { }

	/// <summary>
	/// 嫁接法则
	/// </summary>
	public abstract class GraftRule<N> : SendRuleBase<N, IGraftRule> where N : class, INode, AsRule<IGraftRule> { }
}
