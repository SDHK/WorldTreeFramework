
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/5 14:44

* 描述： 数值组件Float类型转换法则

*/

namespace WorldTree
{
    public static partial class TreeValueRule
    {
        class FloatStringChangeRule : ValueChangeRule<float, string>
        {
            public override void OnEvent(TreeValueBase<float> self, string value)
            {
                self.Value = string.IsNullOrEmpty(value) ? 0f : float.Parse(value);
            }
        }
        class FloatIntChangeRule : ValueChangeRule<float, int>
        {
            public override void OnEvent(TreeValueBase<float> self, int value)
            {
                self.Value = value;
            }
        }

        class FloatShortChangeRule : ValueChangeRule<float, short>
        {
            public override void OnEvent(TreeValueBase<float> self, short value)
            {
                self.Value = value;
            }
        }
        class FloatLongChangeRule : ValueChangeRule<float, long>
        {
            public override void OnEvent(TreeValueBase<float> self, long value)
            {
                self.Value = value;
            }
        }
        class FloatDoubleChangeRule : ValueChangeRule<float, double>
        {
            public override void OnEvent(TreeValueBase<float> self, double value)
            {
                self.Value = (float)value;
            }
        }
    }

}
