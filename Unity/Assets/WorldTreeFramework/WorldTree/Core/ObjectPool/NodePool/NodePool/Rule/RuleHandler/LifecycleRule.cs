/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 14:27

* 描述： 

*/

namespace WorldTree
{
    public static partial class NodePoolRule
    {
        class AddRule : AddRule<NodePool>
        {
            public override void OnEvent(NodePool self)
            {
                self.Core.RuleManager.SupportNodeRule(self.ObjectType);

                //生命周期法则
                self.newRule = self.GetRuleList<INewRule>(self.ObjectType);
                self.getRule = self.GetRuleList<IGetRule>(self.ObjectType);
                self.recycleRule = self.GetRuleList<IRecycleRule>(self.ObjectType);
                self.destroyRule = self.GetRuleList<IDestroyRule>(self.ObjectType);
            }
        }
        class DestroyRule : DestroyRule<NodePool>
        {
            public override void OnEvent(NodePool self)
            {
                self.DisposeAll();
                self.NewObject = null;
                self.DestroyObject = null;
                self.objectOnNew = null;
                self.objectOnGet = null;
                self.objectOnRecycle = null;
                self.objectOnDestroy = null;

                self.newRule = default;
                self.getRule = default;
                self.recycleRule = default;
                self.destroyRule = default;
            }
        }
    }
}
