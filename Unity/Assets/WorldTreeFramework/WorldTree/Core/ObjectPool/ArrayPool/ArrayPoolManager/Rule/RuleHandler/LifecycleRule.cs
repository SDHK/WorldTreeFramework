/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 12:16

* 描述： 

*/

namespace WorldTree
{
    public static partial class ArrayPoolManagerRule
    {
        class AddRule : AddRule<ArrayPoolManager>
        {
            public override void OnEvent(ArrayPoolManager self)
            {
                self.AddChild(out self.PoolGroups);
            }
        }

        class RemoveRule : RemoveRule<ArrayPoolManager>
        {
            public override void OnEvent(ArrayPoolManager self)
            {
                self.PoolGroups = null;
            }
        }
    }
}
