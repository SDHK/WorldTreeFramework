/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/5 14:44

* 描述： 数值组件Float类型转换法则

*/

namespace WorldTree
{
    public static partial class TreeValueBaseRule
    {
        class FloatStringChangeRuleEvent : TreeValueGenericsChangeRuleEvent<float, string>
        {
            protected override void OnEvent(TreeValueBase<float> self, string value)
            {
                self.Value = string.IsNullOrEmpty(value) ? 0f : float.Parse(value);
            }
        }
        class FloatIntChangeRuleEvent : TreeValueGenericsChangeRuleEvent<float, int>
        {
            protected override void OnEvent(TreeValueBase<float> self, int value)
            {
                self.Value = value;
            }
        }

        class FloatShortChangeRuleEvent : TreeValueGenericsChangeRuleEvent<float, short>
        {
            protected override void OnEvent(TreeValueBase<float> self, short value)
            {
                self.Value = value;
            }
        }
        class FloatLongChangeRuleEvent : TreeValueGenericsChangeRuleEvent<float, long>
        {
            protected override void OnEvent(TreeValueBase<float> self, long value)
            {
                self.Value = value;
            }
        }
        class FloatDoubleChangeRuleEvent : TreeValueGenericsChangeRuleEvent<float, double>
        {
            protected override void OnEvent(TreeValueBase<float> self, double value)
            {
                self.Value = (float)value;
            }
        }
    }

}
