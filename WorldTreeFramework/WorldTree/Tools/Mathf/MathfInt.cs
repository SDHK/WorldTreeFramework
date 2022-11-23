
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/18 14:31

* 描述： int类型的数学计算静态工具类

*/

using System;

namespace WorldTree
{
    public static class MathfInt
    {
        /// <summary>
        /// 循环的正整数                                        
        /// 数值会在 0 ~ MaxIndex 之间循环
        /// </summary>
        /// <param name="Index">正负整数</param>
        /// <param name="MaxIndex">循环最大值</param>
        public static int Loop(this int Index, int MaxIndex)
        {
            int remainder = Math.Abs(Index) % MaxIndex;
            return (Index >= 0) ? Index % MaxIndex : (remainder == 0) ? 0 : MaxIndex - remainder;
        }
    }
}
