/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/5 17:18

* 描述： 数值组件string类型转换法则

*/

using System;

namespace WorldTree
{

    public class TreeValueStringValueChangeRule<T> : ValueChangeRule<string, T>
        where T : IEquatable<T>
    {
        public override void OnEvent(TreeValueBase<string> self, T arg1)
        {
            self.Value = arg1.ToString();
        }
    }

    public class TreeValueStringIntChangeRule : TreeValueStringValueChangeRule<int> { }
    public class TreeValueStringShortChangeRule : TreeValueStringValueChangeRule<short> { }
    public class TreeValueStringLongChangeRule : TreeValueStringValueChangeRule<long> { }
    public class TreeValueStringFloatChangeRule : TreeValueStringValueChangeRule<float> { }
    public class TreeValueStringDoubleChangeRule : TreeValueStringValueChangeRule<double> { }

}
