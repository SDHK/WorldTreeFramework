/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 树渐变管理器
	/// </summary>
	public partial class TreeTweenManager : Node, ComponentOf<World>
		, AsRule<Awake>
	{
		/// <summary>
		/// 全局法则执行器
		/// </summary>
		public RuleBroadcast<TweenUpdate> ruleActuator;

		[NodeRule(nameof(Add))]
		private static void OnAddRule(TreeTweenManager self)
		{
			self.World.GetRuleBroadcast(out self.ruleActuator);
		}

		[NodeRule(nameof(UpdateTime))]
		private static void OnUpdateTimeRule(TreeTweenManager self, TimeSpan deltaTime)
		{
			self.ruleActuator?.Send(deltaTime);
		}

		[NodeRule(nameof(Remove))]
		private static void OnRemoveRule(TreeTweenManager self)
		{
			self.ruleActuator?.Dispose();
			self.ruleActuator = null;
		}
	}
}
