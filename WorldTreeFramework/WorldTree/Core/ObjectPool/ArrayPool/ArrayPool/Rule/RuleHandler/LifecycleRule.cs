/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 12:05

* 描述： 

*/

using System;

namespace WorldTree
{
    public static partial class ArrayPoolRule
    {
        class AwakeRule : AwakeRule<ArrayPool, Type, int>
        {
            public override void OnEvent(ArrayPool self, Type type, int length)
            {
                self.ObjectType = type;
                self.Length = length;
                self.NewObject = self.ObjectNew;
                self.objectOnRecycle = self.ObjectOnRecycle;
            }
        }
    }
}
