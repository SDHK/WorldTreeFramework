
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/11 20:16

* 描述： 数组对象池
* 

*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 数组对象池
    /// </summary>
    public class ArrayPool : GenericPool<Array>, ChildOf<ArrayPoolGroup>
        , AsRule<IAwakeRule<Type, int>>
    {
        /// <summary>
        /// 数组长度
        /// </summary>
        public int Length;

        public override string ToString()
        {
            return $"[ArrayPool<{ObjectType}>] [{Length}] : {Count} ";
        }
    }
}
