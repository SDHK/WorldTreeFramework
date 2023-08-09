
/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/9 16:34

* 描述： long类型CRC64，(正常的CRC64应为ulong类型)
*        起始设计目的是用于long代替Type在字典中的键值
*        
* CRC64结果范围是2^64,大约是1.8 x 10^19 个可能值。
* 假设字典存储.NET类型,数量级大约在2^16,即65536个类型左右。
* 根据生日悖论公式,冲突概率约等于 n^2/2m,这里n是键数量级,m是哈希值范围。
* 即冲突概率约等于 (2^16) ^ 2 / 2^64,即 2^32 / 2^64,约等于 1 / 2^32。
* 1/2^32换算到概率就是大约等于 1 / 4294967295,约等于 43 亿分之一。
* 
* 
*/


using System;

namespace WorldTree
{
    /// <summary>
    /// long类型CRC64
    /// </summary>
    public static class CRC64
    {
        /// <summary>
        /// 多项式 :这是由国际标准化组织(ISO)在1993年指定的64位CRC校验算法。
        /// </summary>
        private const ulong Polynomial = 0x42F0E1EBA9EA3693;

        /// <summary>
        /// 查找表
        /// </summary>
        private readonly static long[] table = GenerateCRC64Table();

        /// <summary>
        /// 生成CRC64位查找表
        /// </summary>
        private static long[] GenerateCRC64Table()
        {
            long[] table = new long[256];
            for (int i = 0; i < 256; i++)
            {
                ulong crc = CalculateCRC64(i);
                byte[] bytes = BitConverter.GetBytes(crc);
                long value = 0;
                for (int j = 0; j < 8; j++)
                {
                    value = (value << 8) | bytes[j];//这样操作可以覆盖到符号位
                }
                table[i] = value;

            }
            return table;
        }
        private static ulong CalculateCRC64(int i)
        {
            ulong crc = (ulong)i;
            for (int j = 0; j < 8; j++)
            {
                if ((crc & 1) == 1)
                {
                    crc = (crc >> 1) ^ Polynomial;
                }
                else
                {
                    crc >>= 1;
                }
            }
            return crc;
        }

        /// <summary>
        /// 转换为CRC64
        /// </summary>
        public static long GetCRC64(this string str)
        {
            long crc = 0;
            foreach (char c in str)
            {
                crc = table[(crc ^ c) & 0xFF] ^ (crc >> 8);
            }
            return crc;
        }
    }
}
