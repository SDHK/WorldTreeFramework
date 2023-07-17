/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 12:11

* 描述： 

*/

using System;

namespace WorldTree
{
    public static partial class ArrayPoolGroupRule
    {
        class AwakeRule : AwakeRule<ArrayPoolGroup, Type>
        {
            public override void OnEvent(ArrayPoolGroup self, Type type)
            {
                self.ArrayType = type;
            }
        }

        class AddRule : AddRule<ArrayPoolGroup>
        {
            public override void OnEvent(ArrayPoolGroup self)
            {
                self.AddChild(out self.Pools);
            }
        }

        class RemoveRule : RemoveRule<ArrayPoolGroup>
        {
            public override void OnEvent(ArrayPoolGroup self)
            {
                self.Pools = null;
            }
        }

    }
}
