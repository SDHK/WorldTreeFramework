
/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/9 20:45

* 描述： long类型哈希码
* 
*       假设65536个类型，冲突概率约等于
*       哈希码64   ：0.00000002%
*       哈希码     ：63.2%
* 
* 哈希码64范围   2^64
* 哈希码范围     2^32 
* 
* 假设类型,数量级大约在2^16,即65536个类型左右。
* 生日悖论公式,p = 1 - e^(-n^2/2m)
* 
* 将n=65536, m=2^64带入:
* p = 1 - e^(-4294967296/18446744073709551616)
* p = 1 - 0.99999999976716935637323542540378
* p = 0.00000000023283064362676457459622
* 
* 哈希码的计算结果是
* p = 0.63212055882855767840447622983854
* 
* p的范围是0到1
* 

*/

using System;
using System.Security.Cryptography;
using System.Text;

namespace WorldTree
{
    /// <summary>
    /// 哈希码计算
    /// </summary>
    public static class HashCore
    {
		/// <summary>
		/// SHA256
		/// </summary>
		private readonly static SHA256 sha256 = SHA256.Create();

        /// <summary>
        /// 获取64位的哈希码
        /// </summary>
        public static long GetHash64(this string str)
        {
            return BitConverter.ToInt64(sha256.ComputeHash(Encoding.UTF8.GetBytes(str)), 0);
        }
    }
}
