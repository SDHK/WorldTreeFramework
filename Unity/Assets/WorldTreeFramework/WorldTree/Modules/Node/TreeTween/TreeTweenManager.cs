
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/15 19:44

* 描述： 树渐变管理器

*/



namespace WorldTree
{
    class TreeTweenManagerRootAddRule : RootAddRule<TreeTweenManager> { }

    public class TreeTweenManager : Node, ComponentOf<WorldTreeRoot>
        , AsRule<IAwakeRule>
    {
        public GlobalRuleActuator<ITweenUpdateRule> ruleActuator;
    }

    class TreeTweenManagerAddRule : AddRule<TreeTweenManager>
    {
        protected override void OnEvent(TreeTweenManager self)
        {
            self.GetOrNewGlobalRuleActuator(out self.ruleActuator);
        }
    }

    class TreeTweenManagerUpdateRule : UpdateTimeRule<TreeTweenManager>
    {
        protected override void OnEvent(TreeTweenManager self, float deltaTime)
        {
            self.ruleActuator?.Send(deltaTime);
        }
    }

    class TreeTweenManagerRemoveRule : RemoveRule<TreeTweenManager>
    {
        protected override void OnEvent(TreeTweenManager self)
        {
            self.ruleActuator?.Dispose();
            self.ruleActuator = null;
        }
    }
}
