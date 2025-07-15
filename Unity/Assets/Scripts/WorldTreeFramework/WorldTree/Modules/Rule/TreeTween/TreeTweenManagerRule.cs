using System;

namespace WorldTree
{
	public static partial class TreeTweenManagerRule
	{
		[NodeRule(nameof(AddRule<TreeTweenManager>))]
		private static void OnAdd(this TreeTweenManager self)
		{
			self.Core.GetGlobalRuleExecutor(out self.ruleActuator);
		}

		[NodeRule(nameof(UpdateTimeRule<TreeTweenManager>))]
		private static void OnUpdateTime(this TreeTweenManager self, TimeSpan deltaTime)
		{
			self.ruleActuator?.Send(deltaTime);
		}

		[NodeRule(nameof(RemoveRule<TreeTweenManager>))]
		private static void OnRemove(this TreeTweenManager self)
		{
			self.ruleActuator?.Dispose();
			self.ruleActuator = null;
		}

	}
}
