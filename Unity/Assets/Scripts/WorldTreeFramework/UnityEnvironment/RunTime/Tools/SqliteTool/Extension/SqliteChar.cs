
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 18:07

* 描述： 

*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public static partial class SqliteDataReaderExtension
    {

        /// <summary>
        /// 获取 Char
        /// </summary>
        public static char GetCharData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetChar();
        }

        /// <summary>
        /// 转为 Char
        /// </summary>
        public static char GetChar(this byte[] arr)
        {
            return BitConverter.ToChar(arr);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this char value)
        {
            return BitConverter.GetBytes(value);
        }
        /// <summary>
        /// 转为 Char
        /// </summary>
        public static char GetChar(this string str)
        {
            if (str != "" && str != null)
            {
                return char.Parse(str);
            }
            else
            {
                return default(char);
            }
        }
        /// <summary>
        /// 转为 Char[]
        /// </summary>
        public static string GetChars(this string str)
        {
            if (str != "" && str != null)
            {
                return str;
            }
            else
            {
                return null;
            }
        }
      

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this char[] values)
        {
            string str = "";
            for (int i = 0; i < values.Length; i++)
            {
                str += values[i];
            }
            return str;
        }
    }
}