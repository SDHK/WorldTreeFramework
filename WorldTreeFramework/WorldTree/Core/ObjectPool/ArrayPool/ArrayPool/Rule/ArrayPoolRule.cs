
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 12:09

* 描述： 

*/

using System;

namespace WorldTree
{
    public static partial class ArrayPoolRule
    {
        public static Array ObjectNew(this ArrayPool self, IPool pool)
        {
            return Array.CreateInstance(self.ObjectType, self.Length);
        }
        public static void ObjectOnRecycle(this ArrayPool self, Array obj)
        {
            Array.Clear(obj, 0, obj.Length);
        }
    }
}
