/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	class TreeTweenManagerRootAddRule : RootAddRule<TreeTweenManager> { }


	/// <summary>
	/// 树渐变管理器
	/// </summary>
	public class TreeTweenManager : Node, ComponentOf<WorldTreeRoot>
		, AsAwake
	{
		/// <summary>
		/// 全局法则执行器
		/// </summary>
		public IRuleActuator<TweenUpdate> ruleActuator;
	}

	class TreeTweenManagerAddRule : AddRule<TreeTweenManager>
	{
		protected override void Execute(TreeTweenManager self)
		{
			self.Core.GetGlobalRuleActuator(out self.ruleActuator);
		}
	}

	class TreeTweenManagerUpdateRule : UpdateTimeRule<TreeTweenManager>
	{
		protected override void Execute(TreeTweenManager self, TimeSpan deltaTime)
		{
			self.ruleActuator?.Send(deltaTime);
		}
	}

	class TreeTweenManagerRemoveRule : RemoveRule<TreeTweenManager>
	{
		protected override void Execute(TreeTweenManager self)
		{
			self.ruleActuator?.Dispose();
			self.ruleActuator = null;
		}
	}
}
