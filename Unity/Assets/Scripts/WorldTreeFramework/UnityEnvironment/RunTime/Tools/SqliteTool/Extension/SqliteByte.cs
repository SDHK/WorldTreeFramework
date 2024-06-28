
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/29 16:42

* 描述： 

*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree.Sample
{
    public static partial class SqliteDataReaderExtension
    {
        /// <summary>
        /// 获取 byte
        /// </summary>
        public static byte GetByteData(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetByte();
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte GetByte(this byte[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                return 0;
            }
            else
            {
                return arr[0];
            }
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this byte value)
        {
            return new byte[] { value };
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte ExcelGetByte(this string str)
        {
            if (str != "" && str != null)
            {
                return byte.Parse(str);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 转为 byte[]
        /// </summary>
        public static byte[] ExcelGetBytes(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                return datas.Select((data) => byte.Parse(data)).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this byte[] values)
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