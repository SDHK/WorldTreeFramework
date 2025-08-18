using System;

namespace WorldTree
{
	public static partial class TreeTweenManagerRule
	{
		[NodeRule(nameof(AddRule<TreeTweenManager>))]
		private static void OnAddRule(this TreeTweenManager self)
		{
			self.Core.GetRuleBroadcast(out self.ruleActuator);
		}

		[NodeRule(nameof(UpdateTimeRule<TreeTweenManager>))]
		private static void OnUpdateTimeRule(this TreeTweenManager self, TimeSpan deltaTime)
		{
			self.ruleActuator?.Send(deltaTime);
		}

		[NodeRule(nameof(RemoveRule<TreeTweenManager>))]
		private static void OnRemoveRule(this TreeTweenManager self)
		{
			self.ruleActuator?.Dispose();
			self.ruleActuator = null;
		}

	}
}
