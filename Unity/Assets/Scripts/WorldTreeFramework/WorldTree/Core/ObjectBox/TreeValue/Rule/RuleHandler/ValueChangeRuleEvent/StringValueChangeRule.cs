/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/5 17:18

* 描述： 数值组件string类型转换法则

*/

using System;

namespace WorldTree
{
    public static partial class TreeValueBaseRule
    {
        class StringValueChangeRuleEvent<T> : TreeValueGenericsChangeRuleEvent<string, T>
           where T : IEquatable<T>
        {
            protected override void OnEvent(TreeValueBase<string> self, T arg1)
            {
                self.Value = arg1.ToString();
            }
        }
        class StringIntChangeRuleEvent : StringValueChangeRuleEvent<int> { }
        class StringShortChangeRuleEvent : StringValueChangeRuleEvent<short> { }
        class StringLongChangeRuleEvent : StringValueChangeRuleEvent<long> { }
        class StringFloatChangeRuleEvent : StringValueChangeRuleEvent<float> { }
        class StringDoubleChangeRuleEvent : StringValueChangeRuleEvent<double> { }
    }
}
