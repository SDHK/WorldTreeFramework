
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

			//未检验
			//int result = index % maxIndex;
			//return result < 0 ? result + maxIndex : result;
		}

		/// <summary>
		/// 获取最小2次幂
		/// </summary>
		public static int GetPowerOfTwo(int n)
        {
			n--;
			n |= n >> 1;
			n |= n >> 2;
			n |= n >> 4;
			n |= n >> 8;
			n |= n >> 16;
			n++;
			return n;

			// 将n的二进制表示中最高位的1后面的所有位都设置为1，
			// 然后再加1
			// 例如：n = 0001 0001 
			// n - 1 = 0001 0000
			// n |= n >> 1 = 0001 1000
			// n |= n >> 2 = 0001 1110
			// n |= n >> 4 = 0001 1111 
			// ... 
			// n + 1 = 0010 0000
		}

	}
}
