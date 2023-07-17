/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/5 17:18

* 描述： 数值组件string类型转换法则

*/

using System;

namespace WorldTree
{
    public static partial class TreeValueRule
    {
        class StringValueChangeRule<T> : ValueChangeRule<string, T>
           where T : IEquatable<T>
        {
            public override void OnEvent(TreeValueBase<string> self, T arg1)
            {
                self.Value = arg1.ToString();
            }
        }
        class StringIntChangeRule : StringValueChangeRule<int> { }
        class StringShortChangeRule : StringValueChangeRule<short> { }
        class StringLongChangeRule : StringValueChangeRule<long> { }
        class StringFloatChangeRule : StringValueChangeRule<float> { }
        class StringDoubleChangeRule : StringValueChangeRule<double> { }
    }
}
