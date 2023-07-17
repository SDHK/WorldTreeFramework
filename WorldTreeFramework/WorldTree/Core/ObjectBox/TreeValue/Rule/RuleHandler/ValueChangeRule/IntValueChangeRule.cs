/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/5 17:04

* 描述： 数值组件Int类型转换法则

*/


namespace WorldTree
{
    public static partial class TreeValueRule
    {
        class IntStringChangeRule : ValueChangeRule<int, string>
        {
            public override void OnEvent(TreeValueBase<int> self, string value)
            {
                self.Value = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
            }
        }
        class IntShortChangeRule : ValueChangeRule<int, short>
        {
            public override void OnEvent(TreeValueBase<int> self, short value)
            {
                self.Value = value;
            }
        }
        class IntLongChangeRule : ValueChangeRule<int, long>
        {
            public override void OnEvent(TreeValueBase<int> self, long value)
            {
                self.Value = (int)value;
            }
        }
        class IntFloatChangeRule : ValueChangeRule<int, float>
        {
            public override void OnEvent(TreeValueBase<int> self, float value)
            {
                self.Value = (int)value;
            }
        }
        class IntDoubleChangeRule : ValueChangeRule<int, double>
        {
            public override void OnEvent(TreeValueBase<int> self, double value)
            {
                self.Value = (int)value;
            }
        }
    }
}
