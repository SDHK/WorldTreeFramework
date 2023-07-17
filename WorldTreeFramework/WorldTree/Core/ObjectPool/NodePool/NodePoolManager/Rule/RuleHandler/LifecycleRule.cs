/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 14:33

* 描述： 

*/

namespace WorldTree
{
    public static partial class NodePoolManagerRule
    {
        class AddRule : AddRule<NodePoolManager>
        {
            public override void OnEvent(NodePoolManager self)
            {
                self.AddChild(out self.m_Pools);
            }
        }

        class RemoveRule : RemoveRule<NodePoolManager>
        {
            public override void OnEvent(NodePoolManager self)
            {
                self.m_Pools = null;
            }
        }
    }
}
