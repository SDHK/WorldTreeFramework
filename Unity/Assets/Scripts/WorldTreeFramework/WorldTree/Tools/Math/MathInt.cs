
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/18 14:31

* 描述： int类型的数学计算静态工具类

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// int类型的数学计算静态工具类
	/// </summary>
	public static class MathInt
    {
        /// <summary>
        /// 循环的正整数                                        
        /// 数值会在 0 ~ MaxIndex 之间循环
        /// </summary>
        /// <param name="index">正负整数</param>
        /// <param name="maxIndex">循环最大值</param>
        public static int Loop(this int index, int maxIndex)
        {
            int remainder = Math.Abs(index) % maxIndex;
            return (index >= 0) ? index % maxIndex : (remainder == 0) ? 0 : maxIndex - remainder;
        }
    }
}
