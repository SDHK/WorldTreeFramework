
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 17:41

* 描述： 

*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameWork
{
    public static partial class SqliteDataReaderExtension
    {

        /// <summary>
        /// 获取 Long
        /// </summary>
        public static long GetLongData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetLong();
        }
        /// <summary>
        /// 获取 Long[]
        /// </summary>
        public static long[] GetLongsData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetLongs();
        }
        /// <summary>
        /// 转为 Long
        /// </summary>
        public static long GetLong(this byte[] arr)
        {
            return BitConverter.ToInt64(arr, 0);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this long value)
        {
            return BitConverter.GetBytes(value);
        }


        /// <summary>
        /// 转为 long[]
        /// </summary>
        public static long[] GetLongs(this byte[] arr)
        {
            if (arr == null) return null;
            byte[] data = new byte[8];
            long[] result = new long[arr.Length / data.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < result.Length; i++)
            {
                stream.Read(data);
                result[i] = data.GetLong();
            }
            return result;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this long[] value)
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
        /// 转为 long
        /// </summary>

        public static long GetLong(this string str)
        {
            if (str != "" && str != null)
            {
                return int.Parse(str);
            }
            else
            {
                return default(int);
            }
        }

        /// <summary>
        /// 转为 long[]
        /// </summary>
        public static long[] GetLongs(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                return datas.Select((data) => long.Parse(data)).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this long[] values)
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