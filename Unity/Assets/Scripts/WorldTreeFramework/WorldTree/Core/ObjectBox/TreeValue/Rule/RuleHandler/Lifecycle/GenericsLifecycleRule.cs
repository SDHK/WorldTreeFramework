/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 11:14

* 描述： 

*/

using System;

namespace WorldTree
{
    public static partial class TreeValueBaseRule
    {
        class GenericsRemoveRule<T> : RemoveRule<TreeValueBase<T>>
        where T : IEquatable<T>
        {
            protected override void Execute(TreeValueBase<T> self)
            {
                self.globalValueChange = default;
                self.valueChange = default;
                self.Value = default;
            }
        }
    }
}
