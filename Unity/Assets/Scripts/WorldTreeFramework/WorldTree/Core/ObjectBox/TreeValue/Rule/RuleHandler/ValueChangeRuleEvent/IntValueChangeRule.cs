/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/5 17:04

* 描述： 数值组件Int类型转换法则

*/


namespace WorldTree
{
    public static partial class TreeValueBaseRule
    {
        class IntStringChangeRuleEvent : TreeValueGenericsChangeRuleEvent<int, string>
        {
            protected override void Execute(TreeValueBase<int> self, string value)
            {
                self.Value = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
            }
        }
        class IntShortChangeRuleEvent : TreeValueGenericsChangeRuleEvent<int, short>
        {
            protected override void Execute(TreeValueBase<int> self, short value)
            {
                self.Value = value;
            }
        }
        class IntLongChangeRuleEvent : TreeValueGenericsChangeRuleEvent<int, long>
        {
            protected override void Execute(TreeValueBase<int> self, long value)
            {
                self.Value = (int)value;
            }
        }
        class IntFloatChangeRuleEvent : TreeValueGenericsChangeRuleEvent<int, float>
        {
            protected override void Execute(TreeValueBase<int> self, float value)
            {
                self.Value = (int)value;
            }
        }
        class IntDoubleChangeRuleEvent : TreeValueGenericsChangeRuleEvent<int, double>
        {
            protected override void Execute(TreeValueBase<int> self, double value)
            {
                self.Value = (int)value;
            }
        }
    }
}
