
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 18:25

* 描述： 

*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public static partial class SqliteDataReaderExtension
    {

        /// <summary>
        /// 获取 String
        /// </summary>
        public static string GetStringData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetString();
        }

        /// <summary>
        /// 获取 String[]
        /// </summary>
        public static string[] GetStringsData(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetStrings();
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetString(this byte[] arr)
        {
            if (arr == null) return "";
            return Encoding.UTF8.GetString(arr);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "";
            }

            return Encoding.UTF8.GetBytes(value);
        }


        /// <summary>
        /// 转为 String[]
        /// </summary>
        public static string[] GetStrings(this byte[] arr)
        {
            if (arr == null) return null;
            return arr.GetString().Split('|');
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this string[] value)
        {
            if (value == null) return null;
            string str = "";
            for (int i = 0; i < value.Length; i++)
            {
                if (i < value.Length - 1)
                {
                    str += value[i] + '|';
                }
                else
                {
                    str += value[i];
                }
            }
            return str.GetBytes();
        }


        /// <summary>
        /// 转为 string[]
        /// </summary>
        public static string[] GetStrings(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                return datas.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this string[] values)
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