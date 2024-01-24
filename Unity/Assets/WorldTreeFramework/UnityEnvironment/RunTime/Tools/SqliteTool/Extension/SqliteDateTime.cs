
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 18:09

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
        /// 获取 DateTime
        /// </summary>
        public static DateTime GetDateTimeData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetDateTime();
        }
        /// <summary>
        /// 获取 DateTime[]
        /// </summary>
        public static DateTime[] GetDateTimesData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetDateTimes();
        }
        /// <summary>
        /// 转为 DateTime
        /// </summary>
        public static DateTime GetDateTime(this byte[] arr)
        {
            long ticks = BitConverter.ToInt64(arr, 0);

            DateTime dt = new DateTime(ticks);
            return dt;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this DateTime value)
        {
            return BitConverter.GetBytes(value.Ticks);
        }


        /// <summary>
        /// 转为 DateTime[]
        /// </summary>
        public static DateTime[] GetDateTimes(this byte[] arr)
        {

            if (arr == null) return null;
            byte[] data = new byte[8];
            DateTime[] result = new DateTime[arr.Length / data.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < result.Length; i++)
            {
                stream.Read(data);
                result[i] = data.GetDateTime();
            }
            return result;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this DateTime[] value)
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
        /// 转为 DateTime
        /// </summary>
        public static DateTime GetDateTime(this string str)
        {
            if (str != "" && str != null)
            {
                return DateTime.Parse(str);
            }
            else
            {
                return default(DateTime);
            }
        }
        /// <summary>
        /// 转为 DateTime[]
        /// </summary>
        public static DateTime[] GetDateTimes(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                return datas.Select((data) => DateTime.Parse(data)).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this DateTime[] values)
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