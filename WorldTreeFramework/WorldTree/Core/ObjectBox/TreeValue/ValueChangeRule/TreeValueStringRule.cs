/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/5 17:18

* 描述： 数值组件string类型转换法则

*/

using System;

namespace WorldTree
{
    class TreeValueStringAwakeRule : AwakeRule<TreeValueBase<string>>
    {
        public override void OnEvent(TreeValueBase<string> self)
        {

            self.Value = "";
        }
    }

    class TreeValueStringValueAwakeRule : AwakeRule<TreeValueBase<string>, string>
    {
        public override void OnEvent(TreeValueBase<string> self, string value)
        {
            self.Value = value;
        }
    }

    class TreeValueStringValueChangeRule<T> : ValueChangeRule<string, T>
       where T : IEquatable<T>
    {
        public override void OnEvent(TreeValueBase<string> self, T arg1)
        {
            self.Value = arg1.ToString();
        }
    }

    class TreeValueStringIntChangeRule : TreeValueStringValueChangeRule<int> { }
    class TreeValueStringShortChangeRule : TreeValueStringValueChangeRule<short> { }
    class TreeValueStringLongChangeRule : TreeValueStringValueChangeRule<long> { }
    class TreeValueStringFloatChangeRule : TreeValueStringValueChangeRule<float> { }
    class TreeValueStringDoubleChangeRule : TreeValueStringValueChangeRule<double> { }

}
