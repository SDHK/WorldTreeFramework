/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 11:37

* 描述： 

*/

namespace WorldTree
{
    public static partial class TreeValueRule
    {
        class TreeValueStringAwakeRule : AwakeRule<TreeValueBase<string>>
        {
            public override void OnEvent(TreeValueBase<string> self)
            {
                self.Value = "";
            }
        }
        class TreeValueStringValueAwakeRule : AwakeRule<TreeValueBase<string>, string>
        {
            public override void OnEvent(TreeValueBase<string> self, string value)
            {
                self.Value = value;
            }
        }
    }
}
