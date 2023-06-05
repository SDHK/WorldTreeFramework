
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/15 19:44

* 描述： 树渐变管理器

*/



namespace WorldTree
{
    class TreeTweenManagerRootAddRule : RootAddRule<TreeTweenManager> { }

    public class TreeTweenManager : Node, ComponentOf<WorldTreeRoot>
    {
        public GlobalRuleActuator<ITweenUpdateRule> ruleActuator;
    }

    class TreeTweenManagerAddRule : AddRule<TreeTweenManager>
    {
        public override void OnEvent(TreeTweenManager self)
        {
            self.TryGetGlobalRuleActuator(out self.ruleActuator);
        }
    }

    class TreeTweenManagerUpdateRule : UpdateRule<TreeTweenManager>
    {
        public override void OnEvent(TreeTweenManager self, float deltaTime)
        {
            self.ruleActuator?.Send(deltaTime);
        }
    }

    class TreeTweenManagerRemoveRule : RemoveRule<TreeTweenManager>
    {
        public override void OnEvent(TreeTweenManager self)
        {
            self.ruleActuator?.Dispose();
            self.ruleActuator = null;
        }
    }
}
