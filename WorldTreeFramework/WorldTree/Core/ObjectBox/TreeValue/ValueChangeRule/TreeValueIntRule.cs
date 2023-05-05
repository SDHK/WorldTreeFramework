/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/5 17:04

* 描述： 数值组件Int类型转换法则

*/

using System;

namespace WorldTree
{
    public class TreeValueIntStringChangeRule : ValueChangeRule<int, string>
    {
        public override void OnEvent(TreeValueBase<int> self, string value)
        {
            self.Value = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
        }
    }
    public class TreeValueIntShortChangeRule : ValueChangeRule<int, short>
    {
        public override void OnEvent(TreeValueBase<int> self, short value)
        {
            self.Value = value;
        }
    }
    public class TreeValueIntLongChangeRule : ValueChangeRule<int, long>
    {
        public override void OnEvent(TreeValueBase<int> self, long value)
        {
            self.Value = (int)value;
        }
    }
    public class TreeValueIntFloatChangeRule : ValueChangeRule<int, float>
    {
        public override void OnEvent(TreeValueBase<int> self, float value)
        {
            self.Value = (int)value;
        }
    }
    public class TreeValueIntDoubleChangeRule : ValueChangeRule<int, double>
    {
        public override void OnEvent(TreeValueBase<int> self, double value)
        {
            self.Value = (int)value;
        }
    }

}
