
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/15 19:44

* 描述： 树渐变管理器

*/



using System;

namespace WorldTree
{
    class TreeTweenManagerRootAddRule : RootAddRule<TreeTweenManager> { }

    public class TreeTweenManager : Node, ComponentOf<WorldTreeRoot>
        , AsAwake
    {
        public GlobalRuleActuator<TweenUpdate> ruleActuator;
    }

    class TreeTweenManagerAddRule : AddRule<TreeTweenManager>
    {
        protected override void Execute(TreeTweenManager self)
        {
            self.GetOrNewGlobalRuleActuator(out self.ruleActuator);
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
