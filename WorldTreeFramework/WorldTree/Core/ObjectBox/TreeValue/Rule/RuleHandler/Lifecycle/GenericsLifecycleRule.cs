/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 11:14

* 描述： 

*/

using System;

namespace WorldTree
{
    public static partial class TreeValueRule
    {
        class GenericsRemoveRule<T> : RemoveRule<TreeValueBase<T>>
        where T : IEquatable<T>
        {
            public override void OnEvent(TreeValueBase<T> self)
            {
                self.m_GlobalValueChange = default;
                self.m_ValueChange = default;
                self.Value = default;
            }
        }
    }
}
