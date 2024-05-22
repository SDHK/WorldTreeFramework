
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 17:48

* 描述： 

*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree.Sample
{
    public static partial class SqliteDataReaderExtension
    {

        /// <summary>
        /// 获取 Decimal[]
        /// </summary>
        public static decimal[] GetDecimalsData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetDecimals();
        }

        /// <summary>
        /// 转为 Decimal
        /// </summary>
        public static decimal GetDecimal(this byte[] arr)
        {
            int[] bits = new int[arr.Length / 4];
            for (int i = 0; i < bits.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    bits[i] |= arr[i * 4 + j] << j * 8;
                }
            }
            return new decimal(bits);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this decimal value)
        {
            int[] bits = decimal.GetBits(value);
            byte[] bytes = new byte[bits.Length * 4];
            for (int i = 0; i < bits.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    bytes[i * 4 + j] = (byte)(bits[i] >> (j * 8));
                }
            }
            return bytes;
        }

        /// <summary>
        /// 转为 decimal[]
        /// </summary>
        public static decimal[] GetDecimals(this byte[] arr)
        {
            if (arr == null) return null;
            byte[] data = new byte[16];
            decimal[] result = new decimal[arr.Length / data.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < result.Length; i++)
            {
                stream.Read(data);
                result[i] = data.GetDecimal();
            }
            return result;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this decimal[] value)
        {
            if (value == null) return null;
            MemoryStream stream = new MemoryStream();
            for (int i = 0; i < value.Length; i++)
            {
                stream.Write(value[i].GetBytes());
            }
            return stream.ToArray();
        }
        /// <summary>
        /// 转为 decimal
        /// </summary>
        public static decimal GetDecimal(this string str)
        {
            if (str != "" && str != null)
            {
                return decimal.Parse(str);
            }
            else
            {
                return default(decimal);
            }

        }

        /// <summary>
        /// 转为 decimal[]
        /// </summary>
        public static decimal[] GetDecimals(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                return datas.Select((data) => decimal.Parse(data)).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this decimal[] values)
        {
            string str = "";
            for (int i = 0; i < values.Length; i++)
            {
                str += values[i];
                if (i < values.Length - 1)
                {
                    str += "|";
                }
            }
            return str;
        }

    }
}
