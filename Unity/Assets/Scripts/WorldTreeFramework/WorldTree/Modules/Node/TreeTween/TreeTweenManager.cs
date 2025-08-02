/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{


	/// <summary>
	/// 树渐变管理器
	/// </summary>
	public class TreeTweenManager : Node, ComponentOf<World>
		, AsRule<Awake>
	{
		/// <summary>
		/// 全局法则执行器
		/// </summary>
		public IRuleExecutor<TweenUpdate> ruleActuator;
	}
}
