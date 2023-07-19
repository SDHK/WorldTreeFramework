/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 14:57

* 描述： 

*/

namespace WorldTree
{
    public static partial class UnitPoolManagerRule
    {

        class AddRule : AddRule<UnitPoolManager>
        {
            public override void OnEvent(UnitPoolManager self)
            {
                self.AddChild(out self.m_Pools);
            }
        }

        class RemoveRule : RemoveRule<UnitPoolManager>
        {
            public override void OnEvent(UnitPoolManager self)
            {
                self.m_Pools = null;
            }
        }
    }
}
